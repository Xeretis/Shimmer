using Shimmer.Core;

namespace Shimmer.Builders;

public interface
    IShimmerJobTreeBuilder<T, TData> : IShimmerJobConfigurationBuilder<T, TData, IShimmerJobTreeBuilder<T, TData>>
    where T : ShimmerJob<TData> where TData : class
{
    public ShimmerJobTreeNode? CurrentNode { get; set; }

    Task<DateTimeOffset> FireAsync(CancellationToken cancellationToken = default);

    IShimmerJobTreeBuilder<T, TData> AddChainedJob<TNew>(Action<IShimmerJobTreeBuilder<TNew>>? configure = null)
        where TNew : ShimmerJob;

    IShimmerJobTreeBuilder<T, TData> AddChainedJob<TNew, TNewData>(
        Action<IShimmerJobTreeBuilder<TNew, TNewData>> configure)
        where TNew : ShimmerJob<TNewData> where TNewData : class;

    IShimmerJobTreeBuilder<T, TData> AddChainedJob<TNew>(IShimmerJobTreeBuilder<TNew> job) where TNew : ShimmerJob;

    IShimmerJobTreeBuilder<T, TData> AddChainedJob<TNew, TNewData>(IShimmerJobTreeBuilder<TNew, TNewData> job)
        where TNew : ShimmerJob<TNewData> where TNewData : class;
}

public interface IShimmerJobTreeBuilder<T> : IShimmerJobConfigurationBuilder<T, IShimmerJobTreeBuilder<T>>
    where T : ShimmerJob
{
    public ShimmerJobTreeNode? CurrentNode { get; set; }

    Task<DateTimeOffset> FireAsync(CancellationToken cancellationToken = default);

    IShimmerJobTreeBuilder<T> AddDependentJob<TNew>(Action<IShimmerJobTreeBuilder<TNew>>? configure = null)
        where TNew : ShimmerJob;

    IShimmerJobTreeBuilder<T> AddDependentJob<TNew, TNewData>(Action<IShimmerJobTreeBuilder<TNew, TNewData>> configure)
        where TNew : ShimmerJob<TNewData> where TNewData : class;

    IShimmerJobTreeBuilder<T> AddDependentJob<TNew>(IShimmerJobTreeBuilder<TNew> jobBuilder) where TNew : ShimmerJob;

    IShimmerJobTreeBuilder<T> AddDependentJob<TNew, TNewData>(IShimmerJobTreeBuilder<TNew, TNewData> jobBuilder)
        where TNew : ShimmerJob<TNewData> where TNewData : class;
}