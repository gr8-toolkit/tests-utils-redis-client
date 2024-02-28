namespace GR8Tech.TestUtils.RedisClient.Settings
{
    public sealed class RedisClientSettings
    {
        public WaiterSettings? WaiterSettings { get; set; }

        public string ConnectionString { get; set; }

        public string? SerializerTypeString { get; set; }

        public string? DeserializerTypeString { get; set; }
    }
}