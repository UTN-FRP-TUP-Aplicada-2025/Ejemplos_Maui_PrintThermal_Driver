namespace MotorDsl.MultaApp.Templates;

/// <summary>
/// Template DSL del acta de infracción de tránsito y datos de ejemplo.
/// Sprint 08 — Rediseño completo.
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
            "type": "text",
            "text": "MUNICIPALIDAD DE EJEMPLO",
            "style": { "align": "center", "bold": true }
          },
          {
            "type": "text",
            "text": "DIRECCIÓN DE TRÁNSITO",
            "style": { "align": "center" }
          },
          {
            "type": "text",
            "text": "================================"
          },
          {
            "type": "text",
            "text": "ACTA DE INFRACCIÓN N°: {{nroActa}}",
            "style": { "bold": true }
          },
          {
            "type": "text",
            "text": "Fecha: {{fecha}}  Hora: {{hora}}"
          },
          {
            "type": "text",
            "text": "================================"
          },
          {
            "type": "text",
            "text": "DATOS DEL INFRACTOR:",
            "style": { "bold": true }
          },
          {
            "type": "text",
            "text": "Apellido y Nombre: {{infractor.apellido}} {{infractor.nombre}}"
          },
          {
            "type": "text",
            "text": "DNI: {{infractor.dni}}"
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
            "text": "VEHÍCULO:",
            "style": { "bold": true }
          },
          {
            "type": "text",
            "text": "Patente: {{vehiculo.patente}}"
          },
          {
            "type": "text",
            "text": "Marca/Modelo: {{vehiculo.marca}} {{vehiculo.modelo}} {{vehiculo.anio}}"
          },
          {
            "type": "text",
            "text": "================================"
          },
          {
            "type": "text",
            "text": "INFRACCIONES COMETIDAS:",
            "style": { "bold": true }
          },
          {
            "type": "loop",
            "source": "infracciones",
            "itemAlias": "inf",
            "body": {
              "type": "container",
              "layout": "vertical",
              "children": [
                {
                  "type": "text",
                  "text": "Art. {{inf.articulo}} - {{inf.descripcion}}"
                },
                {
                  "type": "text",
                  "text": "Puntos: {{inf.puntos}}  Monto: ${{inf.monto}}"
                },
                {
                  "type": "text",
                  "text": ""
                }
              ]
            }
          },
          {
            "type": "text",
            "text": "================================"
          },
          {
            "type": "text",
            "text": "TOTAL A PAGAR: ${{totalMonto}}",
            "style": { "align": "center", "bold": true }
          },
          {
            "type": "text",
            "text": "Vencimiento: {{fechaVencimiento}}"
          },
          {
            "type": "text",
            "text": "================================"
          },
          {
            "type": "text",
            "text": "Pague en: https://multas.ejemplo.gob.ar",
            "style": { "align": "center" }
          },
          {
            "type": "image",
            "source": "https://multas.ejemplo.gob.ar/pago/{{nroActa}}",
            "imageType": "qrcode"
          },
          {
            "type": "text",
            "text": "================================"
          },
          {
            "type": "text",
            "text": "Firma del Inspector:"
          },
          {
            "type": "text",
            "text": "{{inspector.nombre}} - Legajo: {{inspector.legajo}}"
          },
          {
            "type": "text",
            "text": "________________________________"
          }
        ]
      }
    }
    """;

    public static Dictionary<string, object> GetSampleData() => new()
    {
        ["nroActa"] = "2026-00123",
        ["fecha"]   = "31/03/2026",
        ["hora"]    = "14:35",

        ["infractor"] = new Dictionary<string, object>
        {
            ["apellido"]  = "García",
            ["nombre"]    = "Carlos Alberto",
            ["dni"]       = "28.456.789",
            ["domicilio"] = "Av. Libertad 1234, Piso 3"
        },

        ["vehiculo"] = new Dictionary<string, object>
        {
            ["patente"] = "AB 123 CD",
            ["marca"]   = "Toyota",
            ["modelo"]  = "Corolla",
            ["anio"]    = "2019"
        },

        ["infracciones"] = new List<Dictionary<string, object>>
        {
            new()
            {
                ["articulo"]    = "Art. 77 inc. 2",
                ["descripcion"] = "Exceso de velocidad en zona urbana - límite 40 km/h circulando a 68 km/h según radar fijo",
                ["puntos"]      = "3",
                ["monto"]       = "15000"
            },
            new()
            {
                ["articulo"]    = "Art. 48",
                ["descripcion"] = "Uso de teléfono celular durante la conducción del vehículo en movimiento",
                ["puntos"]      = "2",
                ["monto"]       = "8000"
            }
        },

        ["totalMonto"]       = "23000",
        ["fechaVencimiento"] = "30/04/2026",

        ["inspector"] = new Dictionary<string, object>
        {
            ["nombre"] = "Juan Pérez",
            ["legajo"] = "4521"
        }
    };
}
