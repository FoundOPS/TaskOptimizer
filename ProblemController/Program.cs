using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using ProblemLib.DataModel;
using ProblemLib.Logging;

namespace ProblemController
{
    class Program
    {
        // Basic structure for problem controller
        static void Main(string[] args)
        {
            ConsoleLogger console = new ConsoleLogger();
            GlobalLogger.AttachLogger(console);
            console.Run();

            TcpClient client = new TcpClient();
            client.Connect("localhost", 17924);
            Stream s = client.GetStream();
            BinaryWriter bw = new BinaryWriter(s);

            Random r = new Random();
            while (true)
            {
                Task t = new Task((UInt32)r.Next(), (Single)r.NextDouble(), (Single)r.NextDouble());
                GlobalLogger.SendLogMessage("ClientTest", "Task sent {{{0}, {1}, {2}}}", t.TaskID, t.Longitude, t.Latitude);

                t.WriteToStream(bw);

                System.Threading.Thread.Sleep(5000);
            }
            

            Console.ReadKey();

            client.Close();
        }
    }
}
