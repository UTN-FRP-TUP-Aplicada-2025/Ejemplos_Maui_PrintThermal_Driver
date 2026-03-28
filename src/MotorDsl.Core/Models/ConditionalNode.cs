namespace MotorDsl.Core.Models;

/// <summary>
/// Represents a conditional node for runtime logic.
/// Allows selective inclusion of content based on expressions.
/// 
/// Source: modelo-datos-logico_v1.0.md (Section 8)
/// </summary>
public class ConditionalNode : DocumentNode
{
    /// <summary>
    /// Gets or sets the condition expression to evaluate.
    /// Expression will be evaluated at runtime against document data.
    /// </summary>
    public string Expression { get; set; }

    /// <summary>
    /// Gets or sets the node to render if expression evaluates to true.
    /// </summary>
    public DocumentNode? TrueBranch { get; set; }

    /// <summary>
    /// Gets or sets the node to render if expression evaluates to false (optional).
    /// </summary>
    public DocumentNode? FalseBranch { get; set; }

    /// <summary>
    /// Constructor for conditional nodes.
    /// </summary>
    /// <param name="expression">Condition expression</param>
    /// <param name="trueBranch">Node for true condition</param>
    /// <param name="falseBranch">Node for false condition (optional)</param>
    public ConditionalNode(string expression, DocumentNode? trueBranch = null, DocumentNode? falseBranch = null)
        : base("conditional")
    {
        Expression = expression;
        TrueBranch = trueBranch;
        FalseBranch = falseBranch;
    }
}
