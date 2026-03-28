namespace MotorDsl.Core.Models;

/// <summary>
/// Represents a loop node for iterating over collections.
/// Generates multiple copies of content for each item in a collection.
/// 
/// Source: modelo-datos-logico_v1.0.md (Section 9)
/// </summary>
public class LoopNode : DocumentNode
{
    /// <summary>
    /// Gets or sets the path to the collection to iterate over.
    /// Must resolve to an enumerable list at runtime.
    /// Example: "items", "orders[0].details"
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// Gets or sets the alias name for each item in the iteration.
    /// Used in data binding expressions within the body.
    /// Example: "item", "order"
    /// </summary>
    public string ItemAlias { get; set; }

    /// <summary>
    /// Gets or sets the content node to repeat for each collection item.
    /// </summary>
    public DocumentNode? Body { get; set; }

    /// <summary>
    /// Constructor for loop nodes.
    /// </summary>
    /// <param name="source">Path to collection</param>
    /// <param name="itemAlias">Name for each item</param>
    /// <param name="body">Content to repeat</param>
    public LoopNode(string source, string itemAlias, DocumentNode? body = null)
        : base("loop")
    {
        Source = source;
        ItemAlias = itemAlias;
        Body = body;
    }
}
