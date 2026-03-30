# Ejemplo 02 — MotorDsl.MultaApp

> Acta de infracción de tránsito. Caso real con todas las funcionalidades de la librería.

**Estado:** Documentación de diseño — la app se implementa en Sprint 08.

---

## 1. Propósito y Audiencia

Aplicación avanzada que demuestra cómo usar MotorDsl en un escenario real de gobierno/fiscalización. Dirigida al desarrollador que ya conoce los fundamentos (ver [ejemplo-01-simple.md](ejemplo-01-simple.md)) y necesita:

- Templates complejos con todos los tipos de nodo
- Imágenes bitmap (logo) rasterizadas para impresora térmica
- Renderers custom implementados por la app (`IRenderer`)
- Generación de PDF sin dependencias en la librería core
- Integración con API REST para persistencia

**Nivel:** Avanzado  
**Ubicación:** `samples/MotorDsl.MultaApp/` *(Sprint 08)*

---

## 2. Funcionalidades

| Feature                    | Descripción                                              |
|----------------------------|----------------------------------------------------------|
| Preview MAUI               | Vista previa nativa del acta con `RenderLayout()`        |
| Hex dump ESC/POS           | Visualización de los comandos ESC/POS generados          |
| PDF                        | Generación y vista previa de PDF via QuestPDF            |
| Exportar a API REST        | Enviar el ticket en Base64 a un endpoint                 |
| Impresión térmica con logo | Imprimir en impresora BT con imagen rasterizada          |
| Código QR                  | QR de pago condicional (si permite pago online)          |
| Validación formal          | Template, datos y profile validados antes del render     |

---

## 3. Arquitectura Prevista

### Árbol de archivos

```
samples/MotorDsl.MultaApp/
├── MauiProgram.cs                  ← Registro DI + renderers custom
├── Templates/
│   └── MultaDsl.cs                 ← Template DSL + datos hardcodeados
├── Pages/
│   ├── MainPage.xaml               ← UI con pestañas
│   └── MainPage.xaml.cs            ← Lógica: preview, hex, pdf, api
├── Renderers/
│   ├── BitmapEscPosRenderer.cs     ← IRenderer: ESC/POS con imágenes
│   └── PdfRenderer.cs              ← IRenderer: PDF con QuestPDF
├── Services/
│   └── ThermalPrinterService.cs    ← Conexión BT (reutilizado)
└── MotorDsl.MultaApp.csproj
```

### Dependencias NuGet

| Paquete       | Versión | Uso                                              |
|---------------|---------|--------------------------------------------------|
| SkiaSharp     | 3.x     | Rasterizar imágenes base64 → ESC/POS bitmap      |
| QuestPDF      | 2024.x  | Generar PDF desde `LayoutedDocument`              |

> **Decisión de arquitectura:** La librería core (`MotorDsl.Core`) NO incluye estas dependencias. Es responsabilidad de la app cliente implementar los renderers que las necesiten, registrándolos como `IRenderer`.

### Integración con la librería

```
MotorDsl.MultaApp
    │
    ├── Usa MotorDsl.Core (pipeline, modelos, validación)
    ├── Usa MotorDsl.Extensions (DI, fluent API)
    ├── Usa MotorDsl.Rendering (TextRenderer,EscPosRenderer base)
    │
    └── Implementa sus propios renderers:
        ├── BitmapEscPosRenderer : IRenderer  (Target = "escpos-bitmap")
        └── PdfRenderer : IRenderer           (Target = "pdf")
```

---

## 4. Template DSL de la Multa

```json
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
        "style": { "align": "center", "width": 32, "height": 32 },
        "_comment": "Logo del organismo 32x32 PNG"
      },
      {
        "type": "text",
        "text": "",
        "_comment": "Línea en blanco después del logo"
      },
      {
        "type": "text",
        "text": "ACTA DE INFRACCIÓN",
        "style": { "align": "center", "bold": true, "size": "large" }
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
        "fields": ["articulo", "descripcion", "puntos", "monto"],
        "_comment": "Tabla de infracciones con columnas fijas"
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
              "type": "text",
              "text": "{{qrPagoUrl}}",
              "style": { "align": "center", "qr": true }
            }
          ]
        },
        "falseBranch": {
          "type": "text",
          "text": "Pague en oficinas de Tránsito Municipal",
          "style": { "align": "center" }
        },
        "_comment": "Si permite pago online muestra QR, si no indica oficina"
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
        "style": { "align": "center", "width": 24, "height": 12 },
        "_comment": "Firma digitalizada del agente como base64"
      },
      {
        "type": "text",
        "text": "Ag. {{agente.nombre}} - Leg. {{agente.legajo}}",
        "style": { "align": "center" }
      }
    ]
  }
}
```

