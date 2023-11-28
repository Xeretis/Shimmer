using System.Text.Json;
using Quartz;
using Shimmer.Services;

namespace Shimmer.Core;

/// <summary>
///     The base class for all shimmer jobs. If used without DI, the <see cref="shimmerJobManager" /> property must be set
///     manually.
/// </summary>
/// <typeparam name="T">The type of the data used by this job.</typeparam>
public abstract class ShimmerJob<T> : IJob where T : class
{
    public IShimmerJobManager? shimmerJobManager;

    public async Task Execute(IJobExecutionContext context)
    {
        var data = JsonSerializer.Deserialize<T>(context.JobDetail.JobDataMap.GetString("data")!)!;

        await Process(data, context);

        if (shimmerJobManager!.JobTreeMap.TryGetValue(context.JobDetail.Key, out var jobTreeNode))
            foreach (var nextJobTreeNode in jobTreeNode)
            {
                var nextJobDetail = nextJobTreeNode.JobDetail;

                await context.Scheduler.ScheduleJob(nextJobDetail.Job, nextJobDetail.Trigger);
            }
    }

    /// <summary>
    ///     The method that gets called when the job is executed.
    /// </summary>
    /// <param name="data">The data passed when firing the job.</param>
    /// <param name="context">The Quartz job execution context.</param>
    protected abstract Task Process(T data, IJobExecutionContext context);
}

/// <summary>
///     The base class for all shimmer jobs. If used without DI, the <see cref="shimmerJobManager" /> property must be set
///     manually.
/// </summary>
public abstract class ShimmerJob : IJob
{
    public IShimmerJobManager? shimmerJobManager;

    public async Task Execute(IJobExecutionContext context)
    {
        await Process(context);

        if (shimmerJobManager!.JobTreeMap.TryGetValue(context.JobDetail.Key, out var jobTreeNode))
            foreach (var nextJobTreeNode in jobTreeNode)
            {
                var nextJobDetail = nextJobTreeNode.JobDetail;

                await context.Scheduler.ScheduleJob(nextJobDetail.Job, nextJobDetail.Trigger);
            }
    }

    /// <summary>
    ///     The method that gets called when the job is executed.
    /// </summary>
    /// <param name="context">The Quartz job execution context.</param>
    protected abstract Task Process(IJobExecutionContext context);
}