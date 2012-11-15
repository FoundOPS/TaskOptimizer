using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProblemLib.API
{
    public static class ErrorCodes
    {
        public const UInt32 UnknownError = 0xC8FF;

        // Network errors
        public const UInt32 NetworkError = 0xC802;
        public const UInt32 RedisConnectionFailed = 0xC800;
        public const UInt32 OsrmConnectionFailed = 0xC801;

        // Other errors
        public const UInt32 RedisCacheCorrupted = 0xC902;
        public const UInt32 AttemptToAddRedundantCacheEntry = 0xC903;
    }
}
