# GR8Tech.TestUtils.RedisClient

Authors: Mykola Panasiuk

## TL;DR
This repository contains primary the source code for building NuGet package to Open Source:
- GR8Tech.TestUtils.RedisClient

Client helps to access Redis server and interact with it.

## Settings file

Settings file might be named either `test-settings.json` or `appsettings.json`. It requires only to set at least one RedisClient with any name you want. The simplest example below: 

```json
{
  "RedisSettings": {
    "RedisClients": {
      "local": {
        "ConnectionString": "localhost:6379,allowAdmin=true,abortConnect=false"
      }
    }
  }
}
```

Where `RedisClients` is a Dictionary, you may add as many named clients as you want.
The full settings example below:

```json
{
  "RedisSettings": {
    "DefaultSettings": {
      "WaiterSettings": {
        "RetryCount": 10
      },
      "SerializerTypeString": "Tests.Tests.RedisTests+CustomSerializerFromSettings, Tests",
      "DeserializerTypeString": "Tests.Tests.RedisTests+CustomDeserializerFromSettings, Tests"
    },
    "RedisClients": {
      "local": {
        "ConnectionString": "localhost:6379,allowAdmin=true,abortConnect=false",
        "WaiterSettings": {
          "RetryCount": 10,
          "Interval": "00:00:01"
        }
      },
      "stage": {
        "ConnectionString": "localhost:6379,allowAdmin=true,abortConnect=false",
        "SerializerTypeString": "Tests.Tests.RedisTests+NewCustomSerializer, Tests"
      }
    }
  }
}
```

Where:
- `DefaultSettings` - you may define default settings for all Redis clients here
  - `WaiterSettings` - settings for waiter with two parameters `RetryCount` and `Interval`
  - `SerializerTypeString` - your specific serializer type
  - `DeserializerTypeString` - your specific deserializer type
- two named RedisClients - `local` and `stage`
  - `local` client has their own more specific settings for waiter
  - while `stage` client uses different serializer

**Note**: in code you have a control over waiter, serializer and deserializer 

## Code examples

### Simple usage

Add value to Redis
```c#
var redisClientsFactory = new RedisClientFactory();
var client = redisClientsFactory.GetClient("local");
var key = "my_key";
var value = "my_value";

await client.AddAsync(0, key, value);
```

Get value from Redis
```c#
var redisClientsFactory = new RedisClientFactory();
var client = redisClientsFactory.GetClient("local");
var key = "my_key";

var value = await client.GetAsync(0, key);
```

### Wait and get

```c#
var redisClientsFactory = new RedisClientFactory();
var client = redisClientsFactory.GetClient("local");
var key = "my_key";
var value = "my_value";

Task.Run(async () =>
{
    await Task.Delay(3000);
    await clientLocal.AddAsync(0, key, value);
});

var result = await client.WaitAndGetAsync(
    async c => await c.GetAsync(0, key),
    r => r != null);
```


