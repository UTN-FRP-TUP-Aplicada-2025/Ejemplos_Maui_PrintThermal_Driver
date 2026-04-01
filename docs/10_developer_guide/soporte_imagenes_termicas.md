# Soporte de Imágenes Bitmap en Impresoras Térmicas ESC/POS

**Versión:** 1.0  
**Sprint:** 08 (extensión)  
**Aplica a:** MotorDsl — EscPosRenderer, DeviceProfile, ImageNode

---

## 1. Introducción

Las impresoras térmicas ESC/POS tienen soporte **muy limitado** para imágenes
bitmap. A diferencia de impresoras de escritorio, estas impresoras:

- Solo soportan **1 bit por pixel** (blanco o negro, sin escala de grises)
- Tienen un **ancho máximo fijo** determinado por el ancho del papel
- Requieren que el ancho de la imagen sea **múltiplo de 8**
- No soportan escalado de imágenes (lo que se envía es lo que se imprime)

---

## 2. Especificaciones de la 58HB6-6101

| Parámetro | Valor | Notas |
|-----------|-------|-------|
| Ancho papel | 58mm | |
| Área imprimible | ~48mm | ~384 dots a 8 dots/mm |
| Resolución | 8 dots/mm (203 dpi aprox) | Característica del cabezal |
| Profundidad color | **1 bpp** | Solo blanco/negro |
| Escala de grises | **NO** | No soportado |
| Color | **NO** | No soportado |
| Ancho máximo imagen | **384 px** | Absoluto (puede truncar) |
| Ancho recomendado | **≤ 320 px** | Con margen de seguridad |
| Ancho mínimo recomendado | **8 px** | Múltiplo de 8 |
| Alineación de ancho | **Múltiplo de 8** | Requerimiento del protocolo |
| Alto máximo | Sin límite teórico | Se imprime por bandas |
| Alto recomendado (logo) | **≤ 100 px** | Para logos/firmas |
| Comando imagen | `GS v 0` (0x1D 0x76 0x30) | ESC/POS estándar |

---

## 3. Comando ESC/POS para imágenes (GS v 0)

La impresora 58HB6 usa el comando **GS v 0** para imprimir imágenes raster:

```
0x1D 0x76 0x30 m xL xH yL yH d1 d2 ... dk
```

| Byte | Valor | Descripción |
|------|-------|-------------|
| 0x1D | GS | Grupo separador |
| 0x76 | v | Comando imagen raster |
| 0x30 | 0 | Subcomando |
| m | 0x00 | Modo: Normal density (1x1) |
| xL | `(bytes_por_fila) & 0xFF` | Ancho en bytes, byte bajo |
| xH | `(bytes_por_fila) >> 8` | Ancho en bytes, byte alto |
| yL | `(alto_px) & 0xFF` | Alto en pixels, byte bajo |
| yH | `(alto_px) >> 8` | Alto en pixels, byte alto |
| d1..dk | Datos bitmap | 1 bit/px, MSB primero, fila por fila |

**Cálculo de bytes:**
```
bytes_por_fila = ceil(ancho_px / 8)  → debe ser entero
total_bytes    = bytes_por_fila × alto_px
```

**Orden de bits:** MSB (bit más significativo) = pixel más a la izquierda.  
Bit = 1 → punto negro, Bit = 0 → punto blanco.

---

## 4. Requisitos para imágenes compatibles

### 4.1 Requisitos obligatorios

| Requisito | Descripción | Error si no se cumple |
|-----------|-------------|----------------------|
| **1 bpp** | La imagen DEBE ser binaria (1 bit por pixel) | `ImageColorDepthNotSupported` |
| **Ancho múltiplo de 8** | Si no lo es, se padea automáticamente (o error) | `ImageWidthNotAligned` |
| **Ancho ≤ 384 px** | Para papel 58mm a 8 dots/mm | `ImageWidthExceedsMax` |
| **Alto > 0** | Al menos 1 fila | `ImageHeightInvalid` |
| **Datos no nulos** | Los bytes de imagen deben existir | `ImageDataNull` |

### 4.2 Recomendaciones

| Recomendación | Justificación |
|---------------|---------------|
| Ancho ≤ 320 px | Margen de seguridad para variaciones de papel |
| Alto ≤ 100 px (logos) | Logos más altos consumen mucho papel |
| Alto ≤ 60 px (firmas) | Las firmas suelen ser anchas y bajas |
| Umbral binarización ≤ 128 | Mejor contraste para logos oscuros sobre fondo blanco |
| Formato fuente recomendado | BMP 1bpp o PNG en escala de grises bien contrastado |

---

## 5. Modelo de datos — ImageNode

### 5.1 Definición del nodo en el DSL

