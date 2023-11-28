using Quartz;
using Shimmer.Builders;
using Shimmer.Core;

namespace Shimmer.Services;

/// <summary>
///     The class used to create shimmer jobs.
/// </summary>
/// <param name="jobManager">The job manager (same as the one in the jobs).</param>
/// <param name="schedulerFactory">The Quartz scheduler factory.</param>
public class ShimmerJobFactory(IShimmerJobManager jobManager, ISchedulerFactory schedulerFactory) : IShimmerJobFactory
{
    /// <inheritdoc />
    public async Task<IShimmerJobBuilder<T>> CreateAsync<T>(CancellationToken cancellationToken = default)
        where T : ShimmerJob
    {
        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        var shimmerJobBuilder = new ShimmerJobBuilder<T>(scheduler);

        return shimmerJobBuilder;
    }

    /// <inheritdoc />
    public async Task<IShimmerJobBuilder<T, TData>> CreateAsync<T, TData>(CancellationToken cancellationToken = default)
        where T : ShimmerJob<TData> where TData : class
    {
        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        var shimmerJobBuilder = new ShimmerJobBuilder<T, TData>(scheduler);

        return shimmerJobBuilder;
    }

    /// <inheritdoc />
    public async Task<IShimmerJobTreeBuilder<T>> CreateTreeAsync<T>(CancellationToken cancellationToken = default)
        where T : ShimmerJob
    {
        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        var shimmerJobChainBuilder = new ShimmerJobTreeBuilder<T>(jobManager, scheduler);

        return shimmerJobChainBuilder;
    }

    /// <inheritdoc />
    public async Task<IShimmerJobTreeBuilder<T, TData>> CreateTreeAsync<T, TData>(
        CancellationToken cancellationToken = default)
        where T : ShimmerJob<TData> where TData : class
    {
        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        var shimmerJobChainBuilder = new ShimmerJobTreeBuilder<T, TData>(jobManager, scheduler);

        return shimmerJobChainBuilder;
    }
}