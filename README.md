![Shimmer Logo](.github/ShimmerLogo.jpg)

# Shimmer

![GitHub License](https://img.shields.io/github/license/Xeretis/Shimmer) ![Nuget](https://img.shields.io/nuget/v/Xeretis.Shimmer)

> Shimmer is a simple and lightweight abstarction over [Quartz.NET](https://www.quartz-scheduler.net/) that makes creating jobs and job trees clean and simple.

## Installation

Shimmer is available as a [NuGet package](https://www.nuget.org/packages/Xeretis.Shimmer/).

```bash
dotnet add package Xeretis.Shimmer
```

We also provide a package for [ASP.NET Core](https://www.nuget.org/packages/Xeretis.Shimmer.AspNetCore/) to use shimmer with a Quartz hosted service.

```bash 
dotnet add package Xeretis.Shimmer.AspNetCore
```

## Usage

### Creating simple jobs

Shimmer provides a base class `ShimmerJob` that you can inherit from to create your own jobs that require no data.

```csharp
public class SampleJob : ShimmerJob
{
    protected override Task Process(IJobExecutionContext context)
    {
        // Do something here
    }
}
```

### Creating jobs with data

Shimmer also provides a base class `ShimmerJob<T>` that you can inherit from to create your own jobs that require data.

```csharp
public class SampleJobWithData : ShimmerJob<SampleData>
{
    protected override Task Process(SampleData data, IJobExecutionContext context)
    {
        // Do something here
    }
}
```

### Registering jobs

There are two ways to register jobs with Shimmer. The first way is to register them manually.

```csharp
services.AddShimmer(); // Adds both Quartz and Shimmer services 
services.AddShimmerJob<SampleJob>(ServiceLifetime.Singleton); // Registers SampleJob as a singleton (by default, jobs are registered as scoped)
services.AddShimmerJob<SampleJobWithData, SampleData>();
```

The second way is to register them automatically by scanning assemblies.

```csharp
services.AddShimmer(); // Adds both Quartz and Shimmer services
services.DiscoverJobs(Assembly.GetExecutingAssembly()); // Discovers jobs in the executing assembly and adds them to the service collection
```

### Using Shimmer with ASP.NET Core

Shimmer provides an extension method for `IServiceCollection` that allows you to register Shimmer with the Quartz hosted service.

```csharp
services.AddShimmerHostedService();
```

### Using Jobs

#### Using independent jobs

If you only want to run or schedule a job that does not depend on any other jobs, you can use `IShimmerJobFactory.CreateAsync`.

```csharp
public class SampleController : Controller
{
    private readonly IShimmerJobFactory _jobFactory;

    public SampleController(IShimmerJobFactory jobFactory)
    {
        _jobFactory = jobFactory;
    }

    public async Task<IActionResult> RunJob()
    {
        // Create job
        var job = await _jobFactory.CreateAsync<SampleJob>();

        // Configure job
        job.Name("Sample Job")
           .Description("This is a sample job")
           .CronSchedule("0 0 12 * * ?");
        
        // Run job in the background
        await job.FireAsync();

        return Ok();
    }
}
```

The same can be done with jobs that require data.

```csharp
public class SampleController : Controller
{
    private readonly IShimmerJobFactory _jobFactory;

    public SampleController(IShimmerJobFactory jobFactory)
    {
        _jobFactory = jobFactory;
    }

    public async Task<IActionResult> RunJob()
    {
        // Create job
        var job = await _jobFactory.CreateAsync<SampleJobWithData, string>();

        // Configure job
        job.Name("Sample Job")
           .Description("This is a sample job")
           .CronSchedule("0 0 12 * * ?")
           .Data(new SampleData()); // Very important! If you don't provide data, running the job will throw an exception.
        
        // Run job in the background
        await job.FireAsync();

        return Ok();
    }
}
```

#### Using job trees

If you want to run or schedule a job that depends on other jobs, you can use `IShimmerJobFactory.CreateTreeAsync`.

```csharp
public class SampleController : Controller
{
    private readonly IShimmerJobFactory _jobFactory;

    public SampleController(IShimmerJobFactory jobFactory)
    {
        _jobFactory = jobFactory;
    }

    public async Task<IActionResult> RunJob()
    {
        // Create job tree
        var jobTree = await _jobFactory.CreateTreeAsync<SampleJob>();

        // Configure job tree
        jobTree.Name("Sample Job Tree")
                .Description("This is a sample job tree")
                .CronSchedule("0 0 12 * * ?")
                .AddDependentJob<SampleJob2>(); // SampleJob2 will run after SampleJob finishes (every time)

        // Run job tree in the background
        await jobTree.FireAsync();

        return Ok();
    }
}
```

It's also possible to configure dependent jobs either by passing an action or an already configured job.

```csharp
public class SampleController : Controller
{
    private readonly IShimmerJobFactory _jobFactory;

    public SampleController(IShimmerJobFactory jobFactory)
    {
        _jobFactory = jobFactory;
    }

    public async Task<IActionResult> RunJob()
    {
        // Create job tree
        var jobTree = await _jobFactory.CreateTreeAsync<SampleJob>();

        // Configure job tree
        jobTree.Name("Sample Job Tree")
                .Description("This is a sample job tree")
                .CronSchedule("0 0 12 * * ?")
                .AddDependentJob<SampleJob2>() // SampleJob2 will run after SampleJob finishes (every time)
                .AddDependentJob<SampleJobWithData, SampleData>(j => j.Data(new SampleData())) // SampleJobWithData will run after SampleJob finishes (every time), here we must provide an action to configure the job data
                .AddDependentJob<SampleJob3>(j => j.AddDependentJob<SampleJob4>()); // We can also add more jobs to the tree with an action
                
        // Run job tree in the background
        await jobTree.FireAsync();
    }
}
```

When we have a deeply nested job tree, it might be more advantageous to just pass a configured job instead of an action.

```csharp
public class SampleController : Controller
{
    private readonly IShimmerJobFactory _jobFactory;

    public SampleController(IShimmerJobFactory jobFactory)
    {
        _jobFactory = jobFactory;
    }

    public async Task<IActionResult> RunJob()
    {
        // Create job tree
        var jobTree = await _jobFactory.CreateTreeAsync<SampleJob>();

        var dependentJob = await _jobFactory.CreateTreeAsync<SampleJob2>(); // We must create this as a job tree as well
        
        dependentJob.Name("Sample Job 2")
                    .Description("This is a sample job")
                    .StartAt(DateTimeOffset.UtcNow.AddMinutes(5));
        
        // Configure job tree
        jobTree.Name("Sample Job Tree")
                .Description("This is a sample job tree")
                .AddDependentJob(dependentJob); // SampleJob2 will run 5 minutes after the current time after SampleJob finishes
                
        // Run job tree in the background
        await jobTree.FireAsync();
    }
}
```

And... that's it! It really is that simple. If you want to learn more about the configuration options, check out `IShimmerJobConfigurationBuilder`.

## Small side note

This project was never meant to replace or completely abstract away Quartz.NET. It was created to make it easier to use Quartz.NET with **simple jobs** (and simple applications). If you need more control over your jobs, you can always use Quartz.NET directly (with your Shimmer jobs too).