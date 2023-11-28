using System.Collections.Concurrent;
using Quartz;
using Shimmer.Core;

namespace Shimmer.Services;

/// <summary>
///     The class used to manage shimmer job trees.
/// </summary>
public class ShimmerJobManager : IShimmerJobManager
{
    public ConcurrentDictionary<JobKey, ShimmerJobTreeNode> JobTreeMap { get; init; } = new();
}