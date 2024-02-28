using GR8Tech.TestUtils.RedisClient.Abstractions;
using Newtonsoft.Json;

namespace GR8Tech.TestUtils.RedisClient.Implementation
{
    internal sealed class DefaultDeserializer : IDeserializer
    {
        public T Deserialize<T>(string obj)
        {
            return JsonConvert.DeserializeObject<T>(obj);
        }
    }
}

