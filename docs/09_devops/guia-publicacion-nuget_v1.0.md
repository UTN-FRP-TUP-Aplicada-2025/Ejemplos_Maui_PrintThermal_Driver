# Guía de Publicación y Consumo de Paquetes NuGet  
**Archivo:** guia-publicacion-nuget_v1.0.md  
**Proyecto:** Motor DSL (Librería de Generación de Documentos)  
**Versión:** v1.0  
**Estado:** Aprobado  
**Fecha:** 2026-04-02  
**Owner:** Arquitectura / DevOps  

---

## 1. Propósito

Este documento describe los pasos necesarios para empaquetar, publicar y consumir los componentes del Motor DSL como paquetes NuGet, permitiendo su reutilización en cualquier proyecto .NET (consola, API, MAUI, Blazor, etc.).

---

## 2. Conceptos fundamentales

### 2.1 ¿Qué es un paquete NuGet?

NuGet es el administrador de paquetes oficial para .NET. Un paquete NuGet (`.nupkg`) es un archivo comprimido que contiene:

- Las DLLs compiladas de la librería
- Metadata (autor, versión, descripción, licencia)
- Declaración de dependencias hacia otros paquetes

Cuando un consumidor instala un paquete, NuGet descarga automáticamente ese paquete **y todas sus dependencias transitivas**.

### 2.2 ¿Qué es una dependencia transitiva?

Si el paquete **A** depende de **B**, y **B** depende de **C**, al instalar **A** se descargan automáticamente **B** y **C**. El consumidor no necesita instalarlos manualmente.

```
MotorDsl.Extensions
  └── MotorDsl.Core
  └── MotorDsl.Parser       → depende de Core
  └── MotorDsl.Rendering    → depende de Core
```

Al instalar `MotorDsl.Extensions`, NuGet trae transitivamente Core, Parser y Rendering.

### 2.3 Estrategias de empaquetado: monolítico vs granular

Existen dos enfoques principales para publicar una librería compuesta por varios proyectos:

| Estrategia | Descripción | Ejemplo de la industria |
|---|---|---|
| **Monolítica** | Un solo paquete con todo | `Newtonsoft.Json` |
| **Granular** | Varios paquetes con dependencias entre sí | `Microsoft.Extensions.DependencyInjection` + `Microsoft.Extensions.DependencyInjection.Abstractions` |

#### Ventajas del enfoque granular
- El consumidor instala solo lo que necesita (ej: solo los contratos/interfaces)
- Menor huella de dependencias en proyectos livianos
- Permite versionado independiente por componente

#### Ventajas del enfoque monolítico
- Menor complejidad operativa (1 versión, 1 publicación)
- El consumidor instala un solo paquete
- No hay problemas de sincronización de versiones entre paquetes

### 2.4 Cuadro comparativo de opciones para este proyecto

| Opción | Paquetes publicados | El consumidor instala | Complejidad |
|---|---|---|---|
| **A) 4 paquetes** (actual) | `Core`, `Parser`, `Rendering`, `Extensions` | `MotorDsl.Extensions` | Alta — 4 versiones sincronizadas |
| **B) 1 paquete** | `MotorDsl` (todo junto) | `MotorDsl` | Baja — 1 versión, 1 artefacto |
| **C) 2 paquetes** | `MotorDsl.Core` (contratos) + `MotorDsl` (todo) | `MotorDsl` | Media — separación contratos/implementación |

### 2.5 Decisión adoptada

Se adopta la **opción A (4 paquetes)** con `MotorDsl.Extensions` como punto de entrada único recomendado para el consumidor. Esto permite flexibilidad futura (consumo granular) manteniendo una experiencia simple para el caso de uso principal.

> **Regla para el consumidor:** instalar únicamente `MotorDsl.Extensions`. Este paquete trae todo lo necesario de forma transitiva.

---

## 3. Arquitectura de paquetes

### 3.1 Diagrama de dependencias

```
┌──────────────────────────────────┐
│       MotorDsl.Extensions        │  ← El consumidor instala ESTE
│  (DI, fluent API, AddMotorDsl)   │
└──────┬──────────┬──────────┬─────┘
       │          │          │
       ▼          ▼          ▼
┌──────────┐ ┌──────────┐ ┌──────────────┐
│  .Core   │ │  .Parser │ │  .Rendering  │
│(contratos│ │(JSON DSL │ │(EscPos, Text │
│ modelos) │ │ parser)  │ │  renderers)  │
└──────────┘ └────┬─────┘ └──────┬───────┘
                  │              │
                  ▼              ▼
             ┌──────────────────────┐
             │     MotorDsl.Core    │
             └──────────────────────┘
```

