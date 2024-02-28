using GR8Tech.TestUtils.RedisClient.Abstractions;
using GR8Tech.TestUtils.RedisClient.Common.Logging;
using GR8Tech.TestUtils.RedisClient.Settings;
using Newtonsoft.Json;
using NUnit.Framework;
using Serilog;
using Tests.SetUpFixtures;
using static Tests.GlobalSetUp;

namespace Tests.Tests;

public class RedisTests : TestBase
{
    private ILogger _logger = SerilogDecorator.Logger.ForContext<RedisTests>();

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void RedisClient_is_available_in_RedisClientFactory()
    {
        // Arrange
        var clientLocal = RedisClientFactory.GetClient("local");
        var clientStage = RedisClientFactory.GetClient("stage");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.True(clientLocal is not null, "Client should not be NULL");
            Assert.True(clientLocal.Name == "local", $"Client name is wrong, actual result {clientLocal.Name}");

            Assert.True(clientStage is not null, "Client should not be NULL");
            Assert.True(clientStage.Name == "stage", $"Client name is wrong, actual result {clientStage.Name}");
        });
    }

    [Test]
    public async Task Build_your_own_RedisClient_from_code()
    {
        // Arrange 
        var key = GetNewId();
        var value = GetNewId();

        // Act
        RedisClientFactory.Create("my-super-client",
            new RedisClientSettings
            {
                ConnectionString = "localhost:6379,allowAdmin=true,abortConnect=false"
            });

        var client = RedisClientFactory.GetClient("my-super-client");
        await client.AddAsync(0, key, value);

        // Assert
        var result = await client.GetAsync(0, key);

        Assert.True(result == value, $"Wrong value, actual {result}, expected {value}");
    }

    [Test]
    public async Task Add_string_to_redis_and_verify_string()
    {
        // Arrange
        var clientLocal = RedisClientFactory.GetClient("local");
        var key = GetNewId();
        var value = GetNewId();

        // Act
        await clientLocal.AddAsync(0, key, value);

        // Assert
        var result = await clientLocal.GetAsync(0, key);

        Assert.True(result == value, $"Wrong value, actual {result}, expected {value}");
    }

    public class TestClass
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }

    [Test]
    public async Task Add_object_to_redis_and_verify_object_with_default_serializer_deserializer()
    {
        // Arrange
        var clientLocal = RedisClientFactory.GetClient("local");

        var key = GetNewId();
        var myObject = new TestClass
        {
            Name = GetNewId(),
            Id = RandomInt()
        };

        // Act
        await clientLocal.AddAsync(0, key, myObject);

        // Assert
        var result = await clientLocal.GetAsync<TestClass>(0, key);

        Assert.True(result.Name == myObject.Name, $"Wrong value, actual {result.Name}, expected {myObject.Name}");
        Assert.True(result.Id == myObject.Id, $"Wrong value, actual {result.Id}, expected {myObject.Id}");
    }

    public class CustomSerializer : ISerializer
    {
        public int Checker { get; set; }

        public string Serialize(object obj)
        {
            Checker++;
            return JsonConvert.SerializeObject(obj);
        }
    }

    [Test]
    public async Task Add_object_to_redis_and_verify_object_with_custom_serializer_from_code()
    {
        // Arrange
        var customSerializer = new CustomSerializer();
        var clientLocal = RedisClientFactory.GetClient("local");

        var key = GetNewId();
        var myObject = new TestClass
        {
            Name = GetNewId(),
            Id = RandomInt()
        };

        // Act
        await clientLocal.AddAsync(0, key, myObject, customSerializer);

        // Assert
        var result = await clientLocal.GetAsync<TestClass>(0, key);

        Assert.True(result.Name == myObject.Name, $"Wrong value, actual {result.Name}, expected {myObject.Name}");
        Assert.True(result.Id == myObject.Id, $"Wrong value, actual {result.Id}, expected {myObject.Id}");
        Assert.True(customSerializer.Checker == 1, $"Wrong value, actual {customSerializer.Checker}, expected {1}");
    }

    public class CustomSerializerFromSettings : ISerializer
    {
        public static int Checker { get; set; }

        public string Serialize(object obj)
        {
            Checker++;
            return JsonConvert.SerializeObject(obj);
        }
    }

    [Test]
    public async Task Add_object_to_redis_and_verify_object_with_custom_serializer_from_settings()
    {
        // Arrange
        var client = RedisClientFactory.GetClient("serializer-test");

        var key = GetNewId();
        var myObject = new TestClass
        {
            Name = GetNewId(),
            Id = RandomInt()
        };

        // Act
        await client.AddAsync(0, key, myObject);

        // Assert
        var result = await client.GetAsync<TestClass>(0, key);

        Assert.True(result.Name == myObject.Name, $"Wrong value, actual {result.Name}, expected {myObject.Name}");
        Assert.True(result.Id == myObject.Id, $"Wrong value, actual {result.Id}, expected {myObject.Id}");
        Assert.True(CustomSerializerFromSettings.Checker == 1,
            $"Wrong value, actual {CustomSerializerFromSettings.Checker}, expected {1}");
    }

    public class CustomDeserializer : IDeserializer
    {
        public int Checker { get; set; }

        public T Deserialize<T>(string value)
        {
            Checker++;
            return JsonConvert.DeserializeObject<T>(value);
        }
    }

    [Test]
    public async Task Add_object_to_redis_and_verify_object_with_custom_deserializer_from_code()
    {
        // Arrange
        var customDeserializer = new CustomDeserializer();
        var clientLocal = RedisClientFactory.GetClient("local");

        var key = GetNewId();
        var myObject = new TestClass
        {
            Name = GetNewId(),
            Id = RandomInt()
        };

        // Act
        await clientLocal.AddAsync(0, key, myObject);
        var result = await clientLocal.GetAsync<TestClass>(0, key, customDeserializer);

        // Assert
        Assert.True(result.Name == myObject.Name, $"Wrong value, actual {result.Name}, expected {myObject.Name}");
        Assert.True(result.Id == myObject.Id, $"Wrong value, actual {result.Id}, expected {myObject.Id}");
        Assert.True(customDeserializer.Checker == 1, $"Wrong value, actual {customDeserializer.Checker}, expected {1}");
    }

    public class CustomDeserializerFromSettings : IDeserializer
    {
        public static int Checker { get; set; }

        public T Deserialize<T>(string value)
        {
            Checker++;
            return JsonConvert.DeserializeObject<T>(value);
        }
    }

    [Test]
    public async Task Add_object_to_redis_and_verify_object_with_custom_deserializer_from_settings()
    {
        // Arrange
        var client = RedisClientFactory.GetClient("deserializer-test");

        var key = GetNewId();
        var myObject = new TestClass
        {
            Name = GetNewId(),
            Id = RandomInt()
        };

        // Act
        await client.AddAsync(0, key, myObject);
        var result = await client.GetAsync<TestClass>(0, key);

        // Assert
        Assert.True(result.Name == myObject.Name, $"Wrong value, actual {result.Name}, expected {myObject.Name}");
        Assert.True(result.Id == myObject.Id, $"Wrong value, actual {result.Id}, expected {myObject.Id}");
        Assert.True(CustomDeserializerFromSettings.Checker == 1,
            $"Wrong value, actual {CustomDeserializerFromSettings.Checker}, expected {1}");
    }

    [Test]
    public async Task Add_string_hash_to_redis_and_verify_string_hash()
    {
        // Arrange
        var clientLocal = RedisClientFactory.GetClient("local");

        var key = GetNewId();
        var hashField = GetNewId();
        var hashValue = new TestClass
        {
            Name = GetNewId(),
            Id = RandomInt()
        };

        // Act
        await clientLocal.AddHashAsync(0, key, hashField, hashValue);
        var result = await clientLocal.GetHashAsync<TestClass>(0, key, hashField);

        // Assert
        Assert.True(result.Id == hashValue.Id, $"Wrong value, actual {result.Id}, expected {hashValue.Id}");
        Assert.True(result.Name == hashValue.Name, $"Wrong value, actual {result.Name}, expected {hashValue.Name}");
    }

    [Test]
    public async Task Add_object_hash_to_redis_and_verify_object_hash()
    {
        // Arrange
        var clientLocal = RedisClientFactory.GetClient("local");

        var key = GetNewId();
        var hashField = GetNewId();
        var hashValue = GetNewId();

        // Act
        await clientLocal.AddHashAsync(0, key, hashField, hashValue);
        var result = await clientLocal.GetHashAsync(0, key, hashField);

        // Assert
        Assert.True(result == hashValue, $"Wrong value, actual {result}, expected {hashValue}");
    }

    [Test]
    public async Task Add_string_to_redis_and_delete_string()
    {
        // Arrange
        var clientLocal = RedisClientFactory.GetClient("local");

        var key = GetNewId();
        var value = GetNewId();

        // Act
        await clientLocal.AddAsync(0, key, value);
        var result = await clientLocal.GetAsync(0, key);
        Assert.True(result == value, $"Wrong value, actual {result}, expected {value}");

        await clientLocal.DeleteKeyAsync(0, key);
        result = await clientLocal.GetAsync(0, key);

        // Assert
        Assert.True(string.IsNullOrEmpty(result), $"Wrong value, actual {result}, expected null");
    }

    [Test]
    public async Task Add_hash_to_redis_and_delete_hash()
    {
        // Arrange
        var clientLocal = RedisClientFactory.GetClient("local");

        var key = GetNewId();
        var hashField = GetNewId();
        var hashValue = GetNewId();

        // Act
        await clientLocal.AddHashAsync(0, key, hashField, hashValue);
        var result = await clientLocal.GetHashAsync(0, key, hashField);
        Assert.True(result == hashValue, $"Wrong value, actual {result}, expected {hashValue}");

        await clientLocal.DeleteHashAsync(0, key, hashField);
        result = await clientLocal.GetHashAsync(0, key, hashField);

        // Assert
        Assert.True(string.IsNullOrEmpty(result), $"Wrong value, actual {result}, expected null");
    }

    [Test]
    public async Task Clean_redis_database()
    {
        // Arrange
        var clientLocal = RedisClientFactory.GetClient("local");

        var key = GetNewId();
        var value = GetNewId();

        // Act
        await clientLocal.AddAsync(1, key, value);
        var result = await clientLocal.GetAsync(1, key);
        Assert.True(result == value, $"Wrong value, actual {result}, expected {value}");

        await clientLocal.CleanAsync(1);
        result = await clientLocal.GetAsync(1, key);

        // Assert
        Assert.True(string.IsNullOrEmpty(result), $"Wrong value, actual {result}, expected null");
    }

    [Test]
    public async Task Wait_for_key_in_redis_successfully()
    {
        // Arrange
        var clientLocal = RedisClientFactory.GetClient("local");

        var key = GetNewId();
        var value = GetNewId();

        // Act
        Task.Run(async () =>
        {
            await Task.Delay(3000);
            await clientLocal.AddAsync(0, key, value);
        });

        // Assert
        var result = await clientLocal.WaitAndGetAsync(
            async c => await c.GetAsync(0, key),
            r => r != null, 5);

        //OR
        result = await clientLocal.WaitAndGetAsync(
            c => c.GetAsync(0, key),
            r => r != null, 5);

        Assert.True(!string.IsNullOrEmpty(result), $"Wrong value, actual {result}, expected NOT null");
    }

    [Test]
    public async Task Wait_for_key_in_redis_unsuccessfully()
    {
        // Arrange
        var clientLocal = RedisClientFactory.GetClient("local");

        // Act
        var key = GetNewId();

        // Assert
        var result = await clientLocal.WaitAndGetAsync(
            c => c.GetAsync(0, key),
            r => r != null, 3);

        Assert.True(string.IsNullOrEmpty(result), $"Wrong value, actual {result}, expected null");
    }

    [Test]
    public async Task Wait_for_keys_in_redis_successfully()
    {
        // Arrange
        var clientLocal = RedisClientFactory.GetClient("local");

        var key1 = GetNewId();
        var key2 = GetNewId();
        var value = GetNewId();

        // Act
        Task.Run(async () =>
        {
            await Task.Delay(3000);
            await clientLocal.AddAsync(0, key1, value);
            await clientLocal.AddAsync(0, key2, value);
        });

        // Assert
        var result = await clientLocal.WaitAndGetAsync(
            async c => new List<string>
            {
                await clientLocal.GetAsync(0, key1),
                await clientLocal.GetAsync(0, key2)
            },
            r => r.All(x => x == value), 5);

        Assert.True(result.Count == 2, $"Wrong value, actual {result.Count()}, expected 2");
        Assert.True(result.All(x => x == value));
    }
}