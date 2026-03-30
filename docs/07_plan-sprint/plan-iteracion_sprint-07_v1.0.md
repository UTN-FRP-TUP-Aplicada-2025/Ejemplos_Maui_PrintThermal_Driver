# 📄 Plan de Iteración — Sprint 07
**Archivo:** plan-iteracion_sprint-07_v1.0.md  
**Versión:** 1.0  
**Fecha:** 2026-06-24

## 📋 Información general

* Proyecto: Motor DSL para generación y renderizado de documentos (ESC/POS / PDF / Preview)
* Sprint: 07
* Duración: 2 semanas
* Fecha inicio: 24/06/2026
* Fecha fin: 07/07/2026
* Objetivo del sprint: Validación formal de plantillas DSL y perfiles de dispositivo, error reporting estructurado con severidad y ubicación.

---

## 🎯 Objetivo del Sprint

Consolidar la **capa de validación del motor** para garantizar robustez antes de ejecutar el pipeline, permitiendo:

* Validar plantillas DSL formalmente antes de procesar (esquema, tipos de nodo, campos requeridos)
* Validar perfiles de dispositivo antes del layout y rendering
* Clasificar errores por severidad (Error/Warning) con ubicación precisa
* Propagar warnings sin detener el pipeline

👉 Este sprint habilita **CU-14** (Validar plantilla DSL), **CU-22** (Validar perfil de dispositivo), mejora **CU-30** (Error reporting plantilla) y **CU-31** (Error reporting datos).

---

## 🧩 Historias / tareas comprometidas

| ID    | Tipo     | Descripción                                                                          | Prioridad | Estimación |
| ----- | -------- | ------------------------------------------------------------------------------------ | --------- | ---------- |
| US-25 | Historia | Como desarrollador, quiero validar plantillas DSL formalmente para detectar errores de esquema antes de procesar | Alta      | 13 pts     |
| US-26 | Historia | Como desarrollador, quiero validar perfiles de dispositivo antes de usarlos en layout/render | Alta      | 7 pts      |
| US-27 | Historia | Como desarrollador, quiero errores con severidad y ubicación para diagnosticar problemas rápido | Alta      | 8 pts      |
| US-28 | Historia | Como desarrollador, quiero que los warnings se propaguen sin detener el pipeline       | Media     | 3 pts      |
| TK-48 | Técnica  | Extender `ValidationError` con `Severity` (enum Error/Warning, default=Error) y `Location` (string?, default=null) — backward compatible | Alta      | 3 pts      |
| TK-49 | Técnica  | Definir contrato `ITemplateValidator` en `MotorDsl.Core/Contracts`                   | Alta      | 2 pts      |
| TK-50 | Técnica  | Implementar `TemplateValidator`: JSON syntax, schema, tipos de nodo, propiedades obligatorias por tipo, jerarquía | Alta      | 8 pts      |
| TK-51 | Técnica  | Integrar `ITemplateValidator` en pipeline de `DocumentEngine` (validar post-parse, pre-evaluate) | Alta      | 3 pts      |
| TK-52 | Técnica  | Definir contrato `IProfileValidator` en `MotorDsl.Core/Contracts`                    | Alta      | 2 pts      |
| TK-53 | Técnica  | Implementar `ProfileValidator`: Width > 0, RenderTarget no vacío, Name no vacío      | Alta      | 5 pts      |
| TK-54 | Técnica  | Integrar `IProfileValidator` en pipeline de `DocumentEngine` (validar antes de Layout) | Alta      | 3 pts      |
| TK-55 | Técnica  | Extender `IDataValidator` con soporte de Severity: campos faltantes = Error, valores null = Warning | Alta      | 5 pts      |
| TK-56 | Técnica  | Propagar warnings a `RenderResult.Warnings` en `DocumentEngine` sin detener pipeline | Media     | 3 pts      |
| TK-57 | Técnica  | Tests unitarios: TemplateValidator, ProfileValidator, Severity en ValidationError     | Alta      | 8 pts      |
| TK-58 | Técnica  | Tests de integración Sprint 07: pipeline completo con validaciones                   | Alta      | 5 pts      |

**Total estimado: 47 story points**

---

## 🏗️ Alcance técnico del sprint

### TK-48 — Extender ValidationError con Severity y Location

* Nuevo enum `ValidationSeverity` en `MotorDsl.Core/Models`:

```csharp
public enum ValidationSeverity
{
    Error,    // Detiene el pipeline
    Warning   // Se propaga pero no detiene
}
```

* Propiedades opcionales agregadas a `ValidationError`:

