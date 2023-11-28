namespace Shimmer.Core;

/// <summary>
///     The class representing a node on a job tree.
/// </summary>
public class ShimmerJobTreeNode : HashSet<ShimmerJobTreeNode>
{
    public ShimmerJobDetail JobDetail { get; set; }
}