using System;
using System.IO;
using GR8Tech.TestUtils.RedisClient.Common.Logging;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace GR8Tech.TestUtils.RedisClient.Settings
{
    internal static class SettingsProvider
    {
        static SettingsProvider()
        {
            var configFileName = File.Exists("test-settings.json") ? "test-settings" : "appsettings";
            Config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"{configFileName}.json", false, true)
                .AddJsonFile(
                    $"{configFileName}.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
                .AddEnvironmentVariables()
                .Build()
                .GetSection("RedisSettings")
                .Get<RedisSettings>()!
                .SetCascadeSettings();

            Log.Logger
                .ForContextStaticClass(typeof(SettingsProvider))
                .AddPayload("RedisSettings", Config)
                .Information("RedisSettings have been read and initialized");
        }

        public static RedisSettings Config { get; }

        private static RedisSettings SetCascadeSettings(this RedisSettings settings)
        {
            var defaultSettings = settings.DefaultSettings ?? new DefaultSettings();
            var defaultWaiterOptions = defaultSettings.WaiterSettings ?? new WaiterSettings();

            foreach (var client in settings.RedisClients)
            {
                client.Value.WaiterSettings ??= defaultWaiterOptions;
                client.Value.WaiterSettings.RetryCount ??= defaultWaiterOptions.RetryCount;
                client.Value.WaiterSettings.Interval ??= defaultWaiterOptions.Interval;
                client.Value.SerializerTypeString ??= defaultSettings.SerializerTypeString;
                client.Value.DeserializerTypeString ??= defaultSettings.DeserializerTypeString;
            }

            return settings;
        }
    }
}