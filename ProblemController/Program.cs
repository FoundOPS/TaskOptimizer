using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
            Stream s = client.GetStream();
            BinaryWriter bw = new BinaryWriter(s);

            Random r = new Random();
            try
            {
                bw.Write((UInt16)0xD501);

                DistributionConfiguration c = new DistributionConfiguration();
                c.ControllerServer = IPAddress.Parse("127.0.0.1");
                c.ControllerServerPort = 3879;
                c.RedisServer = IPAddress.Parse("192.168.2.31");
                c.RedisServerPort = 6379;
                c.OsrmServer = IPAddress.Parse("192.168.2.31");
                c.OsrmServerPort = 5000;
                c.RandomSeed = 1234567;
                c.Workers = new Worker[] { new Worker() { WorkerID = 0 }, new Worker() { WorkerID = 1 }, new Worker() { WorkerID = 2 } };
                c.Clusters = new Task[][] {
                    new Task[] { new Task(0, 1111.1111F, 2222.2222F), new Task(0, 1111.1111F, 2222.2222F),new Task(0, 1111.1111F, 2222.2222F) ,new Task(0, 1111.1111F, 2222.2222F)},
                    new Task[] { new Task(0, 5511.1111F, 5522.2222F), new Task(0, 5511.1111F, 5522.2222F),new Task(0, 5511.1111F, 5522.2222F) ,new Task(0, 5511.5511F, 5522.2222F)},
                    new Task[] { new Task(0, 8811.1111F, 2222.2222F), new Task(0, 8811.1111F, 2222.2222F),new Task(0, 8811.1111F, 2222.2222F) ,new Task(0, 8811.1111F, 2222.2222F)},
                };

                c.WriteToStream(bw);
           }
            finally
            {
                bw.Write((ushort)0xD502); // emulate disconnect code
                client.Close();
            }
            
            Console.ReadKey();
        }
    }
}
