using System;
using GR8Tech.TestUtils.RedisClient.Settings;

namespace GR8Tech.TestUtils.RedisClient.Abstractions
{
    public interface IRedisClientsFactory
    {
        /// <summary>
        /// Provides RedisClient by name if there are a few in settings file
        /// </summary>
        /// <param name="name">RedisClient name from the settings json file</param>
        /// <returns>RedisClient with the defined name</returns>
        /// <exception cref="Exception">Throws an exception if settings file has zero or there is no such RedisClient name specified in the settings file</exception>
        IRedisClient GetClient(string? name = null);

        /// <summary>
        /// Build your own RedisClient from code apart from other clients defined in the settings json file
        /// </summary>
        /// <param name="name">The name of your new RedisClient</param>
        /// <param name="options">options for RedisClient</param>
        /// <returns>new RedisClient</returns>
        /// <exception cref="Exception">Throws an exception if RedisClient with such name already exists</exception>
        IRedisClient Create(string name, RedisClientSettings options);
    }
}