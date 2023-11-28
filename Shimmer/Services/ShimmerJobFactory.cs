using Quartz;
using Shimmer.Builders;
using Shimmer.Core;

namespace Shimmer.Services;

public class ShimmerJobFactory(IShimmerJobManager jobManager, ISchedulerFactory schedulerFactory) : IShimmerJobFactory
{
    public async Task<IShimmerJobBuilder<T>> CreateAsync<T>(CancellationToken cancellationToken = default)
        where T : ShimmerJob
    {
        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        var shimmerJobBuilder = new ShimmerJobBuilder<T>(scheduler);

        return shimmerJobBuilder;
    }

    public async Task<IShimmerJobBuilder<T, TData>> CreateAsync<T, TData>(CancellationToken cancellationToken = default)
        where T : ShimmerJob<TData> where TData : class
    {
        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        var shimmerJobBuilder = new ShimmerJobBuilder<T, TData>(scheduler);

        return shimmerJobBuilder;
    }

    public async Task<IShimmerJobTreeBuilder<T>> CreateTreeAsync<T>(CancellationToken cancellationToken = default)
        where T : ShimmerJob
    {
        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        var shimmerJobChainBuilder = new ShimmerJobTreeBuilder<T>(jobManager, scheduler);

        return shimmerJobChainBuilder;
    }

    public async Task<IShimmerJobTreeBuilder<T, TData>> CreateTreeAsync<T, TData>(
        CancellationToken cancellationToken = default)
        where T : ShimmerJob<TData> where TData : class
    {
        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        var shimmerJobChainBuilder = new ShimmerJobTreeBuilder<T, TData>(jobManager, scheduler);

        return shimmerJobChainBuilder;
    }
}