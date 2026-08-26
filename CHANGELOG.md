# Changelog

Todos los cambios notables de **MotorDsl** se documentan en este archivo.

El formato se basa en [Keep a Changelog](https://keepachangelog.com/es-ES/1.1.0/)
y el proyecto sigue [Versionado Semántico](https://semver.org/lang/es/). La
versión de los paquetes se inyecta en build/pack vía `-p:PackageVersion` /
`-p:MotorDslVersion` (ver `docs/09_devops/estrategia-versionado_v1.0.md`), por lo
que el número de versión **no** vive en los `.csproj`.


## [1.0.16] - 2026-08-26

### Corregido

- **El logo salía destruido en impresoras que no filtran los comandos de tiempo real.**
  `BluetoothPrinterTransport.WriteBytesAsync` intercalaba un sondeo de estado
  `DLE EOT n=1` (`10 04 01`) **antes de cada bloque de 256 bytes**
  (`POLL_EVERY_N_CHUNKS = 1`). El chunking es ciego al contenido, así que en un
  ticket con logo eso significaba **50 sondeos = 150 bytes inyectados dentro de
  los 12 800 bytes de datos de píxeles** de un `GS v 0`.

  En ESC/POS los comandos `DLE` son *real-time*: la impresora debe atenderlos sin
  consumirlos como datos. Una `MTP-II` (se identifica como `-PT2D-V1.0`)
  **responde el sondeo pero además suma esos 3 bytes al raster**, con lo que la
  imagen se corre 3 bytes cada 256 y sale un barrido diagonal. Una `58HB6` los
  filtra bien y por eso el defecto estaba enmascarado.

  Verificado sobre capturas HCI en un `moto g42`: mismo documento de 13 728 B a
  las dos impresoras, con sondas inyectadas en ambas (150 B y 33 B), sólo la
  `MTP-II` se rompe; y el mismo documento **sin sondas** imprime bien en la
  `MTP-II`. Tras el cambio, un ticket sale con **5 sondas, todas previas al
  documento, y 0 intercaladas**.

  Se elimina el sondeo por bloque —no gateó nunca: 59 de 59 lecturas dieron
  `Ready`, y `DLE EOT` no informa bytes libres de buffer, sólo online/papel/tapa—
  y se **conserva** `CheckHardwareFastFailAsync`, que corre una vez antes del
  envío, fuera de toda carga útil, y sigue dando `paper out` / `cover open`.

- **Una escritura bloqueada colgaba el envío sin límite.** El control de flujo
  real es el de RFCOMM (créditos): cuando la impresora deja de otorgarlos,
  `WriteAsync` se bloquea. Medido: **106,5 s** sin un solo byte y el enlace muerto
  recién ~4 min después, con 2 416 de 13 728 bytes entregados y el corte dentro
  del raster. `MotorDsl.Network` ya tenía `WriteTimeout`; el transport Bluetooth
  no tenía equivalente.

  Ahora cada bloque está acotado por `PrinterProfile.WriteTimeoutMs`. El `write`
  de un `OutputStream` de Java es bloqueante y **no atiende el
  `CancellationToken`**, así que se corre en un `Task` y se compite contra un
  delay; al vencer se lanza `TimeoutException` (→ `PrintErrorType.Timeout`,
  reintentable) y se invalida la conexión, que además libera el hilo bloqueado.

  Al vencer **no** se sondea la causa por ese mismo stream: sin créditos el sondeo
  se bloquearía igual, y si el write pendiente se destraba después, sus bytes
  quedarían intercalados en la carga útil. La causa la nombra
  `CheckHardwareFastFailAsync` al principio del siguiente intento.

### Añadido

- `PrinterProfile.WriteTimeoutMs` (default **10 s**): tope de escritura por bloque
  en el transport Bluetooth.

### Eliminado

- `WaitUntilReadyAsync`, `QueryStatusAsync`, `NextPacingDecision` y las constantes
  `POLL_EVERY_N_CHUNKS`, `MAX_READY_POLLS`, `READY_POLL_DELAY_MS`, junto con los
  dos tests del latch de degradación del pacing. Eran las piezas del sondeo por
  bloque; no se reactivan.

  Se conservan `ParseStatusByte` y `PrinterStatus`: son el parser puro del byte de
  status, no política.

> Diagnóstico completo, capturas y evidencia en el repositorio de documentación de
> `GDA.Core.APP`: `PROMPTs/Fixs/13-Impresion-Logo/`.

## [1.0.15] - 2026-08-26

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

### Corregido

- **La reconexión Bluetooth fallaba cuando había un enlace vivo que derribar** —
  típicamente la **segunda impresión consecutiva** contra la misma impresora.
  `BluetoothPrinterTransport.ConnectAsync` llamaba a `InvalidateConnection()`
  (`BluetoothSocket.Close()`) e inmediatamente abría el socket nuevo. `Close()`
  vuelve enseguida, pero el stack de Android sigue liberando RFCOMM/L2CAP/ACL:
  medido en un `moto g42` contra una `MTP-II`, la petición de conexión nueva salía
  a los **6 ms** del `Close()`, el RFCOMM viejo recién cerraba a **+35 ms** y el
  ACL caía a **+276 ms** con `hciReason 19` (*remote user terminated*), llevándose
  puesto el socket recién abierto (`find_rfc_slot_by_id unable to find RFCOMM
  slot`). La conexión devolvía `false` y el consumidor lo reportaba como
  «impresora no responde»; reintentar funcionaba porque ya no quedaba enlace vivo.

  Ahora `ConnectAsync`:
  - espera `LINK_TEARDOWN_SETTLE_MS` (600 ms) **sólo** si el cierre previo derribó
    un enlace realmente establecido — la primera conexión del proceso no espera
    nada;
  - reintenta la apertura una vez (`CONNECT_MAX_ATTEMPTS` = 2, backoff
    `CONNECT_RETRY_DELAY_MS` = 800 ms), para cubrir la variabilidad del teardown
    entre modelos de impresora.

  El marcado del teardown pendiente se hace en `InvalidateConnection()` y en
  `DisconnectAsync()`, así que también cubre la reconexión silenciosa de
  `ThermalPrinterService.ReconnectInternalAsync` tras una escritura fallida.

### Cambiado

- `BluetoothPrinterTransport`: el cuerpo de la apertura del socket se extrajo a
  `AbrirSocketAsync` (un intento: SDP + `Connect` + streams + detección de
  capacidades) y la limpieza se separó en dos métodos con semántica distinta:
  `InvalidateConnection()` (cerró un enlace vivo ⇒ marca teardown pendiente) y
  `DiscardSocket()` (socket que nunca llegó a conectar ⇒ no penaliza al próximo
  intento). `ConnectAsync` ya no envuelve la cancelación: una
  `OperationCanceledException` del llamador se propaga sin reintentar.

- `cd-nuget.yml` publica ahora los **8** paquetes, repartiendo el `pack` en dos
  runners: los seis `net10.0` en el self-hosted y `MotorDsl.Bluetooth` y
  `MotorDsl.Maui` en `macos-15`. Es una restricción estructural, no una preferencia:
  `dotnet pack` de un proyecto multi-TFM exige compilar todos sus TFM y el workload
  `ios` no existe para Linux. Antes se publicaban 4 y los otros 3 dependían de un
  script local de Windows.
- `ci.yml` deja de declarar un número fijo de tests en el nombre del job y en el
  comentario de PR: decía 185 cuando la suite ya corría 259.

> **Costo conocido**: una reconexión legítima paga ahora hasta 600 ms extra, y el
> flag de teardown no expira, así que una conexión muy posterior a un
> `Disconnect` también los paga. La conexión completa medida en el mismo equipo
> es de ~770 ms, así que el orden de magnitud no cambia.

> **Verificación**: el fix de Bluetooth vive íntegramente bajo `#if ANDROID` y
> `MotorDsl.Tests` no referencia `MotorDsl.Bluetooth`, por lo que **no tiene
> cobertura de tests unitarios**: se valida en dispositivo físico.

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

[1.0.16]: https://github.com/hdcm-dev/ThermalPrint.MotorDsl.Core/releases/tag/v1.0.16
[1.0.15]: https://github.com/hdcm-dev/ThermalPrint.MotorDsl.Core/releases/tag/v1.0.15
[1.0.13]: https://github.com/hdcm-dev/ThermalPrint.MotorDsl.Core/releases/tag/v1.0.13
