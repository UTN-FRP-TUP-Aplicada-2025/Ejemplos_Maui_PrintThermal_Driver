namespace MotorDsl.Core.Models;

/// <summary>
/// Represents a container node that groups other nodes.
/// Used for structural organization with layout rules.
/// 
/// Source: modelo-datos-logico_v1.0.md (Section 7)
/// </summary>
public class ContainerNode : DocumentNode
{
    /// <summary>
    /// Gets or sets the layout type for child arrangement.
    /// Common values: "vertical", "horizontal"
    /// </summary>
    public string? Layout { get; set; }

    /// <summary>
    /// Constructor for container nodes.
    /// </summary>
    /// <param name="layout">Layout type (e.g., "vertical", "horizontal")</param>
    public ContainerNode(string? layout = "vertical") : base("container")
    {
        Layout = layout;
    }

    /// <summary>
    /// Constructor for container nodes with children.
    /// </summary>
    /// <param name="layout">Layout type (e.g., "vertical", "horizontal")</param>
    /// <param name="children">Child nodes</param>
    public ContainerNode(string? layout, List<DocumentNode> children) : base("container")
    {
        Layout = layout;
        Children = children;
    }
}
