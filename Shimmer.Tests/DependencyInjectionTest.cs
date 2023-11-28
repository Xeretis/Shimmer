using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Shimmer.Core;

namespace Shimmer.Tests;

public class DITestData1
{
}

public class DITestData2
{
}

public class DITestJob1 : ShimmerJob<DITestData1>
{
    protected override Task Process(DITestData1 data, IJobExecutionContext context)
    {
        throw new NotImplementedException();
    }
}

public class DITestJob2 : ShimmerJob<DITestData2>
{
    protected override Task Process(DITestData2 data, IJobExecutionContext context)
    {
        throw new NotImplementedException();
    }
}

public interface IDITestService
{
}

public class DITestService : IDITestService
{
}

internal class DITestJob3(IDITestService idiTestService) : ShimmerJob<DITestData1>
{
    public readonly IDITestService DITestService = idiTestService;

    protected override Task Process(DITestData1 data, IJobExecutionContext context)
    {
        throw new NotImplementedException();
    }
}

internal class DITestJob4 : ShimmerJob
{
    protected override Task Process(IJobExecutionContext context)
    {
        throw new NotImplementedException();
    }
}

public class DependencyInjectionTest
{
    [Fact]
    public void DiscoversJobs()
    {
        var services = new ServiceCollection();

        services.AddShimmer().DiscoverJobs(Assembly.GetAssembly(typeof(DITestJob1))!);
        services.AddTransient<IDITestService, DITestService>();

        var provider = services.BuildServiceProvider();

        provider.GetRequiredService<DITestJob1>().Should().BeOfType<DITestJob1>();
        provider.GetRequiredService<DITestJob2>().Should().BeOfType<DITestJob2>();
        provider.GetRequiredService<DITestJob3>().Should().BeOfType<DITestJob3>();
        provider.GetRequiredService<DITestJob4>().Should().BeOfType<DITestJob4>();
    }

    [Fact]
    public void DiscoversJobsWithLifetime()
    {
        var services = new ServiceCollection();

        services.AddShimmer().DiscoverJobs(Assembly.GetAssembly(typeof(DITestJob1))!, ServiceLifetime.Singleton);

        using var descriptors = services.GetEnumerator();

        while (descriptors.MoveNext())
        {
            var currentDescriptor = descriptors.Current;

            if (currentDescriptor.ServiceType == typeof(DITestJob1))
                currentDescriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
            else if (currentDescriptor.ServiceType == typeof(DITestJob2))
                currentDescriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
            else if (currentDescriptor.ServiceType == typeof(DITestJob3))
                currentDescriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
            else if (currentDescriptor.ServiceType == typeof(DITestJob4))
                currentDescriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
        }
    }

    [Fact]
    public void InjectsJobDependencies()
    {
        var services = new ServiceCollection();

        services.AddShimmer().DiscoverJobs(Assembly.GetAssembly(typeof(DITestJob1))!);
        services.AddTransient<IDITestService, DITestService>();

        var provider = services.BuildServiceProvider();

        var job1 = provider.GetRequiredService<DITestJob1>();

        job1.shimmerJobManager.Should().NotBeNull();

        var job3 = provider.GetRequiredService<DITestJob3>();

        job3.shimmerJobManager.Should().NotBeNull();
        job3.DITestService.Should().NotBeNull();

        var job4 = provider.GetRequiredService<DITestJob4>();

        job4.shimmerJobManager.Should().NotBeNull();
    }
}