using Shimmer.Builders;
using Shimmer.Core;

namespace Shimmer.Services;

/// <summary>
///     The interface used to create shimmer jobs.
/// </summary>
public interface IShimmerJobFactory
{
    /// <summary>
    ///     Creates a new shimmer job builder.
    /// </summary>
    /// <param name="cancellationToken">The cancellation job.</param>
    /// <typeparam name="T">The type of the job.</typeparam>
    /// <returns>The job builder.</returns>
    public Task<IShimmerJobBuilder<T>> CreateAsync<T>(CancellationToken cancellationToken = default)
        where T : ShimmerJob;

    /// <summary>
    ///     Creates a new shimmer job builder.
    /// </summary>
    /// <param name="cancellationToken">The cancellation job.</param>
    /// <typeparam name="T">The type of the job.</typeparam>
    /// <typeparam name="TData">The type of the job data.</typeparam>
    /// <returns>The job builder.</returns>
    public Task<IShimmerJobBuilder<T, TData>> CreateAsync<T, TData>(CancellationToken cancellationToken = default)
        where T : ShimmerJob<TData> where TData : class;

    /// <summary>
    ///     Creates a new shimmer job tree builder.
    /// </summary>
    /// <param name="cancellationToken">The cancellation job.</param>
    /// <typeparam name="T">The type of the job.</typeparam>
    /// <returns>The job tree builder.</returns>
    public Task<IShimmerJobTreeBuilder<T>> CreateTreeAsync<T>(CancellationToken cancellationToken = default)
        where T : ShimmerJob;

    /// <summary>
    ///     Creates a new shimmer job tree builder.
    /// </summary>
    /// <param name="cancellationToken">The cancellation job.</param>
    /// <typeparam name="T">The type of the job.</typeparam>
    /// <typeparam name="TData">The type of the job data.</typeparam>
    /// <returns>The job tree builder.</returns>
    public Task<IShimmerJobTreeBuilder<T, TData>> CreateTreeAsync<T, TData>(
        CancellationToken cancellationToken = default)
        where T : ShimmerJob<TData> where TData : class;
}