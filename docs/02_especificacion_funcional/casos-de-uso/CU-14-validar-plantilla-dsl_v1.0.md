# Caso de Uso: Validar Plantilla DSL

**Código:** CU-14
**Archivo:** CU-14-validar-plantilla-dsl_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-03-28
**Autor:** Equipo Funcional / Arquitectura

---

# 1. Propósito

Este caso de uso describe el proceso mediante el cual el sistema valida una plantilla DSL para asegurar que cumple con la estructura, reglas y consistencia requeridas antes de ser utilizada por el motor.

El objetivo es prevenir errores en tiempo de ejecución, garantizando que las plantillas sean correctas, completas y compatibles con el modelo del sistema.

---

# 2. Actores

## Actor Primario

**Motor DSL** — Responsable de validar la plantilla.

## Actores Secundarios

**Sistema consumidor** — Provee la plantilla a validar.
**Repositorio de Plantillas** — Fuente de origen de la plantilla (opcional).

---

# 3. Precondiciones

* Existe una plantilla DSL cargada (CU-13).
* El esquema del DSL está definido.
* El motor DSL se encuentra inicializado.

---

# 4. Postcondiciones

## En caso de éxito

* La plantilla es considerada válida.
* Se habilita su uso en el motor.
* Se registra la validación exitosa.

## En caso de fallo

* La plantilla es rechazada.
* Se informan los errores detectados.
* No puede utilizarse en el flujo de generación.

---

# 5. Flujo Principal

1. El sistema inicia la validación de la plantilla DSL.
2. El motor DSL recibe la plantilla.
3. El sistema valida la sintaxis del JSON.
4. El sistema valida la estructura:

   * presencia de secciones
   * bloques definidos
   * elementos válidos
5. El sistema valida reglas del DSL:

   * tipos de elementos permitidos
   * propiedades obligatorias
   * jerarquía correcta
6. El sistema valida referencias:

   * variables de datos existentes
   * enlaces entre elementos
7. El sistema valida compatibilidad general.
8. Se construye un resultado de validación.
9. El sistema confirma la validación.
10. El caso de uso finaliza.

---

# 6. Flujos Alternativos

## FA-01 JSON inválido

**En el paso 3**

* Si el JSON no es válido:

  * El sistema informa error de sintaxis.
  * Detiene el proceso.

---

## FA-02 Estructura incompleta

**En el paso 4**

* Si faltan elementos obligatorios:

  * El sistema rechaza la plantilla.
  * Informa los errores.

---

## FA-03 Referencias inválidas

**En el paso 6**

* Si existen referencias a datos inexistentes:

  * El sistema informa inconsistencias.
  * Detiene el proceso.

---

## FA-04 Error técnico

**En cualquier punto**

* El sistema informa error interno.
* Registra el evento.
* El caso de uso finaliza.

---

# 7. Reglas de Negocio Relacionadas

* RN-01: Toda plantilla debe validarse antes de su uso.
* RN-02: No se permiten referencias a datos inexistentes.
* RN-03: La estructura debe cumplir el esquema DSL.

---

# 8. Datos Utilizados

## Entrada

* PlantillaDSL

## Salida

* ResultadoValidacion

---

# 9. Criterios de Aceptación

## CA-01 Validación exitosa

**Dado** una plantilla correcta
**Cuando** se valida
**Entonces** se aprueba sin errores.

---

## CA-02 Detección de errores

**Dado** una plantilla incorrecta
**Cuando** se valida
**Entonces** se informan los errores.

---

## CA-03 Rechazo de plantilla inválida

**Dado** errores críticos
**Cuando** se valida
**Entonces** no se permite su uso.

---

## CA-04 Consistencia de referencias

**Dado** variables en la plantilla
**Cuando** se validan
**Entonces** deben existir en el modelo de datos.

---

# 10. Cambios respecto a versión 1.0

* Versión inicial del caso de uso.

---

# 11. Notas

Este caso de uso es fundamental para garantizar la robustez del sistema basado en DSL, evitando errores en tiempo de ejecución y facilitando la detección temprana de inconsistencias.

---

**Fin del documento**
