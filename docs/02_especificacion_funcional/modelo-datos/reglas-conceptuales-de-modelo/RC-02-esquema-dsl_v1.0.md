# Regla Conceptual: Cumplimiento del Esquema DSL por Plantilla

**Código:** RC-02
**Archivo:** RC-02-esquema-dsl_v1.0.md
**Versión:** 1.0
**Estado:** Aprobada
**Fecha:** 2026-03-28
**Autor:** Equipo Funcional / Arquitectura

---

# 1. Descripción

Esta regla conceptual establece que toda plantilla utilizada en el sistema debe cumplir estrictamente con el esquema definido por el DSL (Domain Specific Language).

El objetivo es asegurar que las plantillas sean interpretables por el motor de generación, manteniendo una estructura consistente, validable y compatible con los renderizadores.

---

# 2. Motivación

El DSL define un contrato entre:

* quienes diseñan las plantillas
* el motor que las interpreta
* los renderizadores que las ejecutan

Si una plantilla no cumple el esquema:

* el motor no puede procesarla correctamente
* pueden producirse errores en tiempo de ejecución
* se pierde interoperabilidad entre componentes

Cumplir el esquema garantiza confiabilidad, mantenibilidad y extensibilidad del sistema.

---

# 3. Definición de la Regla

* Toda plantilla debe respetar la estructura definida por el DSL.
* Debe incluir únicamente elementos y propiedades definidos en el esquema.
* No se permiten atributos, nodos o configuraciones fuera del contrato del DSL.
* La plantilla debe poder ser validada automáticamente contra el esquema antes de su uso.

---

# 4. Condiciones de Aplicación

* Aplica a todas las plantillas cargadas o registradas en el sistema.
* Se valida en el momento de carga (CU-13) y antes de su ejecución en el motor.
* También aplica durante procesos de importación o actualización de plantillas.

---

# 5. Resultados Esperados

* Las plantillas son estructuralmente válidas según el DSL.
* El motor DSL puede interpretarlas sin ambigüedad.
* Se reduce la probabilidad de errores en tiempo de ejecución.
* Los renderizadores reciben estructuras consistentes.

---

# 6. Excepciones

## EX-01 Plantilla inválida

Si una plantilla no cumple el esquema DSL:

* el sistema debe rechazarla
* no debe ser registrada ni ejecutada
* debe informarse el detalle del error de validación

---

## EX-02 Extensión controlada del esquema

Si se incorporan nuevas capacidades al DSL:

* el esquema debe actualizarse formalmente
* las plantillas existentes deben seguir siendo compatibles o versionadas
* se deben definir mecanismos de migración si aplica

---

# 7. Criterios de Validación

## CV-01 Validación estructural

**Dado** una plantilla DSL
**Cuando** se valida contra el esquema
**Entonces** cumple con la estructura definida sin desviaciones

---

## CV-02 Uso de elementos permitidos

**Dado** una plantilla
**Cuando** se analiza su contenido
**Entonces** solo utiliza elementos definidos en el DSL

---

## CV-03 Ausencia de atributos no definidos

**Dado** una plantilla
**Cuando** se valida
**Entonces** no contiene propiedades fuera del esquema

---

## CV-04 Rechazo de plantilla inválida

**Dado** una plantilla que no cumple el esquema
**Cuando** se procesa
**Entonces** el sistema la rechaza con error de validación

---

# 8. Impacto

Esta regla impacta directamente en:

* CU-13 Cargar plantilla DSL
* CU-14 Validar plantilla
* CU-29 Extender motor con renderizadores
* CU-28 Configurar motor DSL
* CU-05 Generación de representación abstracta

Es una de las reglas fundamentales para garantizar la integridad del motor DSL.

---

# 9. Control de Cambios

| Versión | Fecha      | Descripción                 |
| ------- | ---------- | --------------------------- |
| 1.0     | 2026-03-28 | Versión inicial de la regla |

---

**Fin del documento**
