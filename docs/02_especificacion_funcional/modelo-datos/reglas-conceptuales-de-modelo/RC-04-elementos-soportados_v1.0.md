# Regla Conceptual: Elementos Soportados por el Motor

**Código:** RC-04
**Archivo:** RC-04-elementos-soportados_v1.0.md
**Versión:** 1.0
**Estado:** Aprobada
**Fecha:** 2026-03-28
**Autor:** Equipo Funcional / Arquitectura

---

# 1. Descripción

Esta regla conceptual establece que todos los elementos utilizados dentro de un documento o plantilla deben ser compatibles con el motor DSL, es decir, deben estar definidos, implementados y reconocidos por el sistema.

El objetivo es garantizar que el motor pueda interpretar, procesar y renderizar correctamente cada elemento sin ambigüedades ni comportamientos no definidos.

---

# 2. Motivación

El uso de elementos no soportados genera:

* errores en tiempo de validación o ejecución
* fallos en el renderizado
* inconsistencias en la salida final del documento

Al restringir los elementos a un conjunto conocido por el motor, se asegura:

* estabilidad del sistema
* previsibilidad en la ejecución
* facilidad de mantenimiento y extensión controlada

---

# 3. Definición de la Regla

* Todo elemento presente en una plantilla debe estar registrado como tipo soportado por el motor.
* El motor debe poder reconocer y procesar cada elemento sin requerir lógica adicional no definida.
* Los elementos no soportados deben ser rechazados o manejados como error según la política del sistema.
* La lista de elementos soportados debe estar alineada con el esquema DSL vigente.

---

# 4. Condiciones de Aplicación

* Aplica durante la validación de plantillas (CU-14).
* Aplica durante la ejecución del motor DSL (CU-04).
* Aplica en procesos de extensión del sistema (CU-29).
* Aplica a todos los documentos y plantillas sin excepción.

---

# 5. Resultados Esperados

* Todos los elementos del documento son interpretados correctamente por el motor.
* No existen elementos desconocidos en la estructura procesada.
* El renderizado final se ejecuta sin errores relacionados con tipos de elementos.
* El sistema mantiene consistencia entre DSL, motor y renderizadores.

---

# 6. Excepciones

## EX-01 Elemento no soportado

Si un documento contiene un elemento no reconocido por el motor:

* el sistema debe rechazar la plantilla o documento
* debe registrar el error de validación
* no se debe continuar con el procesamiento

---

## EX-02 Modo extensible controlado

Si el motor permite extensiones:

* los nuevos elementos deben registrarse explícitamente
* deben incluir implementación en el motor
* deben contar con soporte en los renderizadores correspondientes

---

# 7. Criterios de Validación

## CV-01 Reconocimiento de elementos

**Dado** una plantilla DSL
**Cuando** se valida
**Entonces** todos los elementos están definidos en el motor

---

## CV-02 Rechazo de elementos desconocidos

**Dado** un elemento no soportado
**Cuando** se procesa la plantilla
**Entonces** el sistema genera un error y detiene la ejecución

---

## CV-03 Compatibilidad con motor

**Dado** un conjunto de elementos válidos
**Cuando** el motor los procesa
**Entonces** cada elemento es interpretado correctamente

---

## CV-04 Consistencia con esquema DSL

**Dado** una plantilla válida
**Cuando** se valida contra el esquema DSL
**Entonces** todos los elementos coinciden con los definidos por el motor

---

# 8. Impacto

Esta regla impacta directamente en:

* CU-13 Cargar plantilla DSL
* CU-14 Validar plantilla
* CU-29 Extender motor con nuevos renderizadores
* CU-28 Configurar motor DSL
* CU-04 Ejecución del motor DSL

Es una regla clave para asegurar la compatibilidad entre el modelo DSL y la implementación del motor.

---

# 9. Control de Cambios

| Versión | Fecha      | Descripción                 |
| ------- | ---------- | --------------------------- |
| 1.0     | 2026-03-28 | Versión inicial de la regla |

---

**Fin del documento**
