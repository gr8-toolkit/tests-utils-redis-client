using System.Collections.Generic;

namespace GR8Tech.TestUtils.RedisClient.Settings
{
    internal sealed class RedisSettings
    {
        public DefaultSettings? DefaultSettings { get; set; }

        public Dictionary<string, RedisClientSettings> RedisClients { get; set; }
    }

    internal sealed class DefaultSettings
    {
        public WaiterSettings? WaiterSettings { get; set; }

        public string? SerializerTypeString { get; set; }

        public string? DeserializerTypeString { get; set; }
    }
}