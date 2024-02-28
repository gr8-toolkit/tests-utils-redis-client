using System;
using System.Linq;
using System.Threading.Tasks;
using GR8Tech.TestUtils.RedisClient.Abstractions;
using GR8Tech.TestUtils.RedisClient.Common.Logging;
using GR8Tech.TestUtils.RedisClient.Settings;
using Serilog;
using StackExchange.Redis;

namespace GR8Tech.TestUtils.RedisClient.Implementation
{
    internal sealed class RedisClient : IRedisClient
    {
        private ILogger _logger;
        private int _waiterRetries;
        private TimeSpan _waiterInterval;

        internal RedisClient(string name, RedisClientSettings options)
        {
            _logger = Log.Logger
                .ForContext<RedisClient>()
                .AddPrefix($"{nameof(RedisClient)}")
                .AddHiddenPrefix(name);

            Name = name;
            ConnectionMultiplexer = StackExchange.Redis.ConnectionMultiplexer.Connect(options.ConnectionString);

            _waiterRetries = options.WaiterSettings?.RetryCount ?? 30;
            _waiterInterval = options.WaiterSettings?.Interval ?? TimeSpan.FromSeconds(1);

            if (!string.IsNullOrEmpty(options.DeserializerTypeString))
                Deserializer = (IDeserializer)Activator.CreateInstance(Type.GetType(options.DeserializerTypeString)!);
            else
                Deserializer = new DefaultDeserializer();

            if (!string.IsNullOrEmpty(options.SerializerTypeString))
                Serializer = (ISerializer)Activator.CreateInstance(Type.GetType(options.SerializerTypeString)!);
            else
                Serializer = new DefaultSerializer();

            _logger
                .AddPayload("RedisClientOptions", options)
                .Information("RedisClient '{name}' has been initialized", Name);
        }

        public IDeserializer Deserializer { get; set; }

        public ISerializer Serializer { get; set; }

        public IConnectionMultiplexer ConnectionMultiplexer { get; }

        public string Name { get; set; }

        public async Task<T> WaitAndGetAsync<T>(
            Func<IRedisClient, Task<T>> clientFunc,
            Func<T, bool> filter,
            int? retries = null,
            TimeSpan? interval = null)
        {
            retries ??= _waiterRetries;
            interval ??= _waiterInterval;

            var policy = PollyPolicies.GetDefaultAsyncRetryPolicy<T>((int)retries!, (TimeSpan)interval!);

            var waitResult = await policy.ExecuteAsync(async () =>
            {
                var result = await clientFunc(this);

                return (filter(result) ? result : default)!;
            });

            _logger
                .AddPayload("WaitResult", waitResult)
                .Information("{action} succeeded", nameof(WaitAndGetAsync));

            return waitResult;
        }

        public async Task AddAsync(int dbNumber, string key, string value)
        {
            try
            {
                await ConnectionMultiplexer
                    .GetDatabase(dbNumber)
                    .StringSetAsync(key, value);

                _logger
                    .AddPayload("AddRequest", new { dbNumber, key, value })
                    .Information("{action} succeeded", nameof(AddAsync));
            }
            catch (Exception e)
            {
                _logger
                    .AddPayload("AddRequest", new { dbNumber, key, value })
                    .Error(e, "{action} failed", nameof(AddAsync));

                throw;
            }
        }

        public async Task AddAsync(int dbNumber, string key, object value, ISerializer serializer = null)
        {
            try
            {
                var serializedObject = serializer == null ? Serializer.Serialize(value) : serializer.Serialize(value);
                await ConnectionMultiplexer
                    .GetDatabase(dbNumber)
                    .StringSetAsync(key, serializedObject);

                _logger
                    .AddPayload("AddRequest", new { dbNumber, key, value })
                    .Information("{action} succeeded", nameof(AddAsync));
            }
            catch (Exception e)
            {
                _logger
                    .AddPayload("AddRequest", new { dbNumber, key, value })
                    .Error(e, "{action} failed", nameof(AddAsync));

                throw;
            }
        }

        public async Task<string> GetAsync(int dbNumber, string key)
        {
            try
            {
                var result = await ConnectionMultiplexer
                    .GetDatabase(dbNumber)
                    .StringGetAsync(key);

                _logger
                    .AddPayload("GetResult", new { dbNumber, key, result = result.ToString() })
                    .Information("{action} succeeded", nameof(GetAsync));

                return result;
            }
            catch (Exception e)
            {
                _logger
                    .AddPayload("GetRequest", new { dbNumber, key })
                    .Error(e, "{action} failed", nameof(GetAsync));

                throw;
            }
        }

        public async Task<T> GetAsync<T>(int dbNumber, string key, IDeserializer deserializer = null)
        {
            try
            {
                var value = await ConnectionMultiplexer
                    .GetDatabase(dbNumber)
                    .StringGetAsync(key);

                var result = deserializer == null
                    ? Deserializer.Deserialize<T>(value)
                    : deserializer.Deserialize<T>(value);

                _logger
                    .AddPayload("GetResult", new { dbNumber, key, result })
                    .Information("{action} succeeded", nameof(GetAsync));

                return result;
            }
            catch (Exception e)
            {
                _logger
                    .AddPayload("GetRequest", new { dbNumber, key })
                    .Error(e, "{action} failed", nameof(GetAsync));

                throw;
            }
        }

