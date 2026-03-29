# Caso de Uso: Renderizar Documento a PDF

**Código:** CU-09
**Archivo:** CU-09-renderizar-pdf_v2.0.md
**Versión:** 2.0
**Estado:** ⚠️ FUERA DEL ALCANCE DE LA LIBRERÍA CORE
**Fecha:** 2026-05-27
**Autor:** Equipo Funcional / Arquitectura

---

# 1. Propósito

Este caso de uso describe la generación de documentos PDF a partir de la representación abstracta del motor DSL.

> **Decisión v2.0:** La generación de PDF queda **fuera del alcance de la librería core** del motor DSL. El motivo principal es que cualquier implementación de PDF requiere dependencias de terceros (iTextSharp, QuestPDF, PdfSharpCore, etc.) que no deben formar parte del núcleo del motor.

---

# 2. Motivo de exclusión

| Aspecto | Detalle |
|---------|---------|
| **Dependencias externas** | Todas las librerías PDF .NET son paquetes de terceros con licencias variadas (AGPL, comercial, MIT). El motor core debe mantenerse libre de dependencias externas. |
| **Complejidad** | El renderizado PDF involucra coordenadas absolutas, gestión de páginas, fuentes embebidas y manejo de imágenes que exceden el alcance de un renderer estándar. |
| **Variabilidad** | Cada cliente puede tener preferencias distintas de librería PDF según requisitos de licencia, rendimiento o compatibilidad. |
| **Principio de extensibilidad** | El motor ya provee `IRenderer` + `IRendererRegistry`, lo cual permite que cualquier cliente implemente su propio renderer PDF sin modificar la librería. |

---

# 3. Alternativa: Implementación por el cliente

El sistema consumidor puede implementar un renderer PDF y registrarlo vía `IRendererRegistry`. El motor procesará el pipeline completo y delegará la generación de bytes PDF al renderer del cliente.

## 3.1 Ejemplo de implementación (QuestPDF)

```csharp
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using MotorDsl.Core.Contracts;
using MotorDsl.Core.Models;

public class PdfRenderer : IRenderer
{
    public string Target => "pdf";

    public RenderResult Render(LayoutedDocument document, DeviceProfile profile)
    {
        try
        {
            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);

                    page.Content().Column(column =>
                    {
                        foreach (var line in document.Lines)
                        {
                            column.Item().Text(line.Content);
                        }
                    });
                });
            }).GeneratePdf();

            return new RenderResult
            {
                IsSuccessful = true,
                Output = pdfBytes,
                Target = "pdf"
            };
        }
        catch (Exception ex)
        {
            return new RenderResult
            {
                IsSuccessful = false,
                Errors = new[] { ex.Message },
                Target = "pdf"
            };
        }
    }
}
```

## 3.2 Registro del renderer

```csharp
// En la configuración del cliente
services.AddMotorDslEngine();
services.AddSingleton<IRenderer, PdfRenderer>();

// El renderer se registra automáticamente en IRendererRegistry
```

## 3.3 Uso

```csharp
var profile = new DeviceProfile
{
    Name = "PDF-A4",
    Width = 80,
    RenderTarget = "pdf"
};

var result = engine.Render(templateDsl, data, profile);

if (result.IsSuccessful)
{
    var pdfBytes = (byte[])result.Output;
    File.WriteAllBytes("ticket.pdf", pdfBytes);
}
```

---

# 4. Pipeline del motor con renderer PDF externo

```text
DSL JSON → Parser → AST → Evaluator → EvaluatedDocument
  → LayoutEngine → LayoutedDocument
    → RendererRegistry.GetRenderer("pdf")
      → PdfRenderer (implementación del CLIENTE)
        → RenderResult { Output = byte[] (PDF) }
```

El motor ejecuta todo el pipeline y solo delega el paso final de renderizado al renderer registrado por el cliente.

---

# 5. Criterios de Aceptación (para la extensibilidad)

## CA-01 Registro de renderer externo

**Dado** un cliente que implementa `IRenderer` con `Target => "pdf"`
**Cuando** lo registra vía DI
**Entonces** `IRendererRegistry` lo resuelve correctamente con `GetRenderer("pdf")`.

---

## CA-02 Pipeline completo con renderer PDF

**Dado** un renderer PDF registrado
**Cuando** se ejecuta `engine.Render()` con `RenderTarget = "pdf"`
**Entonces** el motor ejecuta el pipeline completo y delega al `PdfRenderer` del cliente.

---

## CA-03 Sin dependencia en la librería core

**Dado** la librería core del motor
**Cuando** se inspecciona
**Entonces** no contiene dependencias de librerías PDF.

---

# 6. Cambios respecto a versión 1.0

* **Estado:** Cambiado de "Futuro" a "Fuera del Alcance de la Librería Core".
* **Decisión arquitectónica:** Se documenta formalmente que la librería no incluirá un renderer PDF.
* **Alternativa:** Se documenta cómo el cliente puede implementar su propio `PdfRenderer` usando cualquier librería PDF.
* **Ejemplo:** Se incluye ejemplo completo con QuestPDF como referencia.

---

# 7. Notas

La decisión de excluir PDF del core es consistente con el principio de diseño del motor: la librería provee contratos y el pipeline, mientras que las implementaciones específicas de infraestructura o tecnología son responsabilidad del sistema consumidor. El mecanismo `IRenderer` + `IRendererRegistry` garantiza que esto sea posible sin modificar el núcleo.

---

**Fin del documento**
