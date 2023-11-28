using System.Text.Json;
using Quartz;
using Shimmer.Services;

namespace Shimmer.Core;

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

    protected abstract Task Process(T data, IJobExecutionContext context);
}

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

    protected abstract Task Process(IJobExecutionContext context);
}