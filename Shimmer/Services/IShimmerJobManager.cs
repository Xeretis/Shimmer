using System.Collections.Concurrent;
using Quartz;
using Shimmer.Core;

namespace Shimmer.Services;

public interface IShimmerJobManager
{
    public ConcurrentDictionary<JobKey, ShimmerJobTreeNode> JobTreeMap { init; get; }
}