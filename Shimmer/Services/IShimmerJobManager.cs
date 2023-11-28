using System.Collections.Concurrent;
using Quartz;
using Shimmer.Core;

namespace Shimmer.Services;

/// <summary>
///     The interface used to manage shimmer job trees.
/// </summary>
public interface IShimmerJobManager
{
    public ConcurrentDictionary<JobKey, ShimmerJobTreeNode> JobTreeMap { init; get; }
}