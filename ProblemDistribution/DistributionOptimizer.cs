using System;
using ProblemLib.DataModel;
using ProblemLib.API;

namespace ProblemDistribution
{
    class DistributionOptimizer
    {
        public DistributionConfiguration Configuration { get; private set; }
        public Osrm Osrm { get; private set; }

        public void Initialize(DistributionConfiguration config)
        {
            Configuration = config;
            
            // create osrm instance
            String redisAddress = String.Format("{0}:{1}", config.RedisServer, config.RedisServerPort);
            String osrmAddress = String.Format("http://{0}:{1}/", config.OsrmServer, config.OsrmServerPort);
            Osrm = Osrm.GetInstance(config.ProblemID, redisAddress, osrmAddress);

            
        }

    }
}