```csharp
public class ValidationError
{
    // Existentes (no cambian)
    public string Field { get; }
    public ValidationErrorType Type { get; }
    public string Message { get; }
    public string NodeType { get; }

    // Nuevas — backward compatible
    public ValidationSeverity Severity { get; init; } = ValidationSeverity.Error;
    public string? Location { get; init; } = null;
}
```

* El constructor existente de 4 parámetros no cambia. Las nuevas propiedades se setean opcionalmente via `init`.
* Nuevos `ValidationErrorType` agregados: `InvalidSyntax`, `MissingRequiredField`, `UnknownNodeType`.

### TK-49 + TK-50 — ITemplateValidator + TemplateValidator (CU-14)

* Nuevo contrato en `MotorDsl.Core/Contracts`:

```csharp
public interface ITemplateValidator
{
    ValidationResult ValidateTemplate(string dslJson);
}
```

* Implementación `TemplateValidator` en `MotorDsl.Core/Validation`:
  1. **JSON syntax** — verifica que sea JSON válido → `InvalidSyntax`
  2. **Schema** — verifica presencia de `id`, `version`, `root` → `MissingRequiredField`
  3. **Tipos de nodo** — verifica que `type` sea uno de: `text`, `container`, `conditional`, `loop`, `table`, `image` → `UnknownNodeType`
  4. **Propiedades obligatorias por tipo**:
     * `TextNode`: `text` presente
     * `LoopNode`: `source`, `itemAlias`, `body` presentes
     * `ConditionalNode`: `expression`, `trueBranch` presentes
     * `ContainerNode`: `children` presente
     * `TableNode`: (ninguna obligatoria, headers/rows opcionales)
     * `ImageNode`: `source` presente
  5. **Jerarquía** — verifica recursivamente todos los nodos hijos
  6. Cada error incluye `Location` con path JSON: `"root.children[2].body"`

### TK-51 — Integración ITemplateValidator en DocumentEngine

* `DocumentEngine` recibe `ITemplateValidator?` como parámetro opcional del constructor
* Se ejecuta después del parse y antes de la validación de datos:

```text
Parse → ValidateTemplate (si ITemplateValidator presente) → ValidateData → Evaluate → Layout → Render
```

* Si hay errores de Severity=Error → short-circuit con errores en RenderResult
* Warnings se acumulan y propagan

### TK-52 + TK-53 — IProfileValidator + ProfileValidator (CU-22)

* Nuevo contrato en `MotorDsl.Core/Contracts`:

```csharp
public interface IProfileValidator
{
    ValidationResult ValidateProfile(DeviceProfile profile);
}
```

* Implementación `ProfileValidator` en `MotorDsl.Core/Validation`:
  * `Name` no vacío → `MissingRequiredField`
  * `Width > 0` → `InvalidStructure` (con mensaje descriptivo)
  * `RenderTarget` no vacío → `MissingRequiredField`
  * Cada error incluye `Location` como nombre de propiedad: `"DeviceProfile.Width"`

### TK-54 — Integración IProfileValidator en DocumentEngine

* `DocumentEngine` recibe `IProfileValidator?` como parámetro opcional
* Se ejecuta antes del Layout:

```text
Parse → ValidateTemplate → ValidateData → Evaluate → ValidateProfile → Layout → Render
```

### TK-55 — Extender IDataValidator con Severity (CU-31)

* `DataValidator` asigna Severity según tipo de error:
  * `MissingField` → `Severity = Error` (campo referenciado no existe)
  * `TypeMismatch` → `Severity = Error` (loop source no es IEnumerable)
  * `InvalidStructure` → `Severity = Error` (nodo malformado)
  * Valor null en campo existente → `Severity = Warning` (nuevo caso)
* Agregar `Location` con path del nodo que referencia al campo: `"LoopNode[items].body.TextNode"`

### TK-56 — Propagar Warnings en DocumentEngine (CU-30 + CU-31)

* `RenderResult` ya tiene propiedad `Warnings` (lista de strings) — verificar que exista, crear si no
* `DocumentEngine` al procesar `ValidationResult`:
  * Errores con `Severity = Error` → short-circuit
  * Errores con `Severity = Warning` → se agregan a `RenderResult.Warnings`, el pipeline continúa
* Se aplica a las 3 validaciones: Template, Data y Profile

### TK-57 — Tests unitarios

* `TemplateValidatorTests.cs` (~8 tests):
  * JSON inválido → InvalidSyntax
  * Falta `id` / `version` / `root` → MissingRequiredField
  * Tipo de nodo desconocido → UnknownNodeType
  * LoopNode sin `source` → MissingRequiredField
  * Template válida → IsValid
  * Location incluida en errores
  * Severity por defecto = Error

