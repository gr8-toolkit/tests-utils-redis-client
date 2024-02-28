namespace GR8Tech.TestUtils.RedisClient.Abstractions
{
    public interface IDeserializer
    {
        public T Deserialize<T>(string obj);
    }
}

