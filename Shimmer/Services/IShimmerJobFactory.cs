using Shimmer.Builders;
using Shimmer.Core;

namespace Shimmer.Services;

public interface IShimmerJobFactory
{
    public Task<IShimmerJobBuilder<T>> CreateAsync<T>(CancellationToken cancellationToken = default)
        where T : ShimmerJob;

    public Task<IShimmerJobBuilder<T, TData>> CreateAsync<T, TData>(CancellationToken cancellationToken = default)
        where T : ShimmerJob<TData> where TData : class;

    public Task<IShimmerJobTreeBuilder<T>> CreateTreeAsync<T>(CancellationToken cancellationToken = default)
        where T : ShimmerJob;

    public Task<IShimmerJobTreeBuilder<T, TData>> CreateTreeAsync<T, TData>(
        CancellationToken cancellationToken = default)
        where T : ShimmerJob<TData> where TData : class;
}