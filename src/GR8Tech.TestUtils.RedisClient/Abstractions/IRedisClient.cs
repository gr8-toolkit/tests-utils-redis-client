using System.Threading.Tasks;
using StackExchange.Redis;

namespace GR8Tech.TestUtils.RedisClient.Abstractions
{
    public interface IRedisClient : IWaiter, IHaveDeserializer, IHaveSerializer
    {
        /// <summary>
        /// The name of RedisClient defined in test-settings.json or appsettings.json
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// Asynchronously adds string value to Redis. Under the hood implements a ConnectionMultiplexer method - StringSetAsync()
        /// </summary>
        /// <param name="dbNumber">The number of a database in Redis</param>
        /// <param name="key">The key of a new entry to Redis</param>
        /// <param name="value">The value of a new entry to Redis</param>
        Task AddAsync(int dbNumber, string key, string value);
        
        /// <summary>
        /// Asynchronously adds object value to Redis. Under the hood implements a ConnectionMultiplexer method - StringSetAsync() with Serializer
        /// </summary>
        /// <param name="dbNumber">The number of a database in Redis</param>
        /// <param name="key">The key of a new entry to Redis</param>
        /// <param name="value">The value of a new entry to Redis</param>
        /// <param name="serializer">Your custom Serializer in case of any, default Serializer is present</param>
        Task AddAsync(int dbNumber, string key, object value, ISerializer serializer = null);
        
        /// <summary>
        /// Asynchronously gets value from Redis by key. Under the hood implements a ConnectionMultiplexer method - StringGetAsync()
        /// </summary>
        /// <param name="dbNumber">The number of a database</param>
        /// <param name="key">The key of an entry in Redis</param>
        /// <returns>Redis entry value if it is present or null in case if not</returns>
        Task<string> GetAsync(int dbNumber, string key);
        
        /// <summary>
        /// Asynchronously gets value from Redis by key. Under the hood implements a ConnectionMultiplexer method - StringGetAsync() with Deserializer
        /// </summary>
        /// <param name="dbNumber">The number of a database</param>
        /// <param name="key">The key of an entry in Redis</param>
        /// <param name="deserializer">Your custom Deserializer in case of any, default Deserializer is present</param>
        /// <typeparam name="T">Deserialized type</typeparam>
        /// <returns>Redis entry value if it is present or null in case if not</returns>
        Task<T> GetAsync<T>(int dbNumber, string key, IDeserializer deserializer = null);
        
        /// <summary>
        /// Asynchronously adds a new hash entry to Redis. Under the hood implements a ConnectionMultiplexer method - HashSetAsync()
        /// </summary>
        /// <param name="dbNumber">The number of a database</param>
        /// <param name="key">The key of a hash entry to Redis</param>
        /// <param name="hashField">the name of a hash field</param>
        /// <param name="hashValue">the value of a hash entry</param>
        Task AddHashAsync(int dbNumber, string key, string hashField, string hashValue);
        
        /// <summary>
        /// Asynchronously adds a new hash entry to Redis. Under the hood implements a ConnectionMultiplexer method - HashSetAsync() with Serializer
        /// </summary>
        /// <param name="dbNumber">The number of a database</param>
        /// <param name="key">The key of a hash entry to Redis</param>
        /// <param name="hashField">the name of a hash field</param>
        /// <param name="hashValue">the value of a hash entry</param>
        /// <param name="serializer">Your custom Serializer in case of any, default Serializer is present</param>
        Task AddHashAsync(int dbNumber, string key, string hashField, object hashValue, ISerializer serializer = null);
        
        /// <summary>
        /// Asynchronously gets hash entry value from Redis by key. Under the hood implements a ConnectionMultiplexer method - HashGetAsync()
        /// </summary>
        /// <param name="dbNumber">The number of a database</param>
        /// <param name="key">The key of a hash entry from Redis</param>
        /// <param name="hashField">the name of a hash field</param>
        /// <returns>The hash entry value if it is present or null in case if not</returns>
        Task<string> GetHashAsync(int dbNumber, string key, string hashField);
        
        /// <summary>
        /// Asynchronously gets hash entry value from Redis by key. Under the hood implements a ConnectionMultiplexer method - HashGetAsync() with Deserializer
        /// </summary>
        /// <param name="dbNumber">The number of a database</param>
        /// <param name="key">The key of a hash entry from Redis</param>
        /// <param name="hashField">the name of a hash field</param>
        /// <param name="deserializer">Your custom Deserializer in case of any, default Deserializer is present</param>
        /// <typeparam name="T">Deserialized type</typeparam>
        /// <returns>The hash entry value if it is present or null in case if not</returns>
        Task<T> GetHashAsync<T>(int dbNumber, string key, string hashField, IDeserializer deserializer = null);
        
        /// <summary>
        /// Asynchronously delete entry by key from Redis database. Under the hood implements a ConnectionMultiplexer method - KeyDeleteAsync()
        /// </summary>
        /// <param name="dbNumber">The number of a database</param>
        /// <param name="key">The key of an entry to delete from Redis</param>
        Task DeleteKeyAsync(int dbNumber, string key);
        
        /// <summary>
        /// Asynchronously delete hash entry by key and hashField from Redis database. Under the hood implements a ConnectionMultiplexer method - HashDeleteAsync()
        /// </summary>
        /// <param name="dbNumber">The number of a database</param>
        /// <param name="key">The key of a hash entry to delete from Redis</param>
        /// <param name="hashField">the name of a hash field to delete from Redis</param>
        Task DeleteHashAsync(int dbNumber, string key, string hashField);
        
        /// <summary>
        /// Asynchronously clean Redis database by number. Under the hood implements a ConnectionMultiplexer method - FlushDatabaseAsync()
        /// </summary>
        /// <param name="dbNumber">The number of a database</param>
        Task CleanAsync(int dbNumber);
        
        /// <summary>
        /// An access to ConnectionMultiplexer and all it's methods in case you need more
        /// </summary>
        IConnectionMultiplexer ConnectionMultiplexer { get; }
    }
}


