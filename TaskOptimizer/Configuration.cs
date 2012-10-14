using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace TaskOptimizer
{
    public class Configuration
    {
        private static Configuration _instance = null;
        public static Configuration Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException("Cannot get configuration at this time, please initialize the instance first!");
                return _instance;
            }
        }
        public static void Initialize(String confFile)
        {
            if (!File.Exists(confFile))
                throw new FileNotFoundException();

            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(confFile);

            _instance = new Configuration();
            _instance.RootDirectory = xdoc.SelectSingleNode("TaskOptimizerConfiguration/RootDir").InnerText;
            _instance.RedisServer = xdoc.SelectSingleNode("TaskOptimizerConfiguration/RedisServer").InnerText;
            _instance.OSRMServer = "http://" + xdoc.SelectSingleNode("TaskOptimizerConfiguration/OSRMServer").InnerText + "/";
        }
        public static void Initialize()
        {
            String path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, "config.xml");
            Initialize(path);
        }

        /// <summary>
        /// Root data directory of the program.
        /// </summary>
        public String RootDirectory { get; private set; }

        /// <summary>
        /// Address of the redis server.
        /// </summary>
        public String RedisServer { get; private set; }

        /// <summary>
        /// Address of the OSRM server
        /// </summary>
        public String OSRMServer { get; private set; }
    }
}
