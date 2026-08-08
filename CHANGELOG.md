# Changelog

Todos los cambios notables de **MotorDsl** se documentan en este archivo.

El formato se basa en [Keep a Changelog](https://keepachangelog.com/es-ES/1.1.0/)
y el proyecto sigue [Versionado Semántico](https://semver.org/lang/es/). La
versión de los paquetes se inyecta en build/pack vía `-p:PackageVersion` /
`-p:MotorDslVersion` (ver `docs/09_devops/estrategia-versionado_v1.0.md`), por lo
que el número de versión **no** vive en los `.csproj`.

## [Unreleased]

### Añadido

- **`MotorDsl.Network`**: octavo paquete, transport de impresión por socket TCP al
  puerto 9100 (RAW / JetDirect). **Habilita impresión térmica en iOS**, que hasta
  ahora era imposible porque el único transport disponible era Bluetooth Classic
  SPP y Apple no expone ese perfil a apps de terceros.

  El paquete targetea **`net10.0` puro**. No usa ninguna API de plataforma
  —`System.Net.Sockets` es BCL—, así que es consumible desde `net10.0-android` y
  `net10.0-ios` por herencia de TFM, se compila y empaqueta en cualquier sistema
  operativo, y `MotorDsl.Tests` lo referencia directamente sin el enlace por
  `<Compile Include>` que necesita `MotorDsl.Bluetooth`.

  Se registra con `AddNetworkPrinterTransport()` y expone `Kind = "network"`, de
  modo que convive con el transport Bluetooth: el servicio rutea por `Kind`.
- **`TransportChunking`** en `MotorDsl.Printing.Abstractions`: helper público con
  el chunker de tamaño fijo, para que cualquier transport lo reutilice.
- **Devcontainer** (`.devcontainer/`) para desarrollar en Linux sin instalar el SDK
  en la máquina anfitriona. Cubre los TFM `net10.0`, que es lo compilable en Linux.

### Cambiado

- `cd-nuget.yml` publica ahora los **8** paquetes, repartiendo el `pack` en dos
  runners: los seis `net10.0` en el self-hosted y `MotorDsl.Bluetooth` y
  `MotorDsl.Maui` en `macos-15`. Es una restricción estructural, no una preferencia:
  `dotnet pack` de un proyecto multi-TFM exige compilar todos sus TFM y el workload
  `ios` no existe para Linux. Antes se publicaban 4 y los otros 3 dependían de un
  script local de Windows.
- `ci.yml` deja de declarar un número fijo de tests en el nombre del job y en el
  comentario de PR: decía 185 cuando la suite ya corría 259.

### Notas para el consumidor

- Una app que imprima por red en iOS debe declarar `NSLocalNetworkUsageDescription`
  en su `Info.plist`. Sin esa clave, iOS 14+ termina la app sin mensaje útil.
- En Android, `MotorDsl.Network` requiere los permisos `INTERNET` y
  `ACCESS_NETWORK_STATE`.

## [1.0.13] - 2026-07-13

### Corregido

- **Rasterizado de imágenes bitmap devolvía el ticket completo en 0 bytes.**
  `SkiaSharpRasterizer` decodificaba con `SKBitmap.Decode(byte[])`, que en
  SkiaSharp 3.x devuelve `null` incluso para PNGs perfectamente válidos
  (reproducido con un PNG 1×1 estándar). Al devolver `null`, el rasterizador
  lanzaba `InvalidOperationException` y el `catch` de `BitmapEscPosRenderer`
  descartaba **todo** el documento (`Output = Array.Empty<byte>()`). Ahora la
  decodificación usa `SKImage.FromEncodedData` + `SKBitmap.FromImage`, que sí
  decodifica de forma fiable.
- **El estilo `bold` de los nodos de texto se ignoraba.** `LayoutEngine` sólo
  propagaba `align` al `DeviceMetadata`; los textos con `"bold": true` salían sin
  negrita. Ahora `bold` se propaga y los renderers lo aplican.

### Cambiado

- **`BitmapEscPosRenderer`: manejo de fallas de imagen por severidad.**
  - Una imagen que **no se puede rasterizar** ahora aborta el ticket con un
    `Error` **preciso** que incluye el `source` (recortado) y la causa
    (`Image rasterization failed (source='…'): <causa>`), en lugar del genérico
    `BitmapEscPos rendering failed`.
  - Una imagen que **decodifica pero sale en blanco** (típico de placeholders
    transparentes) genera un `Warning` y el ticket **sí** se imprime.
  - Se elimina la degradación silenciosa: los problemas de imagen siempre quedan
    visibles en `RenderResult.Errors` / `RenderResult.Warnings`.
- `SkiaSharpRasterizer`: el `Convert.FromBase64String` se envuelve para reportar
  un mensaje claro cuando el `source` no es base64 válido.

### Eliminado

- Trazas `Console.WriteLine` de depuración en `SkiaSharpRasterizer` y
  `BitmapEscPosRenderer` que volcaban parte del base64 de la imagen a la salida
  estándar en cada render.

[1.0.13]: https://github.com/Aplicada-Streaming/PrintThermal_Motor_Maui/releases/tag/v1.0.13
