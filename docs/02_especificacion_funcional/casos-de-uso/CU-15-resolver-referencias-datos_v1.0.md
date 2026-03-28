# Caso de Uso: Resolver Referencias a Datos en Plantilla

**Código:** CU-15
**Archivo:** CU-15-resolver-referencias-datos_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-03-28
**Autor:** Equipo Funcional / Arquitectura

---

# 1. Propósito

Este caso de uso describe el proceso mediante el cual el motor DSL resuelve las referencias a datos definidas en la plantilla, vinculándolas con el documento JSON de entrada.

El objetivo es reemplazar expresiones dinámicas (placeholders) por valores concretos, generando un documento listo para ser procesado por las siguientes etapas (adaptación, layout y renderizado).

---

# 2. Actores

## Actor Primario

**Motor DSL** — Responsable de resolver las referencias.

## Actores Secundarios

**Plantilla DSL** — Define las referencias a datos.
**Documento de Datos (JSON)** — Provee los valores a resolver.

---

# 3. Precondiciones

* Existe una plantilla DSL válida (CU-14).
* Existe un documento de datos disponible.
* Las referencias están correctamente definidas en la plantilla.
* El motor DSL se encuentra inicializado.

---

# 4. Postcondiciones

## En caso de éxito

* Todas las referencias válidas son resueltas.
* Se genera un documento con datos concretos.
* El resultado queda listo para las siguientes etapas.

## En caso de fallo

* No se resuelven todas las referencias.
* El sistema informa errores o advertencias.
* Se registra el evento para diagnóstico.

---

# 5. Flujo Principal

1. El sistema inicia la resolución de referencias.
2. El motor DSL recibe:

   * PlantillaDSL
   * DocumentoDatosJSON
3. El sistema recorre la plantilla.
4. Para cada elemento con referencia:

   * identifica el placeholder (ej. {{cliente.nombre}})
5. El sistema busca el valor en el documento JSON.
6. Si el valor existe:

   * reemplaza la referencia por el valor concreto
7. Si el valor es complejo (listas, objetos):

   * aplica reglas de expansión o iteración
8. Se construye el documento con datos resueltos.
9. Se valida la consistencia del resultado.
10. El sistema confirma la operación.
11. El caso de uso finaliza.

---

# 6. Flujos Alternativos

## FA-01 Referencia inexistente

**En el paso 5**

* Si la referencia no existe en el JSON:

  * El sistema informa error o advertencia.
  * Puede usar valor por defecto (si está definido).

---

## FA-02 Tipo de dato incompatible

**En el paso 7**

* Si el tipo no coincide con lo esperado:

  * El sistema informa inconsistencia.
  * Puede intentar conversión.

---

## FA-03 Error en iteración

**En el paso 7**

* Si falla la expansión de listas:

  * El sistema detiene el proceso.
  * Informa el error.

---

## FA-04 Error técnico

**En cualquier punto**

* El sistema informa error interno.
* Registra el evento.
* El caso de uso finaliza.

---

# 7. Reglas de Negocio Relacionadas

* RN-01: Todas las referencias deben resolverse antes del layout.
* RN-02: Se permiten valores por defecto configurables.
* RN-03: Las referencias deben existir en el documento de datos.

---

# 8. Datos Utilizados

## Entrada

* PlantillaDSL
* DocumentoDatosJSON

## Salida

* DocumentoConDatosResueltos

---

# 9. Criterios de Aceptación

## CA-01 Resolución exitosa

**Dado** referencias válidas
**Cuando** se procesa la plantilla
**Entonces** se reemplazan correctamente.

---

## CA-02 Manejo de valores faltantes

**Dado** una referencia inexistente
**Cuando** se procesa
**Entonces** el sistema informa o usa valor por defecto.

---

## CA-03 Soporte de estructuras complejas

**Dado** listas u objetos
**Cuando** se procesan
**Entonces** se expanden correctamente.

---

## CA-04 Manejo de errores

**Dado** un problema en la resolución
**Cuando** ocurre
**Entonces** el sistema informa el error.

---

# 10. Cambios respecto a versión 1.0

* Versión inicial del caso de uso.

---

# 11. Notas

Este caso de uso es central en el motor DSL, ya que conecta la estructura declarativa de la plantilla con los datos reales, permitiendo la generación dinámica de documentos.

---

**Fin del documento**
