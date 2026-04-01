# Contexto del proyecto para Copilot

## Stack
- .NET 10, MAUI, C#
- Android + iOS
- xUnit, 185 tests

## Estructura del repositorio

```
PrintThermalDriver.sln
src/
  MotorDsl.Core/         ← Parser, Evaluator, LayoutEngine, contratos
  MotorDsl.Parser/       ← Parser JSON DSL → DocumentTemplate
  MotorDsl.Rendering/    ← EscPosRenderer, TextRenderer
  MotorDsl.Extensions/   ← DI, fluent API (AddMotorDslEngine)
  MotorDsl.Tests/        ← 185 tests xUnit
samples/
  MotorDsl.SampleApp/    ← MAUI app (Android + iOS)
  MotorDsl.MultaApp/     ← MAUI app con SkiaSharp (Android + iOS)
```

## Cuando un workflow falla

### Error de build Android
- Verificar Java 17 instalado (`actions/setup-java@v4`, distribution: `microsoft`)
- Verificar `dotnet workload install maui-android --version 10.0.100`
- Verificar TargetFramework: `net10.0-android`
- Runner: `ubuntu-latest`

### Error de build iOS
- Solo compila en `macos-15`
- Verificar `dotnet workload install ios maui maui-ios --version 10.0.100`
- Verificar TargetFramework: `net10.0-ios`
- Para simulador usar: `-p:RuntimeIdentifier=iossimulator-arm64`
- Xcode requerido: `26.0` (descarga desde Google Drive con `gdown`)
- Entitlements: `Platforms/iOS/Entitlements.Development.plist` debe existir
- Info.plist debe tener `CFBundleVersion` y `CFBundleShortVersionString`

### Error de tests
- Proyecto: `PrintThermalDriver.sln`
- Target: **185 tests, 0 errores**
- Tests en: `src/MotorDsl.Tests/`
- Comando: `dotnet test PrintThermalDriver.sln --verbosity minimal`

### Error de NuGet
- Proyectos a empaquetar: `Core`, `Parser`, `Rendering`, `Extensions`
- Solo publicar en tags `v*`
- Destino: GitHub Packages (`nuget.pkg.github.com`)
- Requiere `GITHUB_TOKEN` con permisos de escritura en packages

## Workflows disponibles

| Workflow | Trigger | Runner |
|---|---|---|
| `ci.yml` | push/PR a main | ubuntu-latest |
| `cd-android.yml` | push main / tags | ubuntu-latest |
| `cd-ios-sampleapp.yml` | push main (samples/SampleApp/**) | macos-15 |
| `cd-ios-multaapp.yml` | push main (samples/MultaApp/**) | macos-15 |
| `cd-nuget.yml` | tags v* / manual | ubuntu-latest |

## Cómo usar GitHub Actions en VS Code
1. Panel izquierdo → ícono GitHub Actions (extensión `github.vscode-github-actions`)
2. Los workflows `ci`, `cd-android`, `cd-ios-sampleapp`, `cd-ios-multaapp` están fijados en `.vscode/settings.json`
3. Click en un run fallido → ver logs inline
4. Copilot Chat: `"analiza el error de CI en el workflow X"`

## Notas de implementación iOS
- `ThermalPrinterService.SendBytesAsync` usa `#if IOS` para mostrar `DisplayAlert` y retornar (Bluetooth clásico SPP no disponible en iOS)
- Los controles de Bluetooth están ocultos en XAML con `IsVisible="{OnPlatform Android=True, iOS=False}"`
- El pipeline DSL (Core, Parser, Rendering, Extensions) funciona perfectamente en iOS
