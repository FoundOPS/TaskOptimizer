using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ProblemLib.API;
using ProblemLib.DataModel;
using ProblemLib.Logging;

namespace ProblemController
{
    class Program
    {
        // Basic structure for problem controller
        static void Main(string[] args)
        {

            var console = new ConsoleLogger();
            GlobalLogger.AttachLogger(console);
            console.Run();

            TcpClient client = new TcpClient();

            try
            {
                client.Connect("localhost", 17924);
            }
            catch (Exception x)
            {
                GlobalLogger.SendLogMessage("Error", "Failed to connect to distribution server: {0}", x.Message);
                console.Stop();
                return;
            }
            var s = client.GetStream();
            var bw = new BinaryWriter(s);
            var br = new BinaryReader(s);

            var r = new Random();
            try
            {
                bw.Write(ControlCodes.SendingConfiguration);

                var c = new DistributionConfiguration();
                c.ProblemID = Guid.NewGuid();
                c.ControllerServer = IPAddress.Parse("127.0.0.1");
                c.ControllerServerPort = 3879;
                c.RedisServer = IPAddress.Parse("192.168.0.115");
                c.RedisServerPort = 6379;
                c.OsrmServer = IPAddress.Parse("192.168.0.115");
                c.OsrmServerPort = 5000;
                c.RandomSeed = 1234567;
                c.Workers = new[] { new Worker() { WorkerID = 0 }, new Worker() { WorkerID = 1 }, new Worker() { WorkerID = 2 } };
                c.Tasks = new[] {
                    new Task(0, 1111.1111F, 2222.2222F), new Task(0, 1111.1111F, 2222.2222F),new Task(0, 1111.1111F, 2222.2222F) ,new Task(0, 1111.1111F, 2222.2222F),
                    new Task(0, 5511.1111F, 5522.2222F), new Task(0, 5511.1111F, 5522.2222F),new Task(0, 5511.1111F, 5522.2222F) ,new Task(0, 5511.5511F, 5522.2222F),
                    new Task(0, 8811.1111F, 2222.2222F), new Task(0, 8811.1111F, 2222.2222F),new Task(0, 8811.1111F, 2222.2222F) ,new Task(0, 8811.1111F, 2222.2222F)
                };

                c.WriteToStream(bw);

                while (client.Available < 2) ;
                UInt16 ctrl = br.ReadUInt16();

                if (ctrl == ControlCodes.Acknowledge)
                    GlobalLogger.SendLogMessage("ControllerEvent", "Distribution server replied with ACK!");
                else if (ctrl == ControlCodes.Error)
                    GlobalLogger.SendLogMessage("DistributionError", "Distribution server encountered an ERROR!");
            }
            finally
            {
                bw.Write(ControlCodes.TerminateConnection); // emulate disconnect code
                client.Close();
                console.Stop(true);
            }
            
            Console.ReadKey();
        }
    }
}
