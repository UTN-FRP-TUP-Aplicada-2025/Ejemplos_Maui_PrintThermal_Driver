# Caso de Uso: Ejecutar Iteraciones (Listas)

**Código:** CU-16
**Archivo:** CU-16-ejecutar-iteraciones-listas_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-03-28
**Autor:** Equipo Funcional / Arquitectura

---

# 1. Propósito

Este caso de uso describe el proceso mediante el cual el motor DSL ejecuta iteraciones sobre colecciones de datos (listas) definidas en la plantilla.

El objetivo es permitir la generación dinámica de múltiples elementos (por ejemplo, ítems de un ticket) a partir de estructuras repetitivas presentes en el documento JSON de entrada.

---

# 2. Actores

## Actor Primario

**Motor DSL** — Responsable de ejecutar las iteraciones.

## Actores Secundarios

**Plantilla DSL** — Define las estructuras iterables.
**Documento de Datos (JSON)** — Contiene las colecciones a iterar.

---

# 3. Precondiciones

* Existe una plantilla DSL válida (CU-14).
* Existe un documento de datos con colecciones.
* Las iteraciones están correctamente definidas en la plantilla.
* El motor DSL se encuentra inicializado.

---

# 4. Postcondiciones

## En caso de éxito

* Las estructuras iterables son expandidas correctamente.
* Se generan múltiples elementos en el documento.
* El documento queda listo para las siguientes etapas.

## En caso de fallo

* No se ejecutan correctamente las iteraciones.
* El sistema informa errores o advertencias.
* Se registra el evento para diagnóstico.

---

# 5. Flujo Principal

1. El sistema inicia la ejecución de iteraciones.
2. El motor DSL recibe:

   * PlantillaDSL
   * DocumentoDatosJSON
3. El sistema identifica bloques iterables en la plantilla.
4. Para cada bloque iterable:

   * identifica la colección asociada (ej. items)
5. El sistema obtiene la colección desde el JSON.
6. Para cada elemento de la colección:

   * clona la estructura del bloque
   * asigna el contexto de datos actual
   * resuelve las referencias internas
7. Se agregan los elementos generados al documento.
8. Se valida la consistencia del resultado.
9. El sistema confirma la ejecución.
10. El caso de uso finaliza.

---

# 6. Flujos Alternativos

## FA-01 Colección inexistente

**En el paso 5**

* Si la colección no existe:

  * El sistema informa error o advertencia.
  * Puede omitir el bloque.

---

## FA-02 Colección vacía

**En el paso 6**

* Si la colección está vacía:

  * El sistema no genera elementos.
  * Puede aplicar comportamiento por defecto.

---

## FA-03 Error en iteración

**En el paso 6**

* Si ocurre un error al procesar un elemento:

  * El sistema informa el problema.
  * Puede continuar con los siguientes elementos.

---

## FA-04 Error técnico

**En cualquier punto**

* El sistema informa error interno.
* Registra el evento.
* El caso de uso finaliza.

---

# 7. Reglas de Negocio Relacionadas

* RN-01: Las iteraciones deben basarse en colecciones válidas.
* RN-02: Cada elemento debe procesarse de forma independiente.
* RN-03: Se debe mantener el orden de la colección.

---

# 8. Datos Utilizados

## Entrada

* PlantillaDSL
* DocumentoDatosJSON

## Salida

* DocumentoConIteraciones

---

# 9. Criterios de Aceptación

## CA-01 Iteración exitosa

**Dado** una colección válida
**Cuando** se procesa
**Entonces** se generan múltiples elementos.

---

## CA-02 Orden de elementos

**Dado** una lista ordenada
**Cuando** se itera
**Entonces** se mantiene el orden original.

---

## CA-03 Manejo de colección vacía

**Dado** una lista vacía
**Cuando** se procesa
**Entonces** no se generan elementos.

---

## CA-04 Manejo de errores

**Dado** un problema en la iteración
**Cuando** ocurre
**Entonces** el sistema informa el error.

---

# 10. Cambios respecto a versión 1.0

* Versión inicial del caso de uso.

---

# 11. Notas

Este caso de uso es esencial para documentos como tickets o facturas, donde se requiere representar listas dinámicas de elementos de forma repetitiva dentro de la plantilla DSL.

---

**Fin del documento**
