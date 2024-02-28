using System;
using System.Threading.Tasks;

namespace GR8Tech.TestUtils.RedisClient.Abstractions
{
    public interface IWaiter
    {
        /// <summary>
        /// Method waits some data in Redis with a defined function 'clientFunc' and predicate 'filter'
        /// </summary>
        /// <param name="clientFunc">An action to perform by RedisClient</param>
        /// <param name="filter">Predicate to filter RedisClient results</param>
        /// <param name="retries">how many retries should occur (default = 30)</param>
        /// <param name="interval">delay Timespan between retries (default = "00:00:01")</param>
        /// <typeparam name="T">'clientFunc' result type</typeparam>
        /// <example>
        /// <code>
        /// await redisClient.WaitAndGetAsync(
        ///     c => c.GetAsync(0, key),
        ///     r => r != null);
        /// </code>
        /// </example>
        /// <returns>RedisClient results if they satisfy the filter</returns>
        Task<T> WaitAndGetAsync<T>(
            Func<IRedisClient, Task<T>> clientFunc,
            Func<T, bool> filter,
            int? retries = null,
            TimeSpan? interval = null);
    }
}