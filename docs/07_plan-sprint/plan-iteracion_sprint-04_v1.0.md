# 📄 Plan de Iteración — Sprint 04
**Archivo:** plan-iteracion_sprint-04_v1.0.md
**Versión:** 1.0  
**Fecha:** 2026-03-28

## 📋 Información general

* Proyecto: Motor DSL para generación y renderizado de documentos (ESC/POS / PDF / Preview)
* Sprint: 04
* Duración: 2 semanas
* Fecha inicio: 13/05/2026
* Fecha fin: 26/05/2026
* Objetivo del sprint: Incorporar extensibilidad, configuración avanzada y soporte de perfiles/renderers, dejando el motor listo para uso productivo y escalable.

---

## 🎯 Objetivo del Sprint

Evolucionar el motor hacia una arquitectura extensible y configurable, permitiendo:

* Registro dinámico de renderers
* Soporte de perfiles de ejecución (device/config)
* Inyección de dependencias / composición de componentes
* Preparación para plugins o extensiones
* Validaciones estructurales del DSL y AST
* Mejor manejo de errores en parsing y evaluación

👉 Este sprint convierte el motor en una base **realmente reutilizable y escalable**.

---

## 🧩 Historias / tareas comprometidas

| ID    | Tipo     | Descripción                                                  | Prioridad | Estimación |
| ----- | -------- | ------------------------------------------------------------ | --------- | ---------- |
| US-12 | Historia | Implementar sistema de renderers intercambiables             | Alta      | 8 pts      |
| US-13 | Historia | Soporte de perfiles de ejecución (RenderProfile)             | Alta      | 8 pts      |
| US-14 | Historia | Mejorar manejo de errores en parser y evaluator              | Alta      | 5 pts      |
| US-15 | Historia | Implementar sistema básico de extensibilidad (plugins/hooks) | Media     | 13 pts     |
| TK-19 | Técnica  | Implementar RenderEngine centralizador                       | Alta      | 5 pts      |
| TK-20 | Técnica  | Implementar registro de componentes (RendererRegistry)       | Alta      | 5 pts      |
| TK-21 | Técnica  | Definir RenderProfile y configuración por entorno            | Alta      | 5 pts      |
| TK-22 | Técnica  | Mejorar excepciones y logging del motor                      | Media     | 5 pts      |
| TK-23 | Técnica  | Crear validaciones de AST previo a evaluación                | Media     | 5 pts      |
| TK-24 | Técnica  | Tests de integración end-to-end                              | Alta      | 8 pts      |

---

## 🏗️ Alcance técnico del sprint

### Extensibilidad del motor

* Registro dinámico de renderers
* Selección de renderer según perfil
* Arquitectura preparada para plugins

### Configuración

* RenderProfile:

  * Tipo de salida (Text, ESC/POS, PDF)
  * Parámetros específicos por entorno
  * Configuración de ejecución

### Orquestación

* RenderEngine como punto central:

  * Orquesta Parser → Evaluator → Renderer
  * Permite ejecución completa del pipeline

---

## 🔁 Pipeline final del motor

```text id="pipeline_final"
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
RenderEngine
 ↓
Renderer (según perfil)
 ↓
Output final
```

---

## ⚙️ Capacidades habilitadas

* Selección dinámica de renderer
* Configuración por entorno (dev/test/prod)
* Validación previa de estructuras DSL
* Manejo centralizado de errores
* Preparación para múltiples outputs simultáneos
* Base para integración con sistemas externos

---

## 🚫 Fuera de alcance

* Implementación completa de plugins externos dinámicos
* UI visual avanzada
* Integración con hardware real (solo base lógica)
* Optimización avanzada de performance
* Distribución como SDK público

---

## ✅ Criterios de terminado (Definition of Done)

Una tarea se considera terminada cuando:

* Existe un RenderEngine central funcional
* Se pueden seleccionar renderers dinámicamente
* RenderProfile influye en la ejecución
* El motor ejecuta el pipeline completo de forma consistente
* Existen validaciones antes de evaluar/renderizar
* Manejo de errores robusto en parser/evaluator
* Tests end-to-end cubren:

  * DSL completo → output final
  * Diferentes perfiles de render
  * Selección de renderer
* El sistema es extensible sin modificar componentes existentes

---

## 🧪 Plan de pruebas del sprint

### Pruebas unitarias

* RenderEngine
* RendererRegistry
* RenderProfile
* Validaciones de AST
* Manejo de excepciones

### Pruebas de integración (end-to-end)

* DSL → Output final completo
* Diferentes renderers seleccionados dinámicamente
* Evaluación + rendering con perfiles distintos

---

## 🚀 Entregables del sprint

* RenderEngine implementado
* Sistema de registro de renderers
* RenderProfile definido
* Validaciones de AST y errores
* Manejo centralizado de ejecución
* Tests end-to-end
* Motor listo para uso productivo

---

## ⚠️ Riesgos identificados

* Complejidad en selección dinámica de renderers
* Acoplamiento accidental entre capas
* Definición incompleta de perfiles
* Manejo inconsistente de errores
* Dificultad en mantener extensibilidad sin sobreingeniería

---

## 🔄 Métricas de seguimiento

* % de pipelines ejecutados end-to-end correctamente
* Tiempo de ejecución del pipeline
* Cobertura de tests end-to-end
* Errores detectados en validación previa
* Capacidad de cambiar renderer sin impacto en lógica core

---

## 🧠 Resultado esperado al finalizar el Sprint 04

El motor quedará en un estado donde:

* Tiene pipeline completo funcional
* Es configurable por perfiles
* Es extensible por diseño
* Permite múltiples renderers
* Tiene manejo robusto de errores
* Está listo para evolucionar a producción o SDK reutilizable

---