### Tipos de nodo utilizados

| Tipo          | Cantidad | Función                                        |
|---------------|----------|------------------------------------------------|
| `container`   | 2        | Raíz y grupo del QR condicional                |
| `text`        | 19       | Encabezados, datos, separadores, pie           |
| `image`       | 2        | Logo del organismo + firma del agente          |
| `loop/table`  | 1        | Tabla de infracciones con 4 columnas           |
| `conditional` | 1        | QR de pago online vs. texto de oficina         |

---

## 5. Datos de Ejemplo

```csharp
public static Dictionary<string, object> GetSampleData() => new()
{
    // Encabezado
    ["municipio"]   = "San Miguel de Tucumán",
    ["actaNumero"]  = "SMT-2026-004571",
    ["fecha"]       = "30/03/2026 14:35",

    // Infractor
    ["infractor"] = new Dictionary<string, object>
    {
        ["nombre"]    = "Juan Carlos Pérez",
        ["dni"]       = "28.456.789",
        ["domicilio"] = "Av. Mate de Luna 2100"
    },

    // Vehículo
    ["vehiculo"] = new Dictionary<string, object>
    {
        ["patente"] = "AB 123 CD",
        ["marca"]   = "Volkswagen",
        ["modelo"]  = "Gol Trend 2019"
    },

    // Infracciones (tabla)
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

    // Totales
    ["totalMonto"]  = "23500.00",
    ["totalPuntos"] = "6",

    // Pago online
    ["permitePagoOnline"] = true,
    ["qrPagoUrl"]         = "https://multas.tucuman.gob.ar/pago/SMT-2026-004571",

    // Agente
    ["agente"] = new Dictionary<string, object>
    {
        ["nombre"] = "María López",
        ["legajo"] = "T-1247"
    },

    // Firma del agente (base64 de imagen 48x24 placeholder)
    ["firmaAgente"] = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAADAAAA" +
                      "AYCAYAAACk/IOkAAAAMklEQVR4nO3OMQEAAAgDoGl" +
                      "j/0tWwR5cQDZ5sCmpqampqampqampqampqampqampq" +
                      "an5twBf8AAFiHcj8AAAAASUVORK5CYII="
};
```

---

## 6. Renderers Custom

### A) BitmapEscPosRenderer

Extiende los comandos ESC/POS con soporte para imágenes rasterizadas con SkiaSharp.

```csharp
public class BitmapEscPosRenderer : IRenderer
{
    public string Target => "escpos-bitmap";

    public RenderResult Render(LayoutedDocument document, DeviceProfile profile)
    {
        var ms = new MemoryStream();

        foreach (var node in document.Nodes)
        {
            switch (node)
            {
                case LayoutedImageNode img:
                    // Decodificar base64 → SKBitmap → ESC/POS GS v 0
                    var bitmap = DecodeBase64ToBitmap(img.Source);
                    var escposBytes = RasterizeToBitImage(bitmap, profile.Width);
                    ms.Write(escposBytes);
                    break;

                case LayoutedTextNode txt:
                    // Delegar al EscPosRenderer base para texto
                    var textBytes = EscPosCommands.Text(txt.Text);
                    ms.Write(textBytes);
                    break;

                // ... otros nodos
            }
        }

        return new RenderResult(Target, ms.ToArray());
    }

    private SKBitmap DecodeBase64ToBitmap(string base64Source)
    {
        // Extraer bytes del data URI
        var base64 = base64Source.Split(",")[1];
        var bytes = Convert.FromBase64String(base64);
        return SKBitmap.Decode(bytes);
    }

    private byte[] RasterizeToBitImage(SKBitmap bitmap, int widthChars)
    {
        // Convertir a 1-bit y generar comando GS v 0
        // ... implementación SkiaSharp
    }
}
```

