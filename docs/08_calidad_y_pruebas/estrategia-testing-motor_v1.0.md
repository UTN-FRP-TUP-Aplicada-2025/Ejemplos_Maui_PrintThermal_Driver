# Estrategia de Testing — Motor DSL de Renderizado  
**Archivo:** estrategia-testing-motor_v1.0.md  
**Versión:** 1.0  
**Fecha:** 2026-03-28  
**Autor:** Equipo Técnico  
**Estado:** Activo  

---

## 1. Propósito

Definir la estrategia de pruebas específica para el Motor DSL de renderizado, orientada a validar:

- Parsing DSL → AST  
- Evaluación de expresiones  
- Resolución de datos  
- Layout adaptativo  
- Renderizado por targets  
- Extensibilidad del motor  
- Determinismo y consistencia de resultados  

Este documento complementa el Plan de Pruebas, enfocándose en el comportamiento interno de la librería.

---

## 2. Naturaleza del testing en un motor

A diferencia de aplicaciones tradicionales:

- No se valida UI final (salvo renderers específicos)
- Se valida transformación de entrada → salida
- Se prioriza consistencia, no interacción de usuario
- Se utilizan técnicas de comparación estructural y snapshot

---

## 3. Principios de testing

### 3.1 Determinismo

Un mismo input debe producir siempre el mismo output.

Incluye:

- DSL
- Datos de entrada
- DeviceProfile

---

### 3.2 Aislamiento

Cada componente debe poder probarse de forma independiente:

- Parser
- AST
- Layout
- Renderer
- Expresiones

---

### 3.3 Reproducibilidad

Los tests deben ser:

- Ejecutables en cualquier entorno
- Independientes del sistema operativo
- Independientes de infraestructura externa

---

### 3.4 Validación por transformación

Se testean transformaciones:

```text
DSL → AST → Layout → Render
````

No solo estados intermedios, sino outputs finales.

---

## 4. Niveles de testing

### 4.1 Unit Tests

**Objetivo:** validar componentes aislados.

**Alcance:**

* Parser DSL
* Evaluador de expresiones
* Nodos del AST
* Layout engine
* Renderers individuales

**Características:**

* Rápidos
* Sin dependencias externas
* Altamente granulares

---

### 4.2 Integration Tests

**Objetivo:** validar interacción entre componentes.

**Ejemplos:**

* Parser + AST
* AST + Layout
* Layout + Renderer
* Engine completo

---

### 4.3 Snapshot / Golden Tests

**Objetivo:** validar outputs completos.

**Qué se compara:**

* Render final (texto, UI, ESC/POS)
* Estructura generada

**Uso:**

* Detectar regresiones
* Validar cambios de comportamiento

---

### 4.4 Regression Tests

**Objetivo:** evitar que cambios rompan funcionalidades existentes.

**Incluye:**

* Casos críticos del motor
* Documentos DSL representativos
* Flujos completos

---

### 4.5 Tests de extensibilidad

**Objetivo:** validar que el motor soporta extensiones.

Incluye:

* Nuevos nodos DSL
* Nuevos renderers
* Plugins externos

---

## 5. Tipos de pruebas específicas del motor

### 5.1 Tests de parsing DSL

Validan:

* Sintaxis válida
* Manejo de errores
* Construcción correcta del AST

**Casos:**

* DSL válido → AST esperado
* DSL inválido → error controlado

---

### 5.2 Tests de AST

Validan:

* Estructura jerárquica
* Tipos de nodos
* Relaciones padre-hijo

---

### 5.3 Tests de expresiones

Validan:

* Evaluación de condiciones
* Variables
* Contexto de ejecución

---

### 5.4 Tests de layout

Validan:

* Reflow de contenido
* Adaptación a DeviceProfile
* Manejo de restricciones

---

### 5.5 Tests de renderizado

Validan:

* Output final por renderer
* Consistencia por target
* Separación de responsabilidades

---

## 6. Golden Tests (Snapshot Testing)

### Propósito

Comparar salida actual vs salida esperada.

### Flujo:

1. Input DSL + datos
2. Ejecutar motor
3. Generar output
4. Comparar con snapshot almacenado

### Uso típico:

* Documentos complejos
* Render ESC/POS
* UI previews

### Ventajas:

* Detecta regresiones visuales o estructurales
* Simplifica validación de outputs complejos

---

## 7. Datos de prueba (Fixtures)

### Tipos:

* Documentos DSL base
* Inputs de datos mock
* DeviceProfiles simulados

### Reglas:

* Reutilizables
* Versionados
* Representativos de casos reales

---

## 8. Estrategia por componente

### 8.1 Parser

* Tests de sintaxis válida/inválida
* Tests de tokens
* Tests de construcción AST

---

### 8.2 AST

* Validación estructural
* Navegación de nodos
* Integridad jerárquica

---

### 8.3 Layout Engine

* Reflow de contenido
* Adaptación a tamaños
* Casos límite (overflow, wrapping)

---

### 8.4 Renderer

* Output consistente por target
* Comparación de snapshots
* Independencia del AST

---

### 8.5 Motor completo

* Pipeline end-to-end:
  DSL → AST → Layout → Render
* Validación de determinismo

---

## 9. Estrategia de mocks y fakes

Se utilizarán para:

* IDataResolver
* Servicios externos simulados
* Renderers dummy

**Objetivo:**

Aislar el comportamiento del motor sin depender de infraestructura externa.

---

## 10. Automatización

### Obligatoria:

* Unit tests
* Integration tests básicos

### Recomendada:

* Snapshot tests
* Regression suite automatizada

### Ejecución:

* En CI/CD
* En cada PR
* En cada merge

---

## 11. Cobertura de pruebas

### Objetivo mínimo:

* ≥ 70% en módulos críticos

### Áreas críticas:

* Parser DSL
* Evaluación de expresiones
* Layout engine
* Render pipeline

---

## 12. Criterios de aceptación de pruebas

Un conjunto de pruebas se considera válido cuando:

* Es determinista
* No depende de entorno externo
* Tiene inputs claros
* Tiene outputs verificables
* No introduce flakiness

---

## 13. Gestión de regresiones

### Estrategia:

* Uso de snapshot tests
* Comparación automática de outputs
* Validación en CI

### Clasificación de fallos:

* Regresión funcional
* Regresión visual
* Regresión estructural

---

## 14. Métricas de testing

Se deben monitorear:

* Cobertura de código
* Número de tests por componente
* Tiempo de ejecución de tests
* Tests fallidos por build
* Regresiones detectadas

---

## 15. Riesgos en testing

| Riesgo                   | Impacto | Mitigación                     |
| ------------------------ | ------- | ------------------------------ |
| Flakiness en tests       | Alto    | eliminar dependencias externas |
| Snapshots inconsistentes | Medio   | versionado controlado          |
| Acoplamiento en tests    | Alto    | mocks y fakes                  |
| Baja cobertura en core   | Alto    | enforcement en CI              |
| Tests lentos             | Medio   | separación unit vs integration |

---

## 16. Integración con otros documentos

Este documento se relaciona con:

* estrategia-calidad-motor_v1.0.md
* plan de pruebas
* DoD
* backlog técnico
* arquitectura del motor
* flujo de ejecución del motor

---

## 17. Control de versiones

| Versión | Fecha      | Autor          | Descripción     |
| ------- | ---------- | -------------- | --------------- |
| 1.0     | 2026-03-28 | Equipo Técnico | Versión inicial |

---
