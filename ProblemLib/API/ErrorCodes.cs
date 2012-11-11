using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProblemLib.API
{
    public static class ErrorCodes
    {
        public const UInt32 UnknownError = 0xC8FF;

        public const UInt32 RedisConnectionFailed = 0xC800;
        public const UInt32 OsrmConnectionFailed = 0xC801;

        public const UInt32 RedisDistanceTimeCacheCorrupted = 0xC802;
    }
}