> **Decisión:** La librería core provee `EscPosCommands` con texto, códigos de barras y QR. La rasterización de imágenes queda del lado del cliente porque requiere SkiaSharp.

### B) PdfRenderer

Genera PDF usando QuestPDF, mapeando `LayoutedDocument` a elementos QuestPDF.

```csharp
public class PdfRenderer : IRenderer
{
    public string Target => "pdf";

    public RenderResult Render(LayoutedDocument document, DeviceProfile profile)
    {
        var pdfBytes = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);

                page.Content().Column(col =>
                {
                    foreach (var node in document.Nodes)
                    {
                        switch (node)
                        {
                            case LayoutedTextNode txt:
                                var textDesc = col.Item().Text(txt.Text);
                                if (txt.Bold) textDesc.Bold();
                                break;

                            case LayoutedImageNode img:
                                var imgBytes = Convert.FromBase64String(
                                    img.Source.Split(",")[1]);
                                col.Item().Image(imgBytes);
                                break;

                            // ... otros nodos
                        }
                    }
                });
            });
        }).GeneratePdf();

        return new RenderResult(Target, pdfBytes);
    }
}
```

### Registro en DI

```csharp
// MauiProgram.cs
builder.Services.AddMotorDslEngine()
    .AddTemplates(t => t.Add("acta-infraccion", MultaDsl.Template))
    .AddProfiles(p =>
    {
        p.Add(new DeviceProfile("thermal_58mm", 32, "escpos-bitmap"));
        p.Add(new DeviceProfile("a4-pdf", 80, "pdf"));
    });

// Registrar renderers custom DESPUÉS del motor
builder.Services.AddSingleton<IRendererRegistry>(sp =>
{
    var registry = new RendererRegistry();
    registry.Register(new TextRenderer());           // base
    registry.Register(new EscPosRenderer());         // base
    registry.Register(new BitmapEscPosRenderer());   // custom
    registry.Register(new PdfRenderer());            // custom
    return registry;
});
```

---

## 7. Exportar a API REST

### Endpoint esperado

```
POST /api/multas/{actaNumero}/ticket
Content-Type: application/json

{
  "actaNumero":  "SMT-2026-004571",
  "timestamp":   "2026-03-30T14:35:00Z",
  "target":      "escpos-bitmap",
  "contentB64":  "G0AbaQEA... (Base64 del ticket)",
  "pdfB64":      "JVBERi0x... (Base64 del PDF, opcional)",
  "agenteLegajo": "T-1247"
}
```

### Código de exportación

```csharp
private async void OnExportClicked(object sender, EventArgs e)
{
    // 1. Renderizar ESC/POS
    var escposProfile = new DeviceProfile("thermal_58mm", 32, "escpos-bitmap");
    var escposResult = _engine.Render(MultaDsl.Template, MultaDsl.GetSampleData(), escposProfile);

    // 2. Renderizar PDF
    var pdfProfile = new DeviceProfile("a4-pdf", 80, "pdf");
    var pdfResult = _engine.Render(MultaDsl.Template, MultaDsl.GetSampleData(), pdfProfile);

    // 3. Armar payload
    var payload = new
    {
        actaNumero   = "SMT-2026-004571",
        timestamp    = DateTime.UtcNow,
        target       = escposResult.Target,
        contentB64   = escposResult.ToBase64(),
        pdfB64       = pdfResult.ToBase64(),
        agenteLegajo = "T-1247"
    };

    // 4. Enviar
    using var http = new HttpClient();
    var response = await http.PostAsJsonAsync(
        "https://api.ejemplo.com/api/multas/SMT-2026-004571/ticket",
        payload);

    if (response.IsSuccessStatusCode)
        ShowMessage("Exportado correctamente");
}
```

---

## Relación con la Documentación

| Documento                                             | Relación                                     |
|-------------------------------------------------------|----------------------------------------------|
| [ejemplo-01-simple.md](ejemplo-01-simple.md)          | Prerrequisito — fundamentos de la librería   |
| `docs/10_developer_guide/integracion-api-rest.md`     | Patrón REST con ToBase64()                   |
| `docs/05_arquitectura_tecnica/extensibilidad-motor.md`| Cómo agregar renderers custom                |
| `docs/08_calidad_y_pruebas/guia-testing-extensibilidad.md` | Cómo testear renderers custom          |
