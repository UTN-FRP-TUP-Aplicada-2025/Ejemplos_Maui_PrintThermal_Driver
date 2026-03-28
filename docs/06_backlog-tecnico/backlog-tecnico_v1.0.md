# Backlog Técnico — Motor DSL de Renderizado  
**Archivo:** backlog-tecnico_v1.0.md  
**Versión:** 1.0  
**Fecha:** 2026-03-28  
**Autor:** Equipo Técnico  
**Estado:** Activo  

---

## 1. Propósito

Este documento descompone la arquitectura y contratos del Motor DSL de renderizado en tareas técnicas implementables.

El backlog está orientado a la construcción de una librería extensible en .NET, enfocada en:

- Parsing de DSL  
- Construcción de un AST (DocumentModel)  
- Evaluación de expresiones  
- Motor de layout  
- Renderizado desacoplado por targets  
- Extensibilidad mediante renderers y plugins  
- Integración vía Dependency Injection  

---

## 2. Convenciones

- **ID**: identificador único de la tarea  
- **Tipo**: Infraestructura / Core / Parser / Render / Testing / Extensibilidad  
- **Prioridad**: Alta / Media / Baja  
- **Fuente**: documento origen (arquitectura, contratos, etc.)  
- **Estado inicial**: Pendiente  

---

# ÉPICA 1 — Fundaciones del motor

## OBJETIVO

Crear la base del proyecto y estructura de librería.

---

### BT-001 — Crear solución base

- **Tipo:** Infraestructura  
- **Prioridad:** Alta  
- **Fuente:** arquitectura-solucion_v1.0.md  

**Descripción**

Crear solución .NET 10 con estructura modular:

- MotorDsl.Core  
- MotorDsl.Parser  
- MotorDsl.Rendering  
- MotorDsl.Layout  
- MotorDsl.Abstractions  
- MotorDsl.Tests  

**Criterios de aceptación**

- La solución compila  
- Referencias entre proyectos correctas  
- Separación por responsabilidades  

---

### BT-002 — Configurar DI y bootstrap del motor

- **Tipo:** Infraestructura  
- **Prioridad:** Alta  
- **Fuente:** guia-uso-libreria_v1.0.md  

**Descripción**

Implementar extensión:

