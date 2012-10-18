using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ProblemController.Logging;

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
        const int PORT = 17924;

        private TcpListener tcpListener;
        private Thread tcpThread;

        private Boolean continueRunning = true;



        public DistributionServer()
        {
            this.tcpListener = new TcpListener(IPAddress.Any, PORT);
            this.tcpThread = new Thread(new ThreadStart(ListenForClinet));
            this.tcpThread.Start();
        }

        private void ListenForClinet()
        {
            GlobalLogger.SendLogMessage("Server", "Distribution Server started at {0}", tcpListener.LocalEndpoint.ToString());

            this.tcpListener.Start();

            while (continueRunning)
            {
                TcpClient client = this.tcpListener.AcceptTcpClient();

                Thread clientThread = new Thread(new ParameterizedThreadStart(StartNewClient));
                clientThread.Start(client);
            }
        }

        private void StartNewClient(Object tcpClient)
        {
            TcpClient client = (TcpClient)tcpClient;
            GlobalLogger.SendLogMessage("ServerTest", "Client Connected {0}", client.Client.RemoteEndPoint.ToString());

            // TODO get data, resolve it and pass it to the distribution calculator


        }
    }
}
