namespace MotorDsl.MultaApp.Templates;

/// <summary>
/// Template DSL del acta de infracción de tránsito y datos de ejemplo.
/// Sprint 08 | TK-63
/// </summary>
public static class MultaDsl
{
    public const string Template = """
    {
      "id": "acta-infraccion-001",
      "version": "1.0",
      "root": {
        "type": "container",
        "layout": "vertical",
        "children": [
          {
            "type": "image",
            "source": "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAABU0lEQVR4nO2Vu0oDQRSGv9lsNokxEUUbwUawsLG0sfMBfAFfwNrCwsLKB7CxsrBQC+8XvKBGE41ZL7uzOzOSYpfdb2b+c/7/nJkFh//OoKkBtgJLwApQBy6AG8WAJYf/qJOlrOuBC+AcWJLEfOATCp7mgHlJzHfjGSNwCMwD0wlcxAD3fhGbqpxc+lXgKi/CRtYJCEgCJ2RB3NdiYINYAc4kvhXgEzhPz4FT4EoSc1K7L0H8NLTCANMuUMnMt/g56b2fUMfbovmZnzBcQJseGpfACeAxsS/B7wLQkkSb+MCvgPMSWI+GRv4FbwHrCYxnw4GJvHW3p0+2BeeA2Yi3qVkejwqF2GrKRenKD/RG2e4ybyHewbVnmz25QnwVpx4nLiB1iLeuJN4WPbXv6Ot3xKPwKHkpjP+0YDg0pv/4KfScx3A4HAr4FbSew3xA0+U4n5PxD/ACdkdx8asnkuAAAAAElFTkSuQmCC",
            "width": 32,
            "height": 32
          },
          {
            "type": "text",
            "text": ""
          },
          {
            "type": "text",
            "text": "ACTA DE INFRACCIÓN",
            "style": { "align": "center", "bold": true }
          },
          {
            "type": "text",
            "text": "Municipalidad de {{municipio}}",
            "style": { "align": "center" }
          },
          {
            "type": "text",
            "text": "Acta N° {{actaNumero}}    Fecha: {{fecha}}",
            "style": { "bold": true }
          },
          {
            "type": "text",
            "text": "================================"
          },
          {
            "type": "text",
            "text": "DATOS DEL INFRACTOR",
            "style": { "bold": true }
          },
          {
            "type": "text",
            "text": "Nombre: {{infractor.nombre}}"
          },
          {
            "type": "text",
            "text": "DNI:    {{infractor.dni}}"
          },
          {
            "type": "text",
            "text": "Domicilio: {{infractor.domicilio}}"
          },
          {
            "type": "text",
            "text": "================================"
          },
          {
            "type": "text",
            "text": "DATOS DEL VEHÍCULO",
            "style": { "bold": true }
          },
          {
            "type": "text",
            "text": "Patente: {{vehiculo.patente}}"
          },
          {
            "type": "text",
            "text": "Marca:   {{vehiculo.marca}}"
          },
          {
            "type": "text",
            "text": "Modelo:  {{vehiculo.modelo}}"
          },
          {
            "type": "text",
            "text": "================================"
          },
          {
            "type": "text",
            "text": "INFRACCIONES",
            "style": { "bold": true }
          },
          {
            "type": "table",
            "columns": [
              { "header": "Art.", "width": 5 },
              { "header": "Descripción", "width": 14 },
              { "header": "Pts", "width": 4 },
              { "header": "Monto", "width": 9 }
            ],
            "source": "infracciones",
            "fields": ["articulo", "descripcion", "puntos", "monto"]
          },
          {
            "type": "text",
            "text": "================================"
          },
          {
            "type": "text",
            "text": "TOTAL A PAGAR: ${{totalMonto}}",
            "style": { "align": "right", "bold": true }
          },
          {
            "type": "text",
            "text": "Puntos totales: {{totalPuntos}}",
            "style": { "align": "right" }
          },
          {
            "type": "text",
            "text": ""
          },
          {
            "type": "conditional",
            "expression": "{{permitePagoOnline}}",
            "trueBranch": {
              "type": "container",
              "layout": "vertical",
              "children": [
                {
                  "type": "text",
                  "text": "Pague online escaneando el QR:",
                  "style": { "align": "center" }
                },
                {
                  "type": "image",
                  "source": "{{qrPagoUrl}}",
                  "imageType": "qrcode"
                }
              ]
            },
            "falseBranch": {
              "type": "text",
              "text": "Pague en oficinas de Tránsito Municipal",
              "style": { "align": "center" }
            }
          },
          {
            "type": "text",
            "text": ""
          },
          {
            "type": "text",
            "text": "================================"
          },
          {
            "type": "text",
            "text": "Firma del agente:"
          },
          {
            "type": "image",
            "source": "{{firmaAgente}}",
            "width": 24,
            "height": 12
          },
          {
            "type": "text",
            "text": "Ag. {{agente.nombre}} - Leg. {{agente.legajo}}",
            "style": { "align": "center" }
          }
        ]
      }
    }
    """;

    public static Dictionary<string, object> GetSampleData() => new()
    {
        ["municipio"]   = "San Miguel de Tucumán",
        ["actaNumero"]  = "SMT-2026-004571",
        ["fecha"]       = "30/03/2026 14:35",

        ["infractor"] = new Dictionary<string, object>
        {
            ["nombre"]    = "Juan Carlos Pérez",
            ["dni"]       = "28.456.789",
            ["domicilio"] = "Av. Mate de Luna 2100"
        },

        ["vehiculo"] = new Dictionary<string, object>
        {
            ["patente"] = "AB 123 CD",
            ["marca"]   = "Volkswagen",
            ["modelo"]  = "Gol Trend 2019"
        },

        ["infracciones"] = new List<Dictionary<string, object>>
        {
            new()
            {
                ["articulo"]    = "42.1",
                ["descripcion"] = "Exceso velocidad",
                ["puntos"]      = "4",
                ["monto"]       = "15000.00"
            },
            new()
            {
                ["articulo"]    = "38.3",
                ["descripcion"] = "Giro prohibido",
                ["puntos"]      = "2",
                ["monto"]       = "8500.00"
            }
        },

        ["totalMonto"]  = "23500.00",
        ["totalPuntos"] = "6",

        ["permitePagoOnline"] = true,
        ["qrPagoUrl"]         = "https://multas.tucuman.gob.ar/pago/SMT-2026-004571",

        ["agente"] = new Dictionary<string, object>
        {
            ["nombre"] = "María López",
            ["legajo"] = "T-1247"
        },

        ["firmaAgente"] = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAADAAAA" +
                          "AYCAYAAACk/IOkAAAAMklEQVR4nO3OMQEAAAgDoGl" +
                          "j/0tWwR5cQDZ5sCmpqampqampqampqampqampqampq" +
                          "an5twBf8AAFiHcj8AAAAASUVORK5CYII="
    };
}
