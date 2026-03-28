namespace MotorDsl.SampleApp.Templates;

/// <summary>
/// Contiene la plantilla DSL JSON del ticket de venta y datos de ejemplo.
/// </summary>
public static class TicketDsl
{
    public const string Template = """
    {
      "id": "ticket-venta-001",
      "version": "1.0",
      "root": {
        "type": "container",
        "layout": "vertical",
        "children": [
          {
            "type": "text",
            "text": "{{storeName}}",
            "style": { "align": "center", "bold": true }
          },
          {
            "type": "text",
            "text": ""
          },
          {
            "type": "text",
            "text": "Fecha: {{fecha}}"
          },
          {
            "type": "text",
            "text": "--------------------------------"
          },
          {
            "type": "loop",
            "source": "items",
            "itemAlias": "item",
            "body": {
              "type": "container",
              "layout": "vertical",
              "children": [
                {
                  "type": "text",
                  "text": "{{item.nombre}}"
                },
                {
                  "type": "text",
                  "text": "  {{item.cantidad}} x ${{item.precio}}          ${{item.total}}"
                }
              ]
            }
          },
          {
            "type": "text",
            "text": "--------------------------------"
          },
          {
            "type": "text",
            "text": "Subtotal: ${{subtotal}}",
            "style": { "align": "right" }
          },
          {
            "type": "text",
            "text": "Impuesto: ${{impuesto}}",
            "style": { "align": "right" }
          },
          {
            "type": "text",
            "text": "TOTAL: ${{total}}",
            "style": { "align": "right", "bold": true }
          },
          {
            "type": "text",
            "text": ""
          },
          {
            "type": "text",
            "text": "{{footer}}",
            "style": { "align": "center" }
          }
        ]
      }
    }
    """;

    /// <summary>
    /// Retorna los datos de ejemplo para el ticket (mismos que el original).
    /// </summary>
    public static Dictionary<string, object> GetSampleData() => new()
    {
        ["storeName"] = "MI NEGOCIO",
        ["fecha"] = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
        ["items"] = new List<Dictionary<string, object>>
        {
            new() { ["nombre"] = "Producto 1", ["cantidad"] = "2", ["precio"] = "15.50", ["total"] = "31.00" },
            new() { ["nombre"] = "Producto 2", ["cantidad"] = "1", ["precio"] = "25.00", ["total"] = "25.00" },
            new() { ["nombre"] = "Producto 3", ["cantidad"] = "3", ["precio"] = "8.75",  ["total"] = "26.25" }
        },
        ["subtotal"] = "82.25",
        ["impuesto"] = "13.16",
        ["total"] = "95.41",
        ["footer"] = "Gracias por su compra!"
    };
}