```json
{
  "type": "image",
  "source": "data:image/png;base64,<BASE64_DATOS>",
  "imageType": "bitmap",
  "width": 200,
  "style": { "align": "center" }
}
```

### 5.2 Campos del ImageNode

| Campo | Tipo | Requerido | Descripción |
|-------|------|-----------|-------------|
| `type` | string | Sí | Siempre `"image"` |
| `source` | string | Sí | URI de datos en base64 o URL de imagen |
| `imageType` | string | Sí | `"bitmap"`, `"qrcode"`, `"barcode"` |
| `width` | int | No | Ancho deseado en pixels (se usa para validación) |
| `style.align` | string | No | `"left"`, `"center"`, `"right"` |

### 5.3 Formato de source para bitmap

```
data:image/png;base64,<BASE64>
data:image/bmp;base64,<BASE64>
data:image/jpeg;base64,<BASE64>
```

> **Nota:** El renderer convierte la imagen a 1bpp antes de enviar.
> Si la imagen original es a color o escala de grises, se binariza
> aplicando un umbral (por defecto 128).

---

## 6. Validaciones por perfil de impresora

El `DeviceProfile` debe configurar las capacidades de imagen:

```csharp
var profile58HB6 = new DeviceProfile("58HB6", 32, "escpos")
{
    CodePage = 437,
    CodePageCommand = new byte[] { 0x1B, 0x74, 0x00 },
    BaudRate = 115200,
    SupportedBarcodes = new List<string> { "EAN13", "QR", ... }
};

// Capacidades de imagen:
profile58HB6.SetCapability("supports_bitmap", true);
profile58HB6.SetCapability("bitmap_max_width_px", 384);
profile58HB6.SetCapability("bitmap_recommended_width_px", 320);
profile58HB6.SetCapability("bitmap_max_height_px", 800);
profile58HB6.SetCapability("bitmap_color_depth_bpp", 1);
profile58HB6.SetCapability("bitmap_binarization_threshold", 128);
profile58HB6.SetCapability("bitmap_command", "GS_v_0");
```

### 6.1 Errores de validación

Cuando un `ImageNode` con `imageType = "bitmap"` no cumple los requisitos
del perfil activo, el sistema debe generar errores descriptivos:

| Código de error | Descripción | Acción sugerida |
|-----------------|-------------|-----------------|
| `ImageColorDepthNotSupported` | La imagen tiene más de 1bpp y el perfil exige 1bpp | Convertir a escala de grises y binarizar antes de enviar |
| `ImageWidthExceedsMax` | Ancho de imagen supera el máximo del perfil | Escalar o recortar la imagen antes de enviar |
| `ImageWidthNotAligned` | Ancho no es múltiplo de 8 | El renderer debe padear automáticamente con bits 0 |
| `ImageHeightExceedsMax` | Alto supera el máximo configurado | Dividir en bandas o escalar |
| `ImageDataNull` | Source es null, vacío o base64 inválido | Verificar el dato de entrada |
| `BitmapNotSupported` | El perfil no soporta imágenes bitmap | Usar placeholder textual `[IMAGEN]` |

---

## 7. Procesamiento interno en EscPosRenderer

El flujo de procesamiento de un ImageNode bitmap:

```
ImageNode (source base64)
    ↓
1. Decodificar base64 → bytes[]
    ↓
2. Decodificar imagen (PNG/BMP/JPEG) → píxeles RGBA
    ↓
3. Validar dimensiones contra perfil
   - ancho ≤ profile.GetCapability("bitmap_max_width_px")
   - alto  ≤ profile.GetCapability("bitmap_max_height_px")
    ↓
4. Binarizar (convertir a 1bpp)
   - Por cada pixel: (R*0.299 + G*0.587 + B*0.114) > umbral → blanco, sino negro
   - Umbral default = 128 (configurable en perfil)
    ↓
5. Padear ancho a múltiplo de 8
    ↓
6. Empaquetar: 8 pixels → 1 byte, MSB = pixel izquierdo
    ↓
7. Construir comando GS v 0:
   [0x1D, 0x76, 0x30, 0x00, xL, xH, yL, yH, ...datos...]
    ↓
8. Agregar al buffer ESC/POS
```

### 7.1 Cuando IBitmapRasterizer no está disponible

Si no hay rasterizador (SkiaSharp no disponible en la plataforma):

```
EscPosRenderer → texto placeholder: "[BITMAP: logo]"
PdfRenderer    → intento de decodificación nativa de base64
```

---

## 8. Imágenes de referencia

En la carpeta `docs/11_examples/imagenes_logos/` se incluyen imágenes
de referencia que funcionan correctamente con la 58HB6:

