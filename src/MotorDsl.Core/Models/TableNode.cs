namespace MotorDsl.Core.Models;

/// <summary>
/// Represents a table node with headers and rows.
/// Supports structured tabular data rendering.
/// 
/// Source: modelo-datos-logico_v1.0.md
/// Contratos: contratos-del-motor_v1.0.md (Section 5 - Tipos derivados)
/// </summary>
public class TableNode : DocumentNode
{
    /// <summary>
    /// Gets or sets the table column headers.
    /// </summary>
    public List<string> Headers { get; set; }

    /// <summary>
    /// Gets or sets the table data rows.
    /// Each row is a list of cell values.
    /// </summary>
    public List<List<string>> Rows { get; set; }

    /// <summary>
    /// Constructor for table nodes.
    /// </summary>
    /// <param name="headers">Column headers</param>
    /// <param name="rows">Data rows</param>
    public TableNode(List<string>? headers = null, List<List<string>>? rows = null) : base("table")
    {
        Headers = headers ?? new List<string>();
        Rows = rows ?? new List<List<string>>();
    }

    /// <summary>
    /// Adds a data row to the table.
    /// </summary>
    public void AddRow(params string[] cells)
    {
        Rows.Add(new List<string>(cells));
    }
}
