using GR8Tech.TestUtils.RedisClient.Abstractions;
using GR8Tech.TestUtils.RedisClient.Common.Logging;
using GR8Tech.TestUtils.RedisClient.Implementation;
using NUnit.Framework;
using Serilog;
using Tests.SetUpFixtures;

namespace Tests;
 
[SetUpFixture]
public static class GlobalSetUp
{
    private static ILogger _logger = SerilogDecorator.Logger.ForContextStaticClass(typeof(GlobalSetUp));
    private static DockerFixture? DockerFixture { get; set; }

    public static IRedisClientsFactory RedisClientFactory { get; set; }

    [OneTimeSetUp]
    public static async Task SetUp()
    {
        Log.Logger = SerilogDecorator.Logger;
        DockerFixture = new DockerFixture();
        RedisClientFactory = new RedisClientsFactory();

        _logger.Information("Redis has started in Docker container");
    }

    [OneTimeTearDown]
    public static void TearDown()
    {
        DockerFixture?.Dispose();

        _logger.Information("Redis Docker container has been stopped and deleted");
    }
}