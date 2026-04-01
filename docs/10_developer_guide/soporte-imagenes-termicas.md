# Soporte de Imágenes Bitmap - Impresoras Térmicas ESC/POS

## Restricciones 58HB6

- Solo **1bpp** (blanco/negro puro — el printer no interpreta grises ni colores)
- **Ancho máximo:** 384 px (48 mm × 8 dots/mm)
- **Ancho recomendado:** ≤ 320 px (margen de seguridad frente a papel desalineado)
- El **ancho DEBE ser múltiplo de 8** (el protocolo trabaja en bytes completos)
- **Alto máximo recomendado:** 800 px
- **Comando:** `GS v 0` → `0x1D 0x76 0x30 0x00 xL xH yL yH <datos>`
- **Bit = 1 → negro**, Bit = 0 → blanco; el **MSB = pixel izquierdo** de cada byte

## Errores de validación

| Código | Descripción |
|---|---|
| `ImageWidthExceedsMax` | Ancho de la imagen supera el máximo permitido por el perfil (384 px). |
| `ImageColorDepthNotSupported` | La imagen no es 1bpp. Debe binarizarse antes de enviar. |
| `ImageWidthNotAligned` | El ancho en pixels no es múltiplo de 8. Se debe alinear al múltiplo superior. |

## Cómo preparar imágenes

### Con GIMP (recomendado para logos)

1. Abrir la imagen original.
2. **Imagen → Modo → Escala de grises.**
3. **Colores → Umbral** — mover el slider hasta ~128 para binarizar.
4. **Imagen → Escalar imagen** — ajustar ancho a ≤ 320 px (mantener proporción).
5. **Archivo → Exportar como → BMP** — seleccionar `1 bit por pixel`.
6. Convertir el BMP a base64:
   - PowerShell: `[Convert]::ToBase64String([IO.File]::ReadAllBytes("logo.bmp"))`
   - O usar el prefijo `data:image/bmp;base64,<base64>` para usarlo en DSL.

### Con código (SkiaSharpRasterizer)

El `SkiaSharpRasterizer` que implementa `IBitmapRasterizer` escala, convierte a escala de grises y binariza automáticamente al llamar a `Rasterize(source, widthPixels)`.

```csharp
// Inyectar en el renderer:
var rasterizer = new SkiaSharpRasterizer();
var renderer = new EscPosRenderer(rasterizer);
```

## Cómo funciona el comando GS v 0

```
0x1D 0x76 0x30 m xL xH yL yH <datos raster>

m   = 0x00  → densidad normal
xL + xH*256 = bytes por fila (ancho_px / 8)
yL + yH*256 = número de filas (alto en dots)
datos       = bytes 1bpp, MSB = pixel izquierdo
```

**Ejemplo** — logo de 64 px ancho × 20 px alto:
- `bytesPerRow` = 64 / 8 = **8**
- Header: `1D 76 30 00 08 00 14 00`
- Datos: 8 bytes × 20 filas = **160 bytes**

## Integración en el motor DSL

```csharp
// En DocumentNode (ImageNode):
var img = new ImageNode("data:image/bmp;base64,<base64>");

// En DeviceProfile (58HB6):
profile.SetCapability("supports_bitmap", true);
profile.SetCapability("bitmap_max_width_px", 384);
profile.SetCapability("bitmap_recommended_width_px", 320);
profile.SetCapability("bitmap_max_height_px", 800);
profile.SetCapability("bitmap_color_depth_bpp", 1);
profile.SetCapability("bitmap_binarization_threshold", 128);
profile.SetCapability("bitmap_command", "GS_v_0");

// Renderer con soporte bitmap:
var renderer = new EscPosRenderer(new SkiaSharpRasterizer());
// O el renderer extendido:
var renderer = new BitmapEscPosRenderer(new SkiaSharpRasterizer());
```

## Fallback sin rasterizador

Si `EscPosRenderer` se instancia sin `IBitmapRasterizer` (modo legacy: `new EscPosRenderer()`), los nodos bitmap emiten el texto `[BITMAP]` + LF en lugar del comando GS v 0. Esto garantiza compatibilidad con entornos donde SkiaSharp no está disponible.

## Imágenes de referencia

Ver [`docs/11_examples/imagenes_logos/`](../11_examples/imagenes_logos/) para archivos BMP 1bpp de ejemplo (logos municipales y firmas).

| Archivo | Formato | Tamaño |
|---|---|---|
| `logo-sanpedro-print.bmp` | BMP 1bpp | 3.3 KB |
| `logo-unquillo-print.bmp` | BMP 1bpp | 7.1 KB |
| `logo-municipio-print.bmp` | BMP 4bpp | 10.3 KB |
| `firma_alberdi.bmp` | BMP 4bpp | 4.5 KB |

> **Nota:** Los archivos `.base64` contienen el mismo contenido con prefijo `data:image/bmp;base64,` listos para usar en DSL o tests.
