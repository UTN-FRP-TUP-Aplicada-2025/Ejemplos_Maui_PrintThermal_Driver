# Caso de Uso: Validar Estructura de Datos

**Código:** CU-19
**Archivo:** CU-19-validar-estructura-datos_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-03-28
**Autor:** Equipo Funcional / Arquitectura

---

# 1. Propósito

Este caso de uso describe el proceso mediante el cual el sistema valida la estructura del documento de datos (JSON) para asegurar su compatibilidad con la plantilla DSL.

El objetivo es garantizar que los datos contienen los nodos, tipos y jerarquías necesarias para que el motor DSL pueda resolver referencias, iteraciones y condiciones sin errores en tiempo de ejecución.

---

# 2. Actores

## Actor Primario

**Motor DSL** — Responsable de validar la estructura de datos.

## Actores Secundarios

**Sistema consumidor** — Provee el documento de datos.
**Plantilla DSL (opcional)** — Puede utilizarse como referencia para validación.

---

# 3. Precondiciones

* Existe un documento de datos cargado (CU-18).
* Existe una definición esperada de estructura (explícita o implícita).
* El motor DSL se encuentra inicializado.

---

# 4. Postcondiciones

## En caso de éxito

* La estructura de datos es válida.
* Los datos pueden ser utilizados en el procesamiento del documento.
* Se registra la validación exitosa.

## En caso de fallo

* La estructura es inválida o incompleta.
* El sistema informa los errores detectados.
* No se permite continuar con el flujo.

---

# 5. Flujo Principal

1. El sistema inicia la validación de estructura de datos.
2. El motor DSL recibe el documento JSON.
3. El sistema valida la sintaxis del JSON.
4. El sistema verifica la existencia de nodos clave:

   * objetos principales
   * colecciones esperadas
5. El sistema valida tipos de datos:

   * string
   * número
   * boolean
   * listas
6. El sistema valida la jerarquía de datos.
7. (Opcional) Se comparan los datos contra la plantilla DSL:

   * referencias existentes
   * estructuras iterables
8. Se construye un resultado de validación.
9. El sistema confirma la operación.
10. El caso de uso finaliza.

---

# 6. Flujos Alternativos

## FA-01 JSON inválido

**En el paso 3**

* Si el JSON no es válido:

  * El sistema informa error de sintaxis.
  * Detiene el proceso.

---

## FA-02 Nodo faltante

**En el paso 4**

* Si falta un nodo requerido:

  * El sistema informa error.
  * Detiene el proceso.

---

## FA-03 Tipo incorrecto

**En el paso 5**

* Si un dato no coincide con el tipo esperado:

  * El sistema informa inconsistencia.
  * Puede rechazar el documento.

---

## FA-04 Incompatibilidad con plantilla

**En el paso 7**

* Si los datos no coinciden con la plantilla:

  * El sistema informa error.
  * No permite continuar.

---

## FA-05 Error técnico

**En cualquier punto**

* El sistema informa error interno.
* Registra el evento.
* El caso de uso finaliza.

---

# 7. Reglas de Negocio Relacionadas

* RN-01: Los datos deben validarse antes de su uso.
* RN-02: La estructura debe ser consistente.
* RN-03: Los tipos de datos deben ser correctos.

---

# 8. Datos Utilizados

## Entrada

* DocumentoDatosJSON
* PlantillaDSL (opcional)

## Salida

* ResultadoValidacionDatos

---

# 9. Criterios de Aceptación

## CA-01 Validación exitosa

**Dado** un JSON válido
**Cuando** se valida
**Entonces** se aprueba sin errores.

---

## CA-02 Detección de errores

**Dado** datos incorrectos
**Cuando** se valida
**Entonces** se informan inconsistencias.

---

## CA-03 Compatibilidad con plantilla

**Dado** una plantilla definida
**Cuando** se valida
**Entonces** los datos coinciden con lo esperado.

---

## CA-04 Rechazo de datos inválidos

**Dado** errores críticos
**Cuando** se valida
**Entonces** no se permite continuar.

---

# 10. Cambios respecto a versión 1.0

* Versión inicial del caso de uso.

---

# 11. Notas

Este caso de uso es clave para asegurar la calidad de entrada del sistema, evitando fallos en etapas posteriores como resolución de referencias, iteraciones y renderizado.

---

**Fin del documento**