        public async Task AddHashAsync(int dbNumber, string key, string hashField, string hashValue)
        {
            try
            {
                await ConnectionMultiplexer
                    .GetDatabase(dbNumber)
                    .HashSetAsync(key, hashField, hashValue);

                _logger
                    .AddPayload("SetHashResult", new { dbNumber, key, hashField, hashValue })
                    .Information("{action} succeeded", nameof(GetAsync));
            }
            catch (Exception e)
            {
                _logger
                    .AddPayload("SetHashRequest", new { dbNumber, key, hashField, hashValue })
                    .Error(e, "{action} failed", nameof(GetAsync));

                throw;
            }
        }

        public async Task AddHashAsync(
            int dbNumber,
            string key,
            string hashField,
            object hashValue,
            ISerializer serializer = null)
        {
            try
            {
                var serializedObject =
                    serializer == null ? Serializer.Serialize(hashValue) : serializer.Serialize(hashValue);

                await ConnectionMultiplexer
                    .GetDatabase(dbNumber)
                    .HashSetAsync(key, hashField, serializedObject);

                _logger
                    .AddPayload("SetHashResult", new { dbNumber, key, hashField, hashValue })
                    .Information("{action} succeeded", nameof(GetAsync));
            }
            catch (Exception e)
            {
                _logger
                    .AddPayload("SetHashRequest", new { dbNumber, key, hashField, hashValue })
                    .Error(e, "{action} failed", nameof(GetAsync));

                throw;
            }
        }


        public async Task<string> GetHashAsync(
            int dbNumber,
            string key,
            string hashField)
        {
            try
            {
                var hashValue = await ConnectionMultiplexer
                    .GetDatabase(dbNumber)
                    .HashGetAsync(key, hashField);

                _logger
                    .AddPayload("GetHashResult", new { dbNumber, key, hashField, hashValue })
                    .Information("{action} succeeded", nameof(GetHashAsync));

                return hashValue;
            }
            catch (Exception e)
            {
                _logger
                    .AddPayload("GetHashRequest", new { dbNumber, key, hashField })
                    .Error(e, "{action} failed", nameof(GetHashAsync));

                throw;
            }
        }

        public async Task<T> GetHashAsync<T>(
            int dbNumber,
            string key,
            string hashField,
            IDeserializer deserializer = null)
        {
            try
            {
                var hashValue = await ConnectionMultiplexer
                    .GetDatabase(dbNumber)
                    .HashGetAsync(key, hashField);

                var result = deserializer == null
                    ? Deserializer.Deserialize<T>(hashValue)
                    : deserializer.Deserialize<T>(hashValue);

                _logger
                    .AddPayload("GetHashResult", new { dbNumber, key, hasField = hashField, result })
                    .Information("{action} succeeded", nameof(GetHashAsync));

                return result;
            }
            catch (Exception e)
            {
                _logger
                    .AddPayload("GetHashRequest", new { dbNumber, key, hashField })
                    .Error(e, "{action} failed", nameof(GetHashAsync));

                throw;
            }
        }

        public async Task DeleteKeyAsync(int dbNumber, string key)
        {
            try
            {
                await ConnectionMultiplexer.GetDatabase(dbNumber).KeyDeleteAsync(key);

                _logger
                    .AddPayload("DeleteKeyResult", new { dbNumber, key })
                    .Information("{action} succeeded", nameof(DeleteKeyAsync));
            }
            catch (Exception e)
            {
                _logger
                    .AddPayload("DeleteKeyRequest", new { dbNumber, key })
                    .Error(e, "{action} failed", nameof(DeleteKeyAsync));

                throw;
            }
        }

        public async Task DeleteHashAsync(int dbNumber, string key, string hashField)
        {
            try
            {
                await ConnectionMultiplexer.GetDatabase(dbNumber).HashDeleteAsync(key, hashField);

                _logger
                    .AddPayload("DeleteHashResult", new { dbNumber, key, hashField })
                    .Information("{action} succeeded", nameof(DeleteHashAsync));
            }
            catch (Exception e)
            {
                _logger
                    .AddPayload("DeleteHashRequest", new { dbNumber, key, hashField })
                    .Error(e, "{action} failed", nameof(DeleteHashAsync));

                throw;
            }
        }

        public async Task CleanAsync(int dbNumber)
        {
            try
            {
                var endpoints = ConnectionMultiplexer.GetEndPoints();
                await ConnectionMultiplexer.GetServer(endpoints.First()).FlushDatabaseAsync(dbNumber);

                _logger
                    .AddPayload("CleanResult", new { dbNumber })
                    .Information("{action} succeeded", nameof(CleanAsync));
            }
            catch (Exception e)
            {
                _logger
                    .AddPayload("CleanRequest", new { dbNumber })
                    .Error(e, "{action} failed", nameof(CleanAsync));

                throw;
            }
        }
    }
}