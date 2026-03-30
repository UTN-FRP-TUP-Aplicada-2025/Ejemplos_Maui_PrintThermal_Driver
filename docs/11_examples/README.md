# Ejemplos de Uso — MotorDsl

> Aplicaciones de ejemplo que demuestran cómo integrar la librería MotorDsl en proyectos .NET MAUI.

---

## Índice de Ejemplos

| #  | Proyecto               | Nivel     | Descripción                                        |
|----|------------------------|-----------|----------------------------------------------------|
| 01 | MotorDsl.SampleApp     | Básico    | Ticket simple — aprender la librería paso a paso   |
| 02 | MotorDsl.MultaApp      | Avanzado  | Multa de tránsito — todas las funcionalidades      |

---

## Ejemplo 01 — MotorDsl.SampleApp

**Ubicación:** `samples/MotorDsl.SampleApp/`

### Qué demuestra

- Configuración inicial con `AddMotorDslEngine()`
- Template DSL básico con bindings (`{{campo}}`)
- Renderizado a texto plano y ESC/POS
- Conexión Bluetooth con impresora térmica
- Envío de comandos ESC/POS al dispositivo
- Manejo de errores de impresión con reintentos

### Requisitos previos

- .NET 10 SDK
- Visual Studio 2022+ con workload MAUI
- Dispositivo Android físico (para Bluetooth)
- Impresora térmica 58mm compatible ESC/POS (opcional — funciona sin impresora para preview)

### UI

Pantalla única con:
- Botón **Escanear** para descubrir impresoras Bluetooth
- Lista de dispositivos encontrados
- Botón **Imprimir** que renderiza el template y envía a la impresora seleccionada
- Área de texto con el hex dump del resultado ESC/POS

---

## Ejemplo 02 — MotorDsl.MultaApp

**Ubicación:** `samples/MotorDsl.MultaApp/` *(Sprint 08)*

### Qué demuestra

- Template DSL complejo con múltiples tipos de nodo (text, container, loop, conditional, image, table)
- Logo como imagen base64 embebida en el template
- Loop de infracciones con tabla
- Código QR de pago
- Firma digital del agente
- Validación formal de template (`ITemplateValidator`)
- Validación de datos (`IDataValidator`) con warnings para campos null
- Validación de profile (`IProfileValidator`)
- Preview en MAUI (vista previa en pantalla)
- Renderizado ESC/POS para impresora térmica
- Renderizado PDF con QuestPDF (implementado por la app, no por la librería)
- Hex dump para debug
- Exportar ticket a API REST vía `ToBase64()`

### Requisitos previos

- .NET 10 SDK
- Visual Studio 2022+ con workload MAUI
- Dispositivo Android físico (para Bluetooth)
- NuGet adicionales (el proyecto los incluye):
  - `SkiaSharp` — rasterización de imágenes para ESC/POS
  - `QuestPDF` — generación de PDF (el cliente implementa `IRenderer`)

### UI

Pantalla principal con pestañas:
- **Preview** — vista previa del documento de multa en pantalla
- **ESC/POS** — hex dump de los comandos generados
- **PDF** — vista previa del PDF generado
- **API** — botón para exportar el ticket en Base64 a un endpoint REST

---

## Cómo ejecutar

```bash
# Ejemplo 01
cd samples/MotorDsl.SampleApp
dotnet build -f net10.0-android

# Ejemplo 02 (Sprint 08)
cd samples/MotorDsl.MultaApp
dotnet build -f net10.0-android
```

> Ambos requieren un dispositivo Android conectado o emulador con soporte Bluetooth para pruebas de impresión.

---

## Relación con la documentación

| Documento                                                  | Relación                        |
|------------------------------------------------------------|---------------------------------|
| `docs/10_developer_guide/guia-integracion-maui.md`         | Setup inicial del motor en MAUI |
| `docs/10_developer_guide/formato-dsl-templates.md`         | Sintaxis del DSL                |
| `docs/10_developer_guide/formato-perfiles-impresora.md`    | Configuración de DeviceProfile  |
| `docs/10_developer_guide/integracion-api-rest.md`          | Patrón REST con ToBase64()      |
| `docs/11_examples/ejemplo-01-simple.md`                    | Detalle del Ejemplo 01          |
| `docs/11_examples/ejemplo-02-multa.md`                     | Detalle del Ejemplo 02          |
