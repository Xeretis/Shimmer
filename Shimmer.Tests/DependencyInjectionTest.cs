using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Shimmer.Core;

namespace Shimmer.Tests;

class TestData1 {}

class TestData2 {}

class TestJob1 : ShimmerJob<TestData1>
{
    protected override Task Process(TestData1 data, IJobExecutionContext context)
    {
        throw new NotImplementedException();
    }
}

class TestJob2 : ShimmerJob<TestData2>
{
    protected override Task Process(TestData2 data, IJobExecutionContext context)
    {
        throw new NotImplementedException();
    }
}

interface ITestService
{
    
}

class TestService : ITestService
{
    
}

class TestJob3(ITestService testService) : ShimmerJob<TestData1>
{
    public readonly ITestService testService = testService;

    protected override Task Process(TestData1 data, IJobExecutionContext context)
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
        
        services.AddShimmer().DiscoverJobs(Assembly.GetAssembly(typeof(TestJob1))!);
        services.AddTransient<ITestService, TestService>();
        
        var provider = services.BuildServiceProvider();

        provider.GetRequiredService<TestJob1>().Should().BeOfType<TestJob1>();
        provider.GetRequiredService<TestJob2>().Should().BeOfType<TestJob2>();
        provider.GetRequiredService<TestJob3>().Should().BeOfType<TestJob3>();
    }
    
    [Fact]
    public void DiscoversJobsWithLifetime()
    {
        var services = new ServiceCollection();
        
        services.AddShimmer().DiscoverJobs(Assembly.GetAssembly(typeof(TestJob1))!, ServiceLifetime.Singleton);

        var descriptors = services.GetEnumerator();
        
        while (descriptors.MoveNext())
        {
            var currentDescriptor = descriptors.Current;
            
            if (currentDescriptor.ServiceType == typeof(TestJob1))
                currentDescriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
            else if (currentDescriptor.ServiceType == typeof(TestJob2))
                currentDescriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
            else if (currentDescriptor.ServiceType == typeof(TestJob3))
                currentDescriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
        }
    }
    
    [Fact]
    public void InjectsJobDependencies()
    {
        var services = new ServiceCollection();
        
        services.AddShimmer().DiscoverJobs(Assembly.GetAssembly(typeof(TestJob1))!);
        services.AddTransient<ITestService, TestService>();
        
        var provider = services.BuildServiceProvider();

        var job1 = provider.GetRequiredService<TestJob1>();

        job1.shimmerJobManager.Should().NotBeNull();
        job1.schedulerFactory.Should().NotBeNull();
        
        var job3 = provider.GetRequiredService<TestJob3>();
        
        job3.shimmerJobManager.Should().NotBeNull();
        job3.schedulerFactory.Should().NotBeNull();
        job3.testService.Should().NotBeNull();
    }
}