using Shimmer.Core;

namespace Shimmer.Builders;

/// <summary>
///     The interface used to build and configure a tree of dependent jobs.
/// </summary>
/// <typeparam name="T">The type of the base job.</typeparam>
/// <typeparam name="TData">The type of the base job data.</typeparam>
public interface
    IShimmerJobTreeBuilder<T, TData> : IShimmerJobConfigurationBuilder<T, TData, IShimmerJobTreeBuilder<T, TData>>
    where T : ShimmerJob<TData> where TData : class
{
    public ShimmerJobTreeNode? CurrentNode { get; set; }

    /// <summary>
    ///     Fires the job asynchronously. If no schedule or start time is set, it will be processed immediately. It will fail
    ///     if no data is provided.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<DateTimeOffset> FireAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Adds a job that depends on the current job (and gets run after the current job).
    /// </summary>
    /// <param name="configure">The action to configure the new job.</param>
    /// <typeparam name="TNew">The type of the new job.</typeparam>
    /// <returns>The job tree builder</returns>
    IShimmerJobTreeBuilder<T, TData> AddDependentJob<TNew>(Action<IShimmerJobTreeBuilder<TNew>>? configure = null)
        where TNew : ShimmerJob;

    /// <summary>
    ///     Adds a job that depends on the current job (and gets run after the current job).
    /// </summary>
    /// <param name="configure">The action to configure the new job.</param>
    /// <typeparam name="TNew">The type of the new job.</typeparam>
    /// <typeparam name="TNewData">The type of the new job's data.</typeparam>
    /// <returns>The job tree builder</returns>
    IShimmerJobTreeBuilder<T, TData> AddDependentJob<TNew, TNewData>(
        Action<IShimmerJobTreeBuilder<TNew, TNewData>> configure)
        where TNew : ShimmerJob<TNewData> where TNewData : class;

    /// <summary>
    ///     Adds a job that depends on the current job (and gets run after the current job).
    /// </summary>
    /// <param name="jobBuilder">The already configured new job.</param>
    /// <typeparam name="TNew">The type of the new job.</typeparam>
    /// <returns>The job tree builder.</returns>
    IShimmerJobTreeBuilder<T, TData> AddDependentJob<TNew>(IShimmerJobTreeBuilder<TNew> jobBuilder)
        where TNew : ShimmerJob;

    /// <summary>
    ///     Adds a job that depends on the current job (and gets run after the current job).
    /// </summary>
    /// <param name="jobBuilder">The already configured new job.</param>
    /// <typeparam name="TNew">The type of the new job.</typeparam>
    /// <typeparam name="TNewData">The type of the new job's data.</typeparam>
    /// <returns>The job tree builder.</returns>
    IShimmerJobTreeBuilder<T, TData> AddDependentJob<TNew, TNewData>(IShimmerJobTreeBuilder<TNew, TNewData> jobBuilder)
        where TNew : ShimmerJob<TNewData> where TNewData : class;
}

public interface IShimmerJobTreeBuilder<T> : IShimmerJobConfigurationBuilder<T, IShimmerJobTreeBuilder<T>>
    where T : ShimmerJob
{
    public ShimmerJobTreeNode? CurrentNode { get; set; }

    /// <summary>
    ///     Fires the job asynchronously. If no schedule or start time is set, it will be processed immediately. It will fail
    ///     if no data is provided.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<DateTimeOffset> FireAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Adds a job that depends on the current job (and gets run after the current job).
    /// </summary>
    /// <param name="configure">The action to configure the new job.</param>
    /// <typeparam name="TNew">The type of the new job.</typeparam>
    /// <returns>The job tree builder</returns>
    IShimmerJobTreeBuilder<T> AddDependentJob<TNew>(Action<IShimmerJobTreeBuilder<TNew>>? configure = null)
        where TNew : ShimmerJob;

    /// <summary>
    ///     Adds a job that depends on the current job (and gets run after the current job).
    /// </summary>
    /// <param name="configure">The action to configure the new job.</param>
    /// <typeparam name="TNew">The type of the new job.</typeparam>
    /// <typeparam name="TNewData">The type of the new job's data.</typeparam>
    /// <returns>The job tree builder</returns>
    IShimmerJobTreeBuilder<T> AddDependentJob<TNew, TNewData>(Action<IShimmerJobTreeBuilder<TNew, TNewData>> configure)
        where TNew : ShimmerJob<TNewData> where TNewData : class;

    /// <summary>
    ///     Adds a job that depends on the current job (and gets run after the current job).
    /// </summary>
    /// <param name="jobBuilder">The already configured new job.</param>
    /// <typeparam name="TNew">The type of the new job.</typeparam>
    /// <returns>The job tree builder.</returns>
    IShimmerJobTreeBuilder<T> AddDependentJob<TNew>(IShimmerJobTreeBuilder<TNew> jobBuilder) where TNew : ShimmerJob;

    /// <summary>
    ///     Adds a job that depends on the current job (and gets run after the current job).
    /// </summary>
    /// <param name="jobBuilder">The already configured new job.</param>
    /// <typeparam name="TNew">The type of the new job.</typeparam>
    /// <typeparam name="TNewData">The type of the new job's data.</typeparam>
    /// <returns>The job tree builder.</returns>
    IShimmerJobTreeBuilder<T> AddDependentJob<TNew, TNewData>(IShimmerJobTreeBuilder<TNew, TNewData> jobBuilder)
        where TNew : ShimmerJob<TNewData> where TNewData : class;
}