using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ProblemController
{
    class Program
    {
        // Basic structure for problem controller
        static void Main(string[] args)
        {
            TcpClient client = new TcpClient();
            client.Connect("localhost", 17924);

            Console.ReadKey();

            client.Close();
        }
    }
}
