using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ProblemLib.API;
using ProblemLib.DataModel;
using ProblemLib.Logging;
using ProblemLib.ErrorHandling;
using ProblemDistribution.Model;

namespace ProblemDistribution
{
    /* Supports multiple tasks on the same machine via the same instance
     * each task assigned a guid
     * supported actions:
     *  - Start task (supplies task configuration data)
     *  - Get status
     *  - Abort/Dispose task
     *  - Get Task Fitness
     *  - Get Task Result
     */

    public class DistributionServer
    {
        /// <summary>
        /// Port number that this server uses
        /// </summary>
        const int Port = 17924;

        private readonly TcpListener _tcpListener;
        private readonly Thread _tcpThread;

        private Boolean _continueRunning = true;

        /// <summary>
        /// Constructor.
        /// Starts the listener thread.
        /// </summary>
        public DistributionServer()
        {
            this._tcpListener = new TcpListener(IPAddress.Any, Port);
            this._tcpThread = new Thread(new ThreadStart(ListenForClients));
            this._tcpThread.Start();
        }

        /// <summary>
        /// Async method that listens for client connections and spawns new threads for each of them.
        /// </summary>
        private void ListenForClients()
        {
            GlobalLogger.SendLogMessage("Server", "Distribution Server started at {0}", _tcpListener.LocalEndpoint.ToString());

            this._tcpListener.Start();

            while (_continueRunning)
            {
                var client = this._tcpListener.AcceptTcpClient(); // wait for connection

                // when connection accepted, spawn new thread
                var clientThread = new Thread(new ParameterizedThreadStart(StartNewClient)); 
                clientThread.Start(client);
            }
        }
        /// <summary>
        /// Async method that handles connection for each of the connected clients.
        /// </summary>
        /// <param name="tcpClient"></param>
        private void StartNewClient(Object tcpClient)
        {
            Boolean runClientThread = true;

            // Get client 
            var client = (TcpClient)tcpClient;
            GlobalLogger.SendLogMessage("ServerEvent", "Client Connected {0}", client.Client.RemoteEndPoint.ToString());

            // Get client stream and create writer/reader
            Stream s = client.GetStream();
            var br = new BinaryReader(s);
            var bw = new BinaryWriter(s);

            // Create Optimizer for this client
            var optimizer = new DistributionOptimizer(this);

            // Start accepting messages
            while (runClientThread)
            {
                while (client.Available < 2) Thread.Sleep(10); // wait 10ms for control code

                ushort code = br.ReadUInt16(); // read control code

                switch (code)
                {
                    case ControlCodes.SendingConfiguration: // configuration data coming!
                        {
                            try
                            {
                                // read configuration from network stream
                                var cfg = DistributionConfiguration.ReadFromStream(client, br);

                                // Print debug info
                                GlobalLogger.SendLogMessage("Server", "Distribution Configuration Received!");
                                GlobalLogger.SendLogMessage("Server", "Problem ID: {0}", cfg.ProblemID.ToString());
                                GlobalLogger.SendLogMessage("Server", "Controller IP: {0}:{1}",
                                                            cfg.ControllerServer.ToString(), cfg.ControllerServerPort);
                                GlobalLogger.SendLogMessage("Server", "Redis IP: {0}:{1}", cfg.RedisServer.ToString(),
                                                            cfg.RedisServerPort);
                                GlobalLogger.SendLogMessage("Server", "OSRM IP: {0}:{1}", cfg.OsrmServer.ToString(),
                                                            cfg.OsrmServerPort);
                                GlobalLogger.SendLogMessage("Server", "Worker Count: {0}", cfg.Workers.Length);
                                GlobalLogger.SendLogMessage("Server", "Task Count: {0}", cfg.Tasks.Length);

                                // initalize the optimizer using the configuration
                                optimizer.Initialize(cfg);

                                // send back acknowledge
                                bw.Write(ControlCodes.Acknowledge);
                            }
                            catch (ProblemLibException x) // ProblemLibException represents expected errors with assigned error codes
                            { // Expected errors are handed back to the controller
                                GlobalLogger.SendLogMessage("Error", "An expected error was caught in DistributionServer.StartNewClient()");
                                GlobalLogger.SendLogMessage("Error", "Error-{0}: {1}", x.ErrorCode, x.InnerException != null ? x.InnerException.Message : "null");
                                // send back error info
                                bw.Write(ControlCodes.Error);
                                bw.Write(x.ErrorCode);
                                bw.Write(x.TimeStamp.Ticks);
                            }
                            catch (Exception ex) // any other exception is unexpected
                            {
                                GlobalLogger.SendLogMessage("Error", "An unexpected exception of type {0} occured: {1}", ex.GetType().FullName, ex.Message);
                                // send back error info
                                bw.Write(ControlCodes.Error);
                                bw.Write(ErrorCodes.UnknownError);
                                bw.Write(DateTime.Now.Ticks);
                            }
                        }
                        break;
                    case ControlCodes.TerminateConnection: // Terminate connection!
                        {
                            runClientThread = false;
                            // NOTE release resources here!


                            // Send ACK and terminate connection
                            bw.Write(ControlCodes.Acknowledge);
                            client.Close();
                        }
                        break;
                }

            }


            GlobalLogger.SendLogMessage("ServerEvent", "Client Disconnected {0}", client.Client.RemoteEndPoint.ToString());
        }
    }
}
