using System;
using System.Collections.Generic;
using Polly;

namespace GR8Tech.TestUtils.RedisClient.Implementation
{
    internal static class PollyPolicies
    {
        internal static AsyncPolicy<T> GetDefaultAsyncRetryPolicy<T>(int retryCount, TimeSpan sleepDuration)
        {
            return Policy
                .Handle<Exception>()
                .OrResult<T>(result => EqualityComparer<T>.Default.Equals(result, default!))
                .WaitAndRetryAsync(retryCount, i => sleepDuration);
        }
    }
}

