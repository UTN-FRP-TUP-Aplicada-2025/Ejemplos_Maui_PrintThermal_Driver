# Ejemplo 03 — MotorDsl.MultaApp.Nuget

> Réplica de MotorDsl.MultaApp que integra el motor a través de los paquetes NuGet publicados en nuget.org, en lugar de referencias de proyecto locales. Sirve de test de integración end-to-end y como ejemplo canónico para el usuario final.

**Estado:** Implementado — Sprint 08 (actualización post-publicación NuGet v1.0.2)  
**Ubicación:** `samples/MotorDsl.MultaApp.Nuget/`

---

## 1. Propósito y Audiencia

Esta aplicación tiene dos objetivos complementarios:

1. **Test de integración NuGet:** valida que los 4 paquetes publicados en nuget.org (`MotorDsl.Core`, `MotorDsl.Parser`, `MotorDsl.Rendering`, `MotorDsl.Extensions`) funcionen correctamente en una app MAUI real, con los mismos renderers y templates de `MotorDsl.MultaApp`.

2. **Ejemplo para el usuario final:** demuestra la forma canónica de integrar MotorDsl en un proyecto nuevo, como si fuera un desarrollador externo que instala la librería desde NuGet.

**Nivel:** Avanzado  
**Audiencia:** Desarrolladores que desean integrar MotorDsl en sus propios proyectos MAUI.

---

## 2. Diferencia clave con MotorDsl.MultaApp

| Aspecto              | MotorDsl.MultaApp            | MotorDsl.MultaApp.Nuget              |
|----------------------|------------------------------|--------------------------------------|
| Referencias al motor | `<ProjectReference>` locales | `<PackageReference>` desde nuget.org |
| Versión consumida    | código fuente local          | `MotorDsl.*.1.0.2`                   |
| ApplicationId        | `com.motordsl.multaapp`      | `com.motordsl.multaapp.nuget`         |
| Namespace            | `MotorDsl.MultaApp.*`        | `MotorDsl.MultaApp.Nuget.*`           |
| Propósito principal  | Desarrollo del motor         | Consumidor final / integración        |

---

## 3. Paquetes NuGet consumidos

```xml
<!-- En MotorDsl.MultaApp.Nuget.csproj -->
<PackageReference Include="MotorDsl.Core"       Version="1.0.2" />
<PackageReference Include="MotorDsl.Parser"     Version="1.0.2" />
<PackageReference Include="MotorDsl.Rendering"  Version="1.0.2" />
<PackageReference Include="MotorDsl.Extensions" Version="1.0.2" />
```

Paquetes adicionales (terceros):

```xml
<PackageReference Include="SkiaSharp.Views.Maui.Controls" Version="3.119.0" />
<PackageReference Include="PdfSharpCore"                  Version="1.3.62" />
```

---

## 4. Estructura de archivos

```
samples/MotorDsl.MultaApp.Nuget/
├── MotorDsl.MultaApp.Nuget.csproj   ← PackageReference en lugar de ProjectReference
├── App.xaml / App.xaml.cs
├── AppShell.xaml / AppShell.xaml.cs
├── MauiProgram.cs                   ← AddMotorDslEngine() usando el NuGet
├── Controls/
│   └── MauiDocumentPreview.cs
├── Pages/
│   ├── MainPage.xaml
│   └── MainPage.xaml.cs
├── Platforms/
│   ├── Android/ (AndroidManifest.xml, MainActivity.cs, MainApplication.cs)
│   └── iOS/     (AppDelegate.cs, Program.cs, Info.plist, Entitlements.Development.plist)
├── Renderers/
│   ├── BitmapEscPosRenderer.cs      ← Renderer custom (implementado por la app, no en el NuGet)
│   ├── PdfRenderer.cs               ← Renderer custom con PdfSharpCore
│   └── SkiaSharpRasterizer.cs       ← IBitmapRasterizer con SkiaSharp
├── Resources/
│   ├── AppIcon/ (appicon.svg, appiconfg.svg)
│   ├── Fonts/   (DroidSans-Regular.ttf — embedded en PDF)
│   ├── Splash/  (splash.svg)
│   └── Styles/  (Colors.xaml, Styles.xaml)
├── Services/
│   ├── IThermalPrinterService.cs
│   ├── PrinterProfile.cs
│   └── ThermalPrinterService.cs     ← Bluetooth SPP (Android), stub (iOS)
└── Templates/
    ├── MultaDsl.cs                   ← Acta de infracción con logo BMP + firma
    ├── TicketSimpleDsl.cs
    └── ComprobanteDsl.cs
```

---

## 5. Funcionalidades

Idénticas a `MotorDsl.MultaApp`:

| Feature                         | Descripción                                             |
|---------------------------------|---------------------------------------------------------|
| Preview texto                   | Renderiza el acta como texto plano                      |
| PDF                             | Genera PDF via PdfSharpCore, lo abre con `Launcher`     |
| Impresión ESC/POS bitmap        | Rasteriza logo con SkiaSharp, emite GS v 0              |
| Bluetooth (Android)             | Escaneo, conexión y envío SPP con reconexión automática  |
| iOS stub                        | Muestra aviso, deshabilita controles BT                 |
| 3 templates                     | Acta de infracción, Ticket simple, Comprobante de pago  |

---

## 6. Configuración DI (MauiProgram.cs)

```csharp
// Igual que MultaApp — pero usando el ensamblado que viene del NuGet
builder.Services.AddMotorDslEngine()        // de MotorDsl.Extensions (NuGet)
    .AddTemplates(t => { ... })
    .AddProfiles(p => { ... })
    .AddRenderer<PdfRenderer>()             // implementado localmente en la app
    .AddRenderer<BitmapEscPosRenderer>();   // implementado localmente en la app

builder.Services.AddSingleton<IBitmapRasterizer, SkiaSharpRasterizer>(); // SkiaSharp local
```

---

## 7. Cómo ejecutar

```bash
cd samples/MotorDsl.MultaApp.Nuget

# Android
dotnet build -f net10.0-android
dotnet run -f net10.0-android  # requiere dispositivo/emulador

# iOS (macOS con workload ios instalado)
dotnet build -f net10.0-ios -p:RuntimeIdentifier=iossimulator-arm64
```

---

## 8. Cómo usar como punto de partida para un proyecto propio

1. Copiar la carpeta `samples/MotorDsl.MultaApp.Nuget/`
2. Renombrar el `.csproj` y los namespaces al nombre de tu proyecto
3. Personalizar los templates DSL en `Templates/`
4. Opcionalmente reemplazar `BitmapEscPosRenderer.cs` y `PdfRenderer.cs` con tus propios renderers
5. Compilar con `dotnet build -f net10.0-android`

No se requiere clonar el repositorio de MotorDsl — todo llega desde NuGet.

---

## 9. Control de cambios

| Versión | Fecha      | Autor     | Descripción                                              |
|---------|------------|-----------|----------------------------------------------------------|
| v1.0    | 2026-04-02 | DevOps    | Creación del ejemplo como consumidor de NuGet v1.0.2     |
