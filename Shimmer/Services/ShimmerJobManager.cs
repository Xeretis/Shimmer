using System.Collections.Concurrent;
using Quartz;
using Shimmer.Core;

namespace Shimmer.Services;

public class ShimmerJobManager : IShimmerJobManager
{
    public ConcurrentDictionary<JobKey, ShimmerJobTreeNode> JobTreeMap { get; init; } = new();
}