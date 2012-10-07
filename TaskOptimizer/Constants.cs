#define USE_RIVERWOOD

namespace TaskOptimizer
{
    public class Constants
    {
#if USE_RIVERWOOD
        public static string RootDirectory = @"X:\Work\TaskOptimizer\TaskOptimizer\";
        public static string RedisServer = "192.168.2.31:6379";
        public static string OSRMServer = "http://192.168.2.31:5000/";
#else
        public static string RootDirectory = "C:/FoundOPS/TaskOptimizer/";
        public static string RedisServer = "192.168.0.104:6379";
        public static string OSRMServer = "http://192.168.0.104:5000/";
#endif
    }
}
