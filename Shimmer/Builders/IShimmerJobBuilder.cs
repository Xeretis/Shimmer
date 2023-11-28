using Shimmer.Core;

namespace Shimmer.Builders;

/// <summary>
///     The interface used to build and configure an independent job.
/// </summary>
/// <typeparam name="T">The type of the job.</typeparam>
/// <typeparam name="TData">The type of the job data.</typeparam>
public interface IShimmerJobBuilder<T, TData> : IShimmerJobConfigurationBuilder<T, TData, IShimmerJobBuilder<T, TData>>
    where T : ShimmerJob<TData> where TData : class
{
    /// <summary>
    ///     Fires the job asynchronously. If no schedule or start time is set, it will be processed immediately. It will fail
    ///     if no data is provided.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<DateTimeOffset> FireAsync(CancellationToken cancellationToken = default);
}

/// <summary>
///     The interface used to build and configure an independent job.
/// </summary>
/// <typeparam name="T">The type of the job.</typeparam>
public interface IShimmerJobBuilder<T> : IShimmerJobConfigurationBuilder<T, IShimmerJobBuilder<T>> where T : ShimmerJob
{
    /// <summary>
    ///     Fires the job asynchronously. If no schedule or start time is set, it will be processed immediately.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<DateTimeOffset> FireAsync(CancellationToken cancellationToken = default);
}