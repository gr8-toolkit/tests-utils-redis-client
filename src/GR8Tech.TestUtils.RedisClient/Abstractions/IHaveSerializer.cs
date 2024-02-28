namespace GR8Tech.TestUtils.RedisClient.Abstractions
{
    public interface IHaveSerializer
    {
        ISerializer Serializer { get; set; }
    }
}

