using System;

namespace GR8Tech.TestUtils.RedisClient.Settings
{
    public sealed class WaiterSettings
    {
        public int? RetryCount { get; set; }
        public TimeSpan? Interval { get; set; }
    }
}