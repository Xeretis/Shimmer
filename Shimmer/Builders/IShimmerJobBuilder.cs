using Shimmer.Core;

namespace Shimmer.Builders;

public interface IShimmerJobBuilder<T, TData> : IShimmerJobConfigurationBuilder<T, TData, IShimmerJobBuilder<T, TData>>
    where T : ShimmerJob<TData> where TData : class
{
    Task<DateTimeOffset> FireAsync(CancellationToken cancellationToken = default);
}

public interface IShimmerJobBuilder<T> : IShimmerJobConfigurationBuilder<T, IShimmerJobBuilder<T>> where T : ShimmerJob
{
    Task<DateTimeOffset> FireAsync(CancellationToken cancellationToken = default);
}