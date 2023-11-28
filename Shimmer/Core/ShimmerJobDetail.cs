using Quartz;

namespace Shimmer.Core;

public class ShimmerJobDetail
{
    public IJobDetail Job { get; set; }
    public ITrigger Trigger { get; set; }
}