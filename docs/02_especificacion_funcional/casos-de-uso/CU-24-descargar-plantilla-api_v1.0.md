# Caso de Uso: Descargar Plantilla desde API

**Código:** CU-24
**Archivo:** CU-24-descargar-plantilla-api_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-03-28
**Autor:** Equipo Funcional / Arquitectura

---

# 1. Propósito

Este caso de uso describe el proceso mediante el cual el sistema descarga una plantilla DSL desde un servicio remoto (API REST).

El objetivo es permitir la actualización dinámica de plantillas sin necesidad de desplegar nuevas versiones de la aplicación, facilitando la gestión centralizada y el versionado de diseños.

---

# 2. Actores

## Actor Primario

**Sistema consumidor** — Solicita la descarga de la plantilla.

## Actores Secundarios

**API de Plantillas** — Provee las plantillas DSL.
**Motor DSL** — Utiliza la plantilla descargada.

---

# 3. Precondiciones

* Existe conectividad con la API.
* La API de plantillas se encuentra disponible.
* Se conoce el identificador o versión de la plantilla.
* El motor DSL se encuentra inicializado.

---

# 4. Postcondiciones

## En caso de éxito

* La plantilla DSL es descargada correctamente.
* Se encuentra disponible para carga y validación (CU-13, CU-14).
* Se registra la versión descargada.

## En caso de fallo

* No se descarga la plantilla.
* El sistema informa el error correspondiente.
* Se registra el evento para diagnóstico.

---

# 5. Flujo Principal

1. El sistema solicita la descarga de una plantilla.
2. El sistema construye la petición a la API:

   * identificador de plantilla
   * versión (opcional)
3. El sistema envía la solicitud HTTP.
4. La API responde con el contenido de la plantilla (JSON).
5. El sistema recibe la respuesta.
6. El sistema valida la respuesta:

   * código HTTP
   * contenido válido
7. El sistema almacena la plantilla en memoria o caché.
8. El sistema confirma la descarga exitosa.
9. El caso de uso finaliza.

---

# 6. Flujos Alternativos

## FA-01 API no disponible

**En el paso 3**

* Si la API no responde:

  * El sistema informa error.
  * Puede reintentar.

---

## FA-02 Plantilla no encontrada

**En el paso 4**

* Si la API retorna 404:

  * El sistema informa error.
  * No continúa el proceso.

---

## FA-03 Respuesta inválida

**En el paso 6**

* Si el contenido no es válido:

  * El sistema rechaza la plantilla.
  * Informa el error.

---

## FA-04 Error técnico

**En cualquier punto**

* El sistema informa error interno.
* Registra el evento.
* El caso de uso finaliza.

---

# 7. Reglas de Negocio Relacionadas

* RN-01: Las plantillas pueden gestionarse remotamente.
* RN-02: Debe soportarse versionado de plantillas.
* RN-03: Se debe validar la integridad de la respuesta.

---

# 8. Datos Utilizados

## Entrada

* IdentificadorPlantilla
* Version (opcional)

## Salida

* PlantillaDSL

---

# 9. Criterios de Aceptación

## CA-01 Descarga exitosa

**Dado** una API disponible
**Cuando** se solicita la plantilla
**Entonces** se descarga correctamente.

---

## CA-02 Manejo de errores HTTP

**Dado** una respuesta inválida
**Cuando** se procesa
**Entonces** el sistema informa el error.

---

## CA-03 Soporte de versionado

**Dado** múltiples versiones
**Cuando** se solicita una específica
**Entonces** se obtiene la correcta.

---

## CA-04 Manejo de fallos de red

**Dado** falta de conectividad
**Cuando** se intenta descargar
**Entonces** el sistema informa el problema.

---

# 10. Cambios respecto a versión 1.0

* Versión inicial del caso de uso.

---

# 11. Notas

Este caso de uso es fundamental para lograr un sistema dinámico y desacoplado, permitiendo evolucionar las plantillas sin necesidad de modificar ni redistribuir la aplicación cliente.

---

**Fin del documento**
