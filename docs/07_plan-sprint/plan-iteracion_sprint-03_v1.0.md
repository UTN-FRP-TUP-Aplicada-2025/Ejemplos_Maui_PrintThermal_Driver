# 📄 Plan de Iteración — Sprint 03
**Archivo:** plan-iteracion_sprint-03_v1.0.md
**Versión:** 1.0  
**Fecha:** 2026-03-28

## 📋 Información general

* Proyecto: Motor DSL para generación y renderizado de documentos (ESC/POS / PDF / Preview)
* Sprint: 03
* Duración: 2 semanas
* Fecha inicio: 29/04/2026
* Fecha fin: 12/05/2026
* Objetivo del sprint: Implementar la capa de renderizado inicial, permitiendo transformar un documento evaluado en una salida concreta (primer target: texto estructurado y base para ESC/POS).

---

## 🎯 Objetivo del Sprint

Implementar el **Rendering Engine inicial**, habilitando:

* Transformación de un EvaluatedDocument a una salida renderizada
* Implementación de un renderer base
* Generación de salida en formato texto (debug / preview)
* Estructura preparada para múltiples renderizadores (ESC/POS, PDF, UI)

👉 Este sprint introduce la **última etapa del pipeline**.

---

## 🧩 Historias / tareas comprometidas

| ID    | Tipo     | Descripción                                           | Prioridad | Estimación |
| ----- | -------- | ----------------------------------------------------- | --------- | ---------- |
| US-09 | Historia | Renderizar EvaluatedDocument a salida textual         | Alta      | 8 pts      |
| US-10 | Historia | Implementar arquitectura de renderers intercambiables | Alta      | 13 pts     |
| US-11 | Historia | Preparar base para render ESC/POS                     | Alta      | 8 pts      |
| TK-13 | Técnica  | Definir interfaz IRenderer                            | Alta      | 3 pts      |
| TK-14 | Técnica  | Implementar TextRenderer (debug/preview)              | Alta      | 5 pts      |
| TK-15 | Técnica  | Implementar estructura de RenderContext               | Alta      | 5 pts      |
| TK-16 | Técnica  | Integrar renderer con EvaluatedDocument               | Alta      | 5 pts      |
| TK-17 | Técnica  | Crear base para ESC/POS renderer                      | Media     | 8 pts      |
| TK-18 | Técnica  | Tests unitarios de rendering                          | Media     | 5 pts      |

---

## 🏗️ Alcance técnico del sprint

### Rendering Engine

* Implementación de:

  * IRenderer
  * RenderContext
* Renderizado de:

  * Texto plano
  * Estructura jerárquica del documento evaluado
* Conversión de EvaluatedDocument → string output

---

## 🔁 Flujo completo del sistema (pipeline final parcial)

```text
DSL
 ↓
Parser
 ↓
AST
 ↓
Evaluator
 ↓
EvaluatedDocument
 ↓
Renderer
 ↓
Output (texto / preview / base ESC/POS)
```

---

## 🎨 Capacidades habilitadas

* Visualización del documento evaluado en formato legible
* Debugging del motor mediante output textual
* Base técnica para futuros renderers:

  * ESC/POS (impresión térmica)
  * PDF
  * UI preview

---

## 🧱 Diseño del rendering

### Principios adoptados

* Renderers desacoplados
* Interfaz común para múltiples outputs
* RenderContext como entrada de configuración
* Independencia del evaluator

---

## 🚫 Fuera de alcance

* Render ESC/POS completo (solo base inicial)
* Render PDF avanzado
* Layout complejo (alineaciones, estilos avanzados)
* Integración con dispositivos físicos
* Optimización de performance de rendering
* Plugins dinámicos de renderers

---

## ✅ Criterios de terminado (Definition of Done)

Una tarea se considera terminada cuando:

* Se puede renderizar un EvaluatedDocument a string
* El renderer respeta la estructura jerárquica
* Existe una interfaz IRenderer implementada
* Se puede intercambiar renderer sin modificar el evaluator
* Tests unitarios cubren:

  * Render de texto simple
  * Render de estructuras anidadas
  * Render de listas/loops evaluados
* El output es consistente y reproducible

---

## 🧪 Plan de pruebas del sprint

### Pruebas unitarias

* Render de nodos simples (TextNode)
* Render de documentos completos
* Render de estructuras jerárquicas
* Validación de salida textual

### Pruebas de integración

* DSL → AST → EvaluatedDocument → Rendered output
* Validación de pipeline completo en modo preview

Ejemplo:

* Entrada DSL con variables y loops
* Evaluación previa
* Render final en texto estructurado

---

## 🚀 Entregables del sprint

* Interfaz IRenderer definida
* Implementación de TextRenderer
* RenderContext implementado
* Integración completa con EvaluatedDocument
* Pipeline funcional completo:

  * DSL → AST → Evaluación → Render
* Tests unitarios de rendering
* Base preparada para render ESC/POS

---

## ⚠️ Riesgos identificados

* Representación inconsistente entre EvaluatedDocument y renderer
* Dificultad en representar estructuras complejas en texto plano
* Acoplamiento accidental entre evaluator y renderer
* Ambigüedad en cómo mapear nodos a salida visual
* Preparación insuficiente para ESC/POS

---

## 🔄 Métricas de seguimiento

* % de documentos evaluados correctamente renderizados
* Cobertura de tests del renderer
* Consistencia del output
* Casos soportados (texto, jerarquía, loops)
* Bugs en transformación EvaluatedDocument → output

---

## 🧠 Resultado esperado al finalizar el Sprint 03

El motor será capaz de:

* Interpretar un DSL
* Construir un AST
* Evaluar el documento con datos reales
* Renderizar el resultado en formato textual

👉 A partir de este sprint, el motor ya tiene un **pipeline completo funcional**, listo para evolucionar hacia:

* ESC/POS real
* PDF rendering
* UI preview avanzada
* Extensibilidad de renderers

---
