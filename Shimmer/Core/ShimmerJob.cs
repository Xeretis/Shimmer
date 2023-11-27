using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Shimmer.Managers;

namespace Shimmer.Core;

public abstract class ShimmerJob<T> : IJob where T : class
{
    public IShimmerJobManager shimmerJobManager;
    public ISchedulerFactory schedulerFactory;

    public Task Execute(IJobExecutionContext context)
    {
        throw new NotImplementedException();
    }

    protected abstract Task Process(T data, IJobExecutionContext context);

    // public async Task RunAsync(T data, CancellationToken cancellationToken = default)
    // {
    //     var job = JobBuilder.Create(GetType())
    //         .UsingJobData("data", JsonSerializer.Serialize(data))
    //         .Build();
    //
    //     var trigger = TriggerBuilder.Create().StartNow().Build();
    //     
    //     var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
    //     
    //     await scheduler.ScheduleJob(job, trigger, cancellationToken);
    // }
}