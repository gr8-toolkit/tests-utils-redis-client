using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;

namespace Tests.SetUpFixtures;

public class DockerFixture : IDisposable
{
    private readonly ICompositeService _containers;

    public DockerFixture()
    {
        _containers = new Builder()
            .UseContainer()
            .UseImage("redis:latest")
            .WithHostName("redis")
            .WithName("redis")
            .ExposePort(6379, 6379)
            .WaitForPort("6379/tcp", 30000 /*30s*/)
            .Builder()
            .Build()
            .Start();
    }

    public void Dispose()
    {
        _containers.Dispose();
    }
}