* `ProfileValidatorTests.cs` (~5 tests):
  * Width = 0 → error
  * RenderTarget vacío → error
  * Name vacío → error
  * Perfil válido → IsValid
  * Location incluida

* `ValidationSeverityTests.cs` (~3 tests):
  * ValidationError default Severity = Error
  * Severity = Warning no detiene pipeline
  * Warnings propagados a RenderResult

### TK-58 — Tests de integración Sprint 07

* `Sprint07IntegrationTests.cs` (~5 tests):
  * Pipeline con plantilla inválida → errores sin llegar a rendering
  * Pipeline con perfil inválido → errores sin llegar a layout
  * Pipeline con warnings → render exitoso + warnings propagados
  * Pipeline completo con todas las validaciones OK → resultado limpio
  * Todas las validaciones reportan Location

---

## 🔁 Pipeline completo con este sprint

```text
DSL JSON
 ↓
Parser (Sprint 01)
 ↓
DocumentTemplate
 ↓
╔══════════════════════════════╗
║ ITemplateValidator (Sprint 07)║  ←── NUEVO: validación formal
║ Schema + nodos + propiedades  ║      de la plantilla DSL
╚══════════════════════════════╝
 ↓ (si válido)
╔══════════════════════════════╗
║ IDataValidator (Sprint 06+07) ║  ←── MEJORADO: ahora con
║ Severity + Location           ║      Error/Warning + ubicación
╚══════════════════════════════╝
 ↓ (si no hay errores críticos)
Evaluator (Sprint 02)
 ↓
EvaluatedDocument
 ↓
╔══════════════════════════════╗
║ IProfileValidator (Sprint 07) ║  ←── NUEVO: valida perfil
║ Width + RenderTarget + Name   ║      antes del layout
╚══════════════════════════════╝
 ↓ (si válido)
LayoutEngine (Sprint 03)
 ↓
LayoutedDocument
 ↓
┌──────────────────────────────┐
│ RendererRegistry (Sprint 03)  │
│  ├── TextRenderer             │
│  ├── EscPosRenderer + QR + BC │
│  └── (extensible)             │
└──────────────────────────────┘
 ↓
RenderResult (output + errors + warnings)
```

---

## 📊 Métricas del Sprint

| Métrica               | Objetivo    |
| --------------------- | ----------- |
| Tests acumulados      | 154 → ~175  |
| Cobertura funcional   | CU-14, CU-22, CU-30 (parcial), CU-31 (parcial) |
| Story points          | 47 pts      |
| Archivos nuevos       | ~8 (contracts, impl, tests) |
| Archivos modificados  | ~5 (ValidationError, DocumentEngine, DI, etc.) |

---

## 📐 Criterios de aceptación del sprint

1. `ITemplateValidator` valida JSON syntax, schema, tipos de nodo y propiedades obligatorias
2. `IProfileValidator` valida Width > 0, RenderTarget y Name no vacíos
3. `ValidationError` tiene `Severity` y `Location` opcionales — backward compatible
4. Errores con Severity=Error detienen el pipeline
5. Warnings con Severity=Warning se propagan sin detener
6. `DocumentEngine` integra las 3 validaciones en el pipeline
7. ~21 tests nuevos (16 unitarios + 5 integración)
8. Build limpio + todos los tests pasando
9. Committed y pushed

---

## 📝 Dependencias entre tareas

```text
TK-48 (modelos)
 ├── TK-49 → TK-50 → TK-51 (Template validation)
 ├── TK-52 → TK-53 → TK-54 (Profile validation)
 ├── TK-55 (Data validator severity)
 └── TK-56 (Warnings propagation)
      └── TK-57 + TK-58 (Tests)
```

* TK-48 es prerequisito de todo — extiende los modelos base
* TK-49/50/51 y TK-52/53/54 pueden hacerse en paralelo después de TK-48
* TK-55 y TK-56 pueden hacerse en paralelo con los bloques de validación
* TK-57 y TK-58 son los últimos — verifican todo

---

## 📦 Fuera de alcance (deferred)

* CU-09 — Renderizar PDF (requiere dependencia externa)
* CU-23 completo — Validar capabilities específicas por renderer
* Validación de rango avanzada para datos (reglas externas)
* BT-073 — Logging del pipeline
* BT-100/101/102 — Performance y cache de templates

---

## 🗓️ Historial de versiones

| Versión | Fecha      | Autor          | Cambios              |
| ------- | ---------- | -------------- | -------------------- |
| 1.0     | 2026-06-24 | Equipo Técnico | Plan Sprint 07 inicial |