| Archivo | Dimensiones | Descripción |
|---------|-------------|-------------|
| `logo_municipio.bmp` | 320×80 px, 1bpp | Logo ejemplo municipio |
| `logo_municipio.b64` | — | Base64 del BMP anterior |
| `firma_inspector.bmp` | 200×60 px, 1bpp | Firma ejemplo inspector |
| `firma_inspector.b64` | — | Base64 del BMP anterior |

### 8.1 Cómo preparar imágenes para la 58HB6

**Con GIMP (libre):**
1. Abrir imagen original
2. Imagen → Escala → Ancho ≤ 320 px, Alto ≤ 100 px
3. Imagen → Modo → Escala de grises
4. Colores → Umbral → ajustar hasta buen contraste
5. Imagen → Modo → Blanco y negro (1bpp)
6. Exportar como BMP (formato: 1 bit por muestra)
7. Convertir a base64: `certutil -encode logo.bmp logo.b64` (Windows)

**Con .NET (programático):**
```csharp
using SkiaSharp;

byte[] PrepararParaTermica(byte[] imagenOriginal, int anchoMax = 320)
{
    using var bitmap = SKBitmap.Decode(imagenOriginal);
    
    // Escalar si es necesario
    int nuevoAncho = Math.Min(bitmap.Width, anchoMax);
    int nuevoAlto  = (int)(bitmap.Height * ((float)nuevoAncho / bitmap.Width));
    // Alinear a múltiplo de 8
    nuevoAncho = (nuevoAncho + 7) / 8 * 8;
    
    using var escalado = bitmap.Resize(
        new SKImageInfo(nuevoAncho, nuevoAlto), SKFilterQuality.High);
    
    // Binarizar
    var resultado = new SKBitmap(nuevoAncho, nuevoAlto);
    for (int y = 0; y < nuevoAlto; y++)
        for (int x = 0; x < nuevoAncho; x++)
        {
            var pixel = escalado.GetPixel(x, y);
            byte gris = (byte)(pixel.Red * 0.299 + pixel.Green * 0.587 + pixel.Blue * 0.114);
            resultado.SetPixel(x, y, gris < 128 ? SKColors.Black : SKColors.White);
        }
    
    using var imagen = SKImage.FromBitmap(resultado);
    using var datos  = imagen.Encode(SKEncodedImageFormat.Png, 100);
    return datos.ToArray();
}
```

---

## 9. Impacto en el modelo de datos DSL

### 9.1 Antes de enviar datos al motor (recomendado)

Validar que las imágenes en el DSL cumplen los requisitos del perfil:

```csharp
// En la capa de preparación de datos, antes de llamar a engine.Render()
void ValidarImagenesEnDatos(Dictionary<string, object> datos, DeviceProfile perfil)
{
    int maxAncho = (int)(perfil.GetCapability("bitmap_max_width_px") ?? 384);
    int maxAlto  = (int)(perfil.GetCapability("bitmap_max_height_px") ?? 800);
    int bpp      = (int)(perfil.GetCapability("bitmap_color_depth_bpp") ?? 1);
    
    foreach (var valor in datos.Values)
    {
        if (valor is string s && s.StartsWith("data:image"))
        {
            // Validar dimensiones y profundidad de color
            ValidarImagenBase64(s, maxAncho, maxAlto, bpp);
        }
    }
}
```

### 9.2 Errores de validación en RenderResult

Cuando una imagen no es compatible, el motor agrega un warning o error:

```csharp
// En RenderResult.Errors o RenderResult.Warnings:
"ImageWidthExceedsMax: imagen 'logoMunicipio' tiene 500px, máximo 384px para perfil '58HB6'"
"ImageColorDepthNotSupported: imagen 'firmaInspector' es color, perfil '58HB6' requiere 1bpp"
```

---

## 10. Tabla resumen de compatibilidad

| Tipo imagen | 58HB6 | Impresoras 80mm | Notas |
|-------------|-------|-----------------|-------|
| 1bpp BMP | ✅ Soportado | ✅ Soportado | Formato nativo |
| 1bpp PNG | ✅ Si se convierte | ✅ Si se convierte | Requiere rasterizador |
| Escala grises | ⚠️ Binarizar | ⚠️ Binarizar | Se aplica umbral |
| Color RGB | ⚠️ Binarizar | ⚠️ Binarizar | Pérdida de información |
| Ancho > 384px | ❌ Error | ❌ Error | Trunca o corrompe |
| Ancho > 320px | ⚠️ Warning | ✅ OK (80mm) | Riesgo de truncado |
| Alto > 800px | ⚠️ Warning | ⚠️ Warning | Mucho papel |
| QR Code | ✅ GS (k) | ✅ GS (k) | Comando separado |
| Barcode | ✅ GS k | ✅ GS k | Ver barcodes soportados |

---

*Documento técnico — MotorDsl v1.0 — Sprint 08*
