using Quartz;

namespace Shimmer.Core;

/// <summary>
///     The class representing a job that will be fired by Shimmer.
/// </summary>
public class ShimmerJobDetail
{
    public IJobDetail Job { get; set; }
    public ITrigger Trigger { get; set; }
}