```csharp
services.AddMotorDslEngine();
````

**Criterios**

* Registra IDocumentEngine
* Registra renderers base
* Registra parser

---

### BT-003 — Definir contratos base en Abstractions

* **Tipo:** Core
* **Prioridad:** Alta
* **Fuente:** contratos-del-motor_v1.0.md

**Interfaces clave:**

* IDocumentEngine
* IDslParser
* IRenderer
* ILayoutEngine
* IDataResolver
* IDeviceProfileProvider

---

# ÉPICA 2 — Modelo de documento (AST)

## OBJETIVO

Implementar el modelo lógico del documento.

---

### BT-010 — Implementar DocumentTemplate

* **Tipo:** Core
* **Prioridad:** Alta
* **Fuente:** modelo-datos-logico_v1.0.md

---

### BT-011 — Implementar DocumentNode base

* **Tipo:** Core
* **Prioridad:** Alta

---

### BT-012 — Implementar nodos básicos

* **Tipo:** Core
* **Prioridad:** Alta

**Incluye:**

* TextNode
* ContainerNode
* ConditionalNode
* LoopNode

---

### BT-013 — Implementar sistema de propiedades dinámicas

* **Tipo:** Core
* **Prioridad:** Media

---

### BT-014 — Validación del AST

* **Tipo:** Core
* **Prioridad:** Alta

**Incluye:**

* Árbol sin ciclos
* Nodos válidos
* Integridad estructural

---

# ÉPICA 3 — Parser DSL

## OBJETIVO

Convertir DSL → AST.

---

### BT-020 — Implementar IDslParser

* **Tipo:** Parser
* **Prioridad:** Alta
* **Fuente:** contratos-del-motor

---

### BT-021 — Definir gramática DSL base

* **Tipo:** Parser
* **Prioridad:** Alta

**Incluye:**

* Containers
* Text nodes
* Expressions
* Condiciones
* Iteraciones

---

### BT-022 — Implementar tokenizer / lexer

* **Tipo:** Parser
* **Prioridad:** Alta

---

### BT-023 — Implementar parser sintáctico

* **Tipo:** Parser
* **Prioridad:** Alta

---

### BT-024 — Mapear DSL → DocumentNode

* **Tipo:** Parser
* **Prioridad:** Alta

---

### BT-025 — Manejo de errores de parsing

* **Tipo:** Parser
* **Prioridad:** Media

---

# ÉPICA 4 — Resolución de datos

## OBJETIVO

Integrar datos externos al AST.

---

### BT-030 — Implementar IDataResolver

* **Tipo:** Core
* **Prioridad:** Alta

---

### BT-031 — Implementar resolución por reflection

* **Tipo:** Core
* **Prioridad:** Alta

---

### BT-032 — Soporte de paths anidados

* **Tipo:** Core
* **Prioridad:** Media

---

### BT-033 — Binding de datos en TextNode

* **Tipo:** Core
* **Prioridad:** Alta

---

# ÉPICA 5 — Evaluación de expresiones

## OBJETIVO

Evaluar lógica dinámica del DSL.

---

### BT-040 — Motor de expresiones básico

* **Tipo:** Core
* **Prioridad:** Alta

---

### BT-041 — Evaluación de condiciones

* **Tipo:** Core
* **Prioridad:** Alta

---

### BT-042 — Evaluación en LoopNode

* **Tipo:** Core
* **Prioridad:** Alta

---

### BT-043 — Contexto de ejecución

* **Tipo:** Core
* **Prioridad:** Alta

---

# ÉPICA 6 — Layout Engine

## OBJETIVO

Ajustar estructura según DeviceProfile.

---

### BT-050 — Implementar ILayoutEngine

* **Tipo:** Core
* **Prioridad:** Alta

---

### BT-051 — Layout básico vertical/horizontal

* **Tipo:** Core
* **Prioridad:** Alta

---

### BT-052 — Adaptación a ancho de dispositivo

* **Tipo:** Core
* **Prioridad:** Alta

---

### BT-053 — Reflow de contenido

* **Tipo:** Core
* **Prioridad:** Media

---

# ÉPICA 7 — Renderizado

## OBJETIVO

Transformar AST en output final.

---

### BT-060 — Implementar IRenderer base

* **Tipo:** Render
* **Prioridad:** Alta

---

### BT-061 — Implementar Renderer de texto

* **Tipo:** Render
* **Prioridad:** Alta

---

### BT-062 — Implementar Renderer UI

* **Tipo:** Render
* **Prioridad:** Media

---

### BT-063 — Implementar Renderer ESC/POS

* **Tipo:** Render
* **Prioridad:** Media

---

### BT-064 — Selección de renderer por DeviceProfile

* **Tipo:** Render
* **Prioridad:** Alta

---

### BT-065 — Construcción de RenderResult

* **Tipo:** Render
* **Prioridad:** Alta

---

# ÉPICA 8 — Orquestación del motor

## OBJETIVO

Implementar el pipeline completo.

---

### BT-070 — Implementar IDocumentEngine

* **Tipo:** Core
* **Prioridad:** Alta

---

### BT-071 — Integrar parsing + layout + render

* **Tipo:** Core
* **Prioridad:** Alta

---

### BT-072 — Manejo de errores global

* **Tipo:** Core
* **Prioridad:** Alta

---

### BT-073 — Logging del pipeline

* **Tipo:** Infraestructura
* **Prioridad:** Media

---

# ÉPICA 9 — Extensibilidad

## OBJETIVO

Permitir ampliación del motor.

---

### BT-080 — Registro de renderers dinámicos

* **Tipo:** Extensibilidad
* **Prioridad:** Alta

---

### BT-081 — Soporte de nuevos nodos DSL

* **Tipo:** Extensibilidad
* **Prioridad:** Alta

---

### BT-082 — Plugins por assembly scanning

* **Tipo:** Extensibilidad
* **Prioridad:** Media

---

### BT-083 — Registro de extensiones vía DI

* **Tipo:** Extensibilidad
* **Prioridad:** Alta

---

# ÉPICA 10 — Testing

## OBJETIVO

Validar comportamiento del motor.

---

### BT-090 — Unit tests AST

* **Tipo:** Testing
* **Prioridad:** Alta

---

### BT-091 — Unit tests parser DSL

* **Tipo:** Testing
* **Prioridad:** Alta

---

### BT-092 — Unit tests expresiones

* **Tipo:** Testing
* **Prioridad:** Alta

---

### BT-093 — Tests de renderizado por target

* **Tipo:** Testing
* **Prioridad:** Alta

---

### BT-094 — Tests de integración del pipeline

* **Tipo:** Testing
* **Prioridad:** Alta

---

# ÉPICA 11 — Performance y optimización

## OBJETIVO

Asegurar eficiencia del motor.

---

### BT-100 — Cache de templates parseados

* **Tipo:** Infraestructura
* **Prioridad:** Media

---

### BT-101 — Optimización de evaluación de expresiones

* **Tipo:** Core
* **Prioridad:** Media

---

### BT-102 — Minimizar recorridos del AST

* **Tipo:** Core
* **Prioridad:** Media

---

# 🧭 Orden recomendado de ejecución

```text
1️⃣ Épica 1 — Fundaciones  
2️⃣ Épica 2 — AST  
3️⃣ Épica 3 — Parser  
4️⃣ Épica 4 — Datos  
5️⃣ Épica 5 — Expresiones  
6️⃣ Épica 6 — Layout  
7️⃣ Épica 7 — Render  
8️⃣ Épica 8 — Orquestación  
9️⃣ Épica 9 — Extensibilidad  
🔟 Épica 10 — Testing  
1️⃣1️⃣ Épica 11 — Performance  
```

---

# 10. Definición de Done (DoD)

Una tarea se considera completa cuando:

* Compila correctamente
* Tiene tests asociados (si aplica)
* Cumple contratos definidos
* Está integrada en el pipeline
* No rompe compatibilidad existente
* Pasa revisión técnica

---

# 11. Historial de versiones

| Versión | Fecha      | Autor          | Cambios         |
| ------- | ---------- | -------------- | --------------- |
| 1.0     | 2026-03-28 | Equipo Técnico | Backlog inicial |

---
