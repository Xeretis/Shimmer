namespace Shimmer.Core;

public class ShimmerJobTreeNode : HashSet<ShimmerJobTreeNode>
{
    public ShimmerJobDetail JobDetail { get; set; }
}