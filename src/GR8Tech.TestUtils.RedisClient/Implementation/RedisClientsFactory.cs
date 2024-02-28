using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using GR8Tech.TestUtils.RedisClient.Abstractions;
using GR8Tech.TestUtils.RedisClient.Common.Logging;
using GR8Tech.TestUtils.RedisClient.Settings;
using Serilog;

namespace GR8Tech.TestUtils.RedisClient.Implementation
{
    public class RedisClientsFactory : IRedisClientsFactory
    {
        private readonly ConcurrentDictionary<string, IRedisClient> _redisClients =
            new ConcurrentDictionary<string, IRedisClient>();

        private ILogger _logger;

        public RedisClientsFactory()
        {
            _logger = Log.Logger
                .ForContext<RedisClientsFactory>()
                .AddPrefix($"{nameof(RedisClientsFactory)}");

            try
            {
                foreach (var client in SettingsProvider.Config.RedisClients)
                    _redisClients.TryAdd(client.Key, new RedisClient(client.Key, client.Value));
            }
            catch (Exception ex)
            {
                if (ex is TypeInitializationException || ex is FileNotFoundException)
                    _logger.Warning("No default RedisClients defined in the setting json file");
                else
                    throw;
            }
        }

        public IRedisClient GetClient(string? name = null)
        {
            if (_redisClients.Count == 0)
                throw new Exception($"There are no any RedisClients defined in the factory yet");

            if (string.IsNullOrEmpty(name) && _redisClients.Count == 1)
                return _redisClients.First(_ => true).Value;

            if (string.IsNullOrEmpty(name) && _redisClients.Count > 1)
                throw new Exception("There are more than 1 RedisClients defined in the factory. Please provide a name");

            if (_redisClients.All(x => x.Key != name))
                throw new Exception($"There is no '{name}' RedisClient defined in the factory yet");

            return _redisClients.First(x => x.Key == name).Value;
        }

        public IRedisClient Create(string name, RedisClientSettings options)
        {
            if (_redisClients.Any(x => x.Key == name))
                throw new Exception($"RedisClient with a name \"{name}\" already exists");

            var client = new RedisClient(name, options);
            _redisClients.TryAdd(name, client);

            return client;
        }
    }
}