# Caso de Uso: Evaluar Condiciones en Plantilla

**Código:** CU-17
**Archivo:** CU-17-evaluar-condiciones_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-03-28
**Autor:** Equipo Funcional / Arquitectura

---

# 1. Propósito

Este caso de uso describe el proceso mediante el cual el motor DSL evalúa condiciones definidas en la plantilla para determinar la inclusión, exclusión o modificación de elementos en el documento final.

El objetivo es permitir lógica condicional declarativa dentro de la plantilla, facilitando la generación de documentos dinámicos sin necesidad de código.

---

# 2. Actores

## Actor Primario

**Motor DSL** — Responsable de evaluar las condiciones.

## Actores Secundarios

**Plantilla DSL** — Define las condiciones.
**Documento de Datos (JSON)** — Provee los valores para evaluar.

---

# 3. Precondiciones

* Existe una plantilla DSL válida (CU-14).
* Existe un documento de datos disponible.
* Las condiciones están correctamente definidas.
* El motor DSL se encuentra inicializado.

---

# 4. Postcondiciones

## En caso de éxito

* Las condiciones son evaluadas correctamente.
* Se incluyen o excluyen elementos según corresponda.
* El documento refleja la lógica condicional definida.

## En caso de fallo

* No se pueden evaluar algunas condiciones.
* El sistema informa errores o advertencias.
* Se registra el evento para diagnóstico.

---

# 5. Flujo Principal

1. El sistema inicia la evaluación de condiciones.
2. El motor DSL recibe:

   * PlantillaDSL
   * DocumentoDatosJSON
3. El sistema identifica elementos condicionales en la plantilla.
4. Para cada condición:

   * evalúa la expresión (ej. total > 0)
5. El sistema obtiene los valores desde el JSON.
6. El sistema evalúa la expresión lógica.
7. Según el resultado:

   * TRUE → incluye el elemento
   * FALSE → excluye el elemento
8. Se construye el documento con condiciones aplicadas.
9. Se valida la consistencia del resultado.
10. El sistema confirma la operación.
11. El caso de uso finaliza.

---

# 6. Flujos Alternativos

## FA-01 Variable inexistente

**En el paso 5**

* Si la variable no existe:

  * El sistema informa error o advertencia.
  * Puede asumir valor por defecto.

---

## FA-02 Error en expresión

**En el paso 4**

* Si la condición es inválida:

  * El sistema informa error.
  * Omite la evaluación.

---

## FA-03 Tipo incompatible

**En el paso 6**

* Si los tipos no son comparables:

  * El sistema informa inconsistencia.
  * No evalúa la condición.

---

## FA-04 Error técnico

**En cualquier punto**

* El sistema informa error interno.
* Registra el evento.
* El caso de uso finaliza.

---

# 7. Reglas de Negocio Relacionadas

* RN-01: Las condiciones deben evaluarse antes del layout.
* RN-02: Solo se permiten expresiones válidas.
* RN-03: Las variables deben existir en el documento de datos.

---

# 8. Datos Utilizados

## Entrada

* PlantillaDSL
* DocumentoDatosJSON

## Salida

* DocumentoCondicionado

---

# 9. Criterios de Aceptación

## CA-01 Evaluación correcta

**Dado** una condición válida
**Cuando** se evalúa
**Entonces** se obtiene el resultado esperado.

---

## CA-02 Inclusión de elementos

**Dado** condición verdadera
**Cuando** se procesa
**Entonces** el elemento se incluye.

---

## CA-03 Exclusión de elementos

**Dado** condición falsa
**Cuando** se procesa
**Entonces** el elemento se excluye.

---

## CA-04 Manejo de errores

**Dado** una condición inválida
**Cuando** se evalúa
**Entonces** el sistema informa el error.

---

# 10. Cambios respecto a versión 1.0

* Versión inicial del caso de uso.

---

# 11. Notas

Este caso de uso aporta flexibilidad al motor DSL, permitiendo adaptar el contenido del documento en función de los datos sin necesidad de lógica imperativa en el código.

---

**Fin del documento**
