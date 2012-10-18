using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProblemLib.Logging;

namespace ProblemDistribution
{
    class Program
    {
        // Basic structure for one problem distribution
        static void Main(string[] args)
        {
            ConsoleLogger console = new ConsoleLogger();
            GlobalLogger.AttachLogger(console);
            console.Run();

            DistributionServer server = new DistributionServer();
        }
    }
}
