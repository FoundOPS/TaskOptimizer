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
        const int Port = 17924;

        private readonly TcpListener _tcpListener;
        private readonly Thread _tcpThread;

        private Boolean _continueRunning = true;

        public DistributionServer()
        {
            this._tcpListener = new TcpListener(IPAddress.Any, Port);
            this._tcpThread = new Thread(new ThreadStart(ListenForClinet));
            this._tcpThread.Start();
        }

        private void ListenForClinet()
        {
            GlobalLogger.SendLogMessage("Server", "Distribution Server started at {0}", _tcpListener.LocalEndpoint.ToString());

            this._tcpListener.Start();

            while (_continueRunning)
            {
                var client = this._tcpListener.AcceptTcpClient();

                var clientThread = new Thread(new ParameterizedThreadStart(StartNewClient));
                clientThread.Start(client);
            }
        }

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

            // Create Optimizer
            var optimizer = new DistributionOptimizer();

            // Start accepting messages
            while (runClientThread)
            {
                while (client.Available < 2) Thread.Sleep(10); // wait 10ms for data

                ushort code = br.ReadUInt16();

                switch (code)
                {
                    case ControlCodes.SendingConfiguration: // configuration data coming!
                        {
                            try
                            {
                                var cfg = DistributionConfiguration.ReadFromStream(client, br);

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

                                optimizer.Initialize(cfg);

                                // send back acknowledge
                                bw.Write(ControlCodes.Acknowledge);
                            } 
                            catch (Exception ex)
                            {
                                GlobalLogger.SendLogMessage("Error", "An exception of type {0} occured: {1}", ex.GetType().FullName, ex.Message);
                                // send back error code
                                bw.Write(ControlCodes.Error);
                            }
                        }
                        break;
                    case ControlCodes.TerminateConnection: // Terminate connection!
                        runClientThread = false;
                        break;
                }



                if (code == ControlCodes.SendingConfiguration) // configuration signal
                {
                }
                else if (code == ControlCodes.TerminateConnection) // Termination signal
                {
                    break;
                }

            }


            GlobalLogger.SendLogMessage("ServerEvent", "Client Disconnected {0}", client.Client.RemoteEndPoint.ToString());
        }
    }
}
