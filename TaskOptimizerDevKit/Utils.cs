using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TaskOptimizerDevKit
{
    static class Utils
    {
        public static Boolean IsValidIp(String str)
        {
            IPAddress addr;
            return IPAddress.TryParse(str, out addr);
        }

        public static IPAddress ToIpAddress(String str)
        {
            IPAddress addr;
            if (!IPAddress.TryParse(str, out addr))
                return null;
            else
                return addr;
        }
    }
}
