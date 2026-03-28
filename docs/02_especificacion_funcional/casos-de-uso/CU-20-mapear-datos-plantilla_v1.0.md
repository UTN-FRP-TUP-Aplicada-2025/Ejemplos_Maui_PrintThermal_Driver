# Caso de Uso: Mapear Datos a Plantilla

**Código:** CU-20
**Archivo:** CU-20-mapear-datos-plantilla_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-03-28
**Autor:** Equipo Funcional / Arquitectura

---

# 1. Propósito

Este caso de uso describe el proceso mediante el cual el sistema establece la correspondencia entre los datos del documento (JSON) y la estructura definida en la plantilla DSL.

El objetivo es preparar el contexto de ejecución del motor DSL, asegurando que las referencias, iteraciones y condiciones puedan resolverse correctamente sobre los datos disponibles.

---

# 2. Actores

## Actor Primario

**Motor DSL** — Responsable de realizar el mapeo.

## Actores Secundarios

**Plantilla DSL** — Define la estructura y referencias.
**Documento de Datos (JSON)** — Contiene la información a mapear.

---

# 3. Precondiciones

* Existe un documento de datos válido (CU-19).
* Existe una plantilla DSL validada (CU-14).
* El motor DSL se encuentra inicializado.

---

# 4. Postcondiciones

## En caso de éxito

* Se establece un contexto de datos asociado a la plantilla.
* Las rutas de acceso a datos quedan resueltas lógicamente.
* El documento queda listo para ejecución del motor.

## En caso de fallo

* No se logra mapear correctamente.
* El sistema informa inconsistencias.
* Se registra el evento para diagnóstico.

---

# 5. Flujo Principal

1. El sistema inicia el proceso de mapeo.
2. El motor DSL recibe:

   * PlantillaDSL
   * DocumentoDatosJSON
3. El sistema analiza las referencias de la plantilla:

   * rutas de acceso (ej. cliente.nombre)
4. El sistema verifica correspondencia en el JSON.
5. Se construye un contexto de datos:

   * nodos accesibles
   * estructuras iterables
6. El sistema asocia los datos a la estructura de la plantilla.
7. Se validan las relaciones establecidas.
8. El sistema confirma el mapeo exitoso.
9. El caso de uso finaliza.

---

# 6. Flujos Alternativos

## FA-01 Referencia sin correspondencia

**En el paso 4**

* Si no existe la ruta en el JSON:

  * El sistema informa error o advertencia.
  * Puede marcar la referencia como no resuelta.

---

## FA-02 Estructura incompatible

**En el paso 5**

* Si la estructura no coincide:

  * El sistema informa inconsistencia.
  * Detiene el proceso.

---

## FA-03 Ambigüedad en datos

**En el paso 3**

* Si múltiples rutas coinciden:

  * El sistema selecciona según regla definida.
  * O informa conflicto.

---

## FA-04 Error técnico

**En cualquier punto**

* El sistema informa error interno.
* Registra el evento.
* El caso de uso finaliza.

---

# 7. Reglas de Negocio Relacionadas

* RN-01: Todas las referencias deben mapearse a datos existentes.
* RN-02: El mapeo debe ser consistente con la estructura del JSON.
* RN-03: Se debe construir un contexto de datos reutilizable.

---

# 8. Datos Utilizados

## Entrada

* PlantillaDSL
* DocumentoDatosJSON

## Salida

* ContextoDatosPlantilla

---

# 9. Criterios de Aceptación

## CA-01 Mapeo exitoso

**Dado** datos y plantilla válidos
**Cuando** se realiza el mapeo
**Entonces** se construye el contexto correctamente.

---

## CA-02 Correspondencia de referencias

**Dado** referencias en la plantilla
**Cuando** se evalúan
**Entonces** coinciden con el JSON.

---

## CA-03 Detección de inconsistencias

**Dado** datos incompatibles
**Cuando** se procesa
**Entonces** el sistema informa el problema.

---

## CA-04 Manejo de errores

**Dado** un fallo en el mapeo
**Cuando** ocurre
**Entonces** el sistema informa el error.

---

# 10. Cambios respecto a versión 1.0

* Versión inicial del caso de uso.

---

# 11. Notas

Este caso de uso actúa como puente entre los datos y la plantilla, permitiendo que el motor DSL opere con un contexto estructurado y coherente durante toda la ejecución.

---

**Fin del documento**
