using GR8Tech.TestUtils.RedisClient.Abstractions;
using Newtonsoft.Json;

namespace GR8Tech.TestUtils.RedisClient.Implementation
{
    internal class DefaultSerializer : ISerializer
    {
        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}