### 3.2 Tabla de paquetes

| Paquete | Contenido | Dependencias NuGet | Dependencias internas |
|---|---|---|---|
| `MotorDsl.Core` | Contratos, modelos, evaluador, layout engine | Ninguna | — |
| `MotorDsl.Parser` | Parser JSON DSL → `DocumentTemplate` | — | → Core |
| `MotorDsl.Rendering` | `EscPosRenderer`, `TextRenderer` | `System.Text.Encoding.CodePages` | → Core |
| `MotorDsl.Extensions` | DI, fluent API `AddMotorDslEngine()` | `Microsoft.Extensions.DependencyInjection.Abstractions` | → Core, Parser, Rendering |

### 3.3 ¿Qué instala el consumidor?

```bash
dotnet add package MotorDsl.Extensions --version 1.0.0
```

Esto descarga transitivamente: `MotorDsl.Core`, `MotorDsl.Parser`, `MotorDsl.Rendering` y sus dependencias externas.

---

## 4. Prerrequisitos

### 4.1 Herramientas

- .NET SDK 10.0 o superior
- Git (para tags de versionado)

### 4.2 Cuentas y tokens

| Destino | Cuenta requerida | Token / API Key |
|---|---|---|
| **GitHub Packages** | Cuenta GitHub con acceso al repositorio | `GITHUB_TOKEN` (automático en Actions) o PAT con scope `read:packages` / `write:packages` |
| **NuGet.org** (futuro) | Cuenta en [nuget.org](https://www.nuget.org) | API Key generada en Account → API Keys |

---

## 5. Metadata NuGet en los proyectos

### 5.1 Estado actual

Los archivos `.csproj` actuales **no tienen metadata de paquete**. Ejemplo de `MotorDsl.Core.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
```

Faltan propiedades esenciales: `PackageId`, `Description`, `Authors`, `License`, `RepositoryUrl`, etc.

### 5.2 Recomendación: crear `Directory.Build.props`

Crear un archivo `src/Directory.Build.props` para centralizar las propiedades compartidas entre los 4 proyectos empaquetables:

```xml
<Project>
  <PropertyGroup>
    <Authors>UTN - Tecnicatura Universitaria en Programación</Authors>
    <Company>UTN</Company>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>
    <RepositoryUrl>https://github.com/OWNER/Ejemplos_Maui_PrintThermal_Driver</RepositoryUrl>
    <RepositoryType>git</RepositoryType>
    <PackageTags>thermal-printer;escpos;dsl;dotnet;maui</PackageTags>
    <PackageReadmeFile>README.md</PackageReadmeFile>
  </PropertyGroup>
</Project>
```

Y en cada `.csproj` agregar la propiedad específica:

```xml
<PropertyGroup>
  <PackageId>MotorDsl.Core</PackageId>
  <Description>Contratos, modelos y motor de evaluación del DSL para impresión térmica.</Description>
</PropertyGroup>
```

> **Nota:** estos cambios no están aplicados aún. Son una mejora recomendada antes de la primera publicación formal.

### 5.3 Propiedades recomendadas por paquete

| Paquete | `PackageId` | `Description` |
|---|---|---|
| Core | `MotorDsl.Core` | Contratos, modelos y motor de evaluación del DSL para impresión térmica |
| Parser | `MotorDsl.Parser` | Parser JSON DSL que transforma plantillas en DocumentTemplate |
| Rendering | `MotorDsl.Rendering` | Renderizadores ESC/POS y texto plano para documentos DSL |
| Extensions | `MotorDsl.Extensions` | Integración con DI y fluent API para el Motor DSL de impresión térmica |

---

## 6. Publicación paso a paso

### 6.1 Prueba local con `dotnet pack`

Generar los `.nupkg` localmente para verificar que el empaquetado funciona:

```bash
# Desde la raíz del repositorio
dotnet build src/MotorDsl.Extensions/MotorDsl.Extensions.csproj -c Release

dotnet pack src/MotorDsl.Core/MotorDsl.Core.csproj -c Release -o ./nupkg
dotnet pack src/MotorDsl.Parser/MotorDsl.Parser.csproj -c Release -o ./nupkg
dotnet pack src/MotorDsl.Rendering/MotorDsl.Rendering.csproj -c Release -o ./nupkg
dotnet pack src/MotorDsl.Extensions/MotorDsl.Extensions.csproj -c Release -o ./nupkg
```

Verificar el contenido:

```bash
ls ./nupkg/
# MotorDsl.Core.1.0.0.nupkg
# MotorDsl.Parser.1.0.0.nupkg
# MotorDsl.Rendering.1.0.0.nupkg
# MotorDsl.Extensions.1.0.0.nupkg
```

### 6.2 Publicación en GitHub Packages (automática vía CI/CD)

El workflow `cd-nuget.yml` se encarga automáticamente. Los pasos son:

#### Paso 1 — Crear un tag de versión

```bash
git tag v1.0.0
git push origin v1.0.0
```

#### Paso 2 — El workflow se ejecuta automáticamente

1. Ejecuta los 185 tests
2. Empaqueta los 4 proyectos con la versión del tag (ej: `1.0.0`)
3. Publica en `https://nuget.pkg.github.com/<OWNER>/index.json`
4. Sube los `.nupkg` como artifacts del workflow (retención 30 días)

#### Paso 3 — Verificar

- En GitHub → repositorio → Packages: deben aparecer los 4 paquetes
- En el workflow run → Artifacts: descargar los `.nupkg`

### 6.3 Publicación en GitHub Packages (manual vía workflow_dispatch)

1. Ir a GitHub → Actions → **CD NuGet - Publish MotorDsl**
2. Click en **Run workflow**
3. Ingresar versión (ej: `1.0.0`)
4. El workflow ejecuta los mismos pasos que con un tag

### 6.4 Versiones preview (automáticas)

Cada push a `main` genera paquetes con versión `0.0.0-preview.<run_number>` como artifacts descargables (no se publican al feed). Útil para pruebas internas.

### 6.5 Publicación en NuGet.org (opcional / futuro)

Para publicar en el repositorio público de NuGet:

#### Paso 1 — Obtener API Key

1. Ir a [nuget.org/account/apikeys](https://www.nuget.org/account/apikeys)
2. Crear una API Key con scope **Push** para los paquetes `MotorDsl.*`
3. Copiar la key (solo se muestra una vez)

#### Paso 2 — Publicar manualmente

```bash
dotnet nuget push ./nupkg/*.nupkg \
  --api-key <TU_API_KEY> \
  --source https://api.nuget.org/v3/index.json \
  --skip-duplicate
```

#### Paso 3 — (Futuro) Automatizar en CI/CD

Agregar un secret `NUGET_API_KEY` en el repositorio y un step adicional en `cd-nuget.yml`:

```yaml
- name: Publish to NuGet.org
  if: startsWith(github.ref, 'refs/tags/v')
  run: |
    dotnet nuget push ./nupkg/*.nupkg \
      --api-key ${{ secrets.NUGET_API_KEY }} \
      --source https://api.nuget.org/v3/index.json \
      --skip-duplicate
```

---

## 7. Consumo desde un proyecto .NET

### 7.1 Configurar el feed (solo para GitHub Packages)

GitHub Packages requiere autenticación. Crear un archivo `nuget.config` en la raíz del proyecto consumidor:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="github" value="https://nuget.pkg.github.com/OWNER/index.json" />
  </packageSources>
  <packageSourceCredentials>
    <github>
      <add key="Username" value="TU_USUARIO_GITHUB" />
      <add key="ClearTextPassword" value="TU_PAT_CON_READ_PACKAGES" />
    </github>
  </packageSourceCredentials>
</configuration>
```

> **Seguridad:** no commitear tokens en el repositorio. Usar variables de entorno o `dotnet nuget add source` en la máquina local.

Si los paquetes están en **nuget.org**, no se necesita `nuget.config` ni autenticación.

### 7.2 Instalar el paquete

```bash
dotnet add package MotorDsl.Extensions --version 1.0.0
```

Esto agrega al `.csproj`:

```xml
<ItemGroup>
  <PackageReference Include="MotorDsl.Extensions" Version="1.0.0" />
</ItemGroup>
```

### 7.3 Uso en código

#### Aplicación .NET con DI (API, Worker, MAUI, etc.)

```csharp
// Program.cs o MauiProgram.cs
using MotorDsl.Extensions;

builder.Services.AddMotorDslEngine();
```

#### Uso directo (aplicación consola sin DI)

```csharp
using MotorDsl.Parser;
using MotorDsl.Rendering;

// Parsear una plantilla DSL
var parser = new DslParser();
var template = parser.Parse(jsonDsl);

// Renderizar a ESC/POS
var renderer = new EscPosRenderer();
byte[] output = renderer.Render(template, data);
```

### 7.4 Ejemplo mínimo funcional (consola)

```csharp
using MotorDsl.Extensions;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddMotorDslEngine();
var provider = services.BuildServiceProvider();

// Resolver servicios registrados y usar el motor
var engine = provider.GetRequiredService<IDslEngine>();
var result = engine.Process(jsonTemplate, data);
```

---

## 8. Versionado

El proyecto sigue la estrategia de versionado semántico (SemVer) definida en [estrategia-versionado_v1.0.md](estrategia-versionado_v1.0.md).

### Formato

```
MAJOR.MINOR.PATCH
```

| Cambio | Ejemplo | Cuándo |
|---|---|---|
| MAJOR | `2.0.0` | Cambios incompatibles en la API |
| MINOR | `1.1.0` | Nuevas funcionalidades, retrocompatibles |
| PATCH | `1.0.1` | Correcciones de bugs |
| Preview | `0.0.0-preview.42` | Builds automáticos en `main` (no publicados al feed) |

### ¿Cómo se determina la versión en el workflow?

| Trigger | Versión resultante | ¿Se publica al feed? |
|---|---|---|
| Tag `v1.2.3` | `1.2.3` | Sí |
| `workflow_dispatch` con input `1.2.3` | `1.2.3` | Sí |
| Push a `main` | `0.0.0-preview.<run_number>` | No (solo artifact) |

---

## 9. Troubleshooting

### 9.1 Error de autenticación en GitHub Packages

```
error: Response status code does not indicate success: 401 (Unauthorized)
```

**Solución:** verificar que el PAT tenga el scope `read:packages` (para consumir) o `write:packages` (para publicar). Regenerar si expiró.

### 9.2 Paquete no aparece en el feed

- Los paquetes pueden tardar 1-2 minutos en indexarse
- Verificar en GitHub → repositorio → **Packages** que el paquete fue publicado
- Si usó `--skip-duplicate` y la versión ya existía, no se sobrescribe (diseño intencional)

### 9.3 Versión duplicada

```
error: Conflict - The feed already contains 'MotorDsl.Core 1.0.0'
```

**Solución:** incrementar la versión. Las versiones publicadas son inmutables por diseño. Crear un nuevo tag (ej: `v1.0.1`).

### 9.4 El paquete no trae las dependencias transitivas

Verificar que los `.csproj` usen `<ProjectReference>` (no `<Reference>`). El SDK de .NET convierte automáticamente las `ProjectReference` en dependencias del paquete NuGet al hacer `dotnet pack`.

### 9.5 Error `dotnet pack` falla con `--no-build`

El paso `--no-build` requiere que se haya hecho `dotnet build -c Release` previamente. Si falla, ejecutar sin `--no-build`:

```bash
dotnet pack src/MotorDsl.Core/MotorDsl.Core.csproj -c Release -o ./nupkg
```

---

## 10. Resumen rápido

```
Publicar:
  git tag v1.0.0 && git push origin v1.0.0
  → El workflow cd-nuget.yml hace el resto

Consumir:
  dotnet add package MotorDsl.Extensions --version 1.0.0
  → En código: services.AddMotorDslEngine();
```

---

## 11. Referencias

- [pipeline-ci-cd_v1.0.md](pipeline-ci-cd_v1.0.md) — Pipeline CI/CD completo
- [estrategia-versionado_v1.0.md](estrategia-versionado_v1.0.md) — Estrategia de versionado SemVer
- [entornos-deploy_v1.0.md](entornos-deploy_v1.0.md) — Entornos de despliegue
- [Documentación oficial NuGet](https://learn.microsoft.com/en-us/nuget/) — Microsoft Docs
- [GitHub Packages con NuGet](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry) — GitHub Docs

---

## 12. Control de cambios

| Versión | Fecha | Autor | Descripción |
|---|---|---|---|
| v1.0 | 2026-04-02 | Arquitectura / DevOps | Guía inicial de publicación y consumo NuGet |
