# Caso de Uso: Cargar Plantilla DSL

**Código:** CU-13
**Archivo:** CU-13-cargar-plantilla-dsl_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-03-28
**Autor:** Equipo Funcional / Arquitectura

---

# 1. Propósito

Este caso de uso describe el proceso mediante el cual el sistema carga una plantilla DSL que define la estructura y comportamiento del documento a generar.

El objetivo es permitir que el motor DSL utilice definiciones externas (generalmente en formato JSON) para construir documentos sin necesidad de código, facilitando la flexibilidad y configuración dinámica.

---

# 2. Actores

## Actor Primario

**Sistema consumidor** — Solicita la carga de la plantilla.

## Actores Secundarios

**Motor DSL** — Interpreta la plantilla.
**Repositorio de Plantillas** — Fuente de almacenamiento (local o remoto).

---

# 3. Precondiciones

* Existe al menos una plantilla DSL disponible.
* El sistema tiene acceso al repositorio de plantillas (local o remoto).
* El motor DSL se encuentra inicializado.

---

# 4. Postcondiciones

## En caso de éxito

* La plantilla DSL es cargada correctamente.
* La estructura queda disponible para procesamiento.
* Se valida su formato y consistencia.

## En caso de fallo

* La plantilla no se carga.
* El sistema informa el error correspondiente.
* Se registra el evento para diagnóstico.

---

# 5. Flujo Principal

1. El sistema solicita la carga de una plantilla DSL.
2. El sistema obtiene la fuente de la plantilla:

   * archivo local
   * endpoint REST
3. El sistema recupera el contenido de la plantilla.
4. El motor DSL parsea el JSON.
5. El sistema valida la estructura:

   * secciones
   * bloques
   * elementos
6. El sistema valida reglas del DSL:

   * sintaxis
   * referencias a datos
7. Se construye una representación interna de la plantilla.
8. El sistema confirma la carga exitosa.
9. El caso de uso finaliza.

---

# 6. Flujos Alternativos

## FA-01 Plantilla no encontrada

**En el paso 2**

* Si la plantilla no existe:

  * El sistema informa error.
  * No continúa el proceso.

---

## FA-02 JSON inválido

**En el paso 4**

* Si el JSON es incorrecto:

  * El sistema informa error de parseo.
  * Detiene el proceso.

---

## FA-03 Estructura inválida

**En el paso 5**

* Si faltan elementos obligatorios:

  * El sistema rechaza la plantilla.
  * Informa inconsistencias.

---

## FA-04 Error técnico

**En cualquier punto**

* El sistema informa error interno.
* Registra el evento.
* El caso de uso finaliza.

---

# 7. Reglas de Negocio Relacionadas

* RN-01: Toda plantilla debe cumplir el esquema DSL definido.
* RN-02: Las plantillas deben ser externas al código.
* RN-03: Debe validarse antes de su uso.

---

# 8. Datos Utilizados

## Entrada

* FuentePlantilla (archivo / URL)

## Salida

* PlantillaDSL

---

# 9. Criterios de Aceptación

## CA-01 Carga exitosa

**Dado** una plantilla válida
**Cuando** se carga
**Entonces** queda disponible para uso.

---

## CA-02 Validación de estructura

**Dado** una plantilla incompleta
**Cuando** se procesa
**Entonces** el sistema la rechaza.

---

## CA-03 Manejo de errores

**Dado** un JSON inválido
**Cuando** se intenta cargar
**Entonces** el sistema informa el error.

---

## CA-04 Independencia del origen

**Dado** una plantilla remota o local
**Cuando** se carga
**Entonces** el sistema la procesa correctamente.

---

# 10. Cambios respecto a versión 1.0

* Versión inicial del caso de uso.

---

# 11. Notas

Este caso de uso es clave para el enfoque basado en DSL, ya que permite definir el comportamiento del sistema de forma declarativa y desacoplada del código fuente.

---

**Fin del documento**
