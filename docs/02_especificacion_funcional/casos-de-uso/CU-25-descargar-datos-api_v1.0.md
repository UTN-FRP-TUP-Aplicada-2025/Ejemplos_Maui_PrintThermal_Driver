# Caso de Uso: Descargar Datos desde API

**Código:** CU-25
**Archivo:** CU-25-descargar-datos-api_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-03-28
**Autor:** Equipo Funcional / Arquitectura

---

# 1. Propósito

Este caso de uso describe el proceso mediante el cual el sistema obtiene los datos del documento desde un servicio remoto (API REST).

El objetivo es permitir la generación dinámica de documentos a partir de información actualizada, desacoplando la fuente de datos del motor DSL.

---

# 2. Actores

## Actor Primario

**Sistema consumidor** — Solicita la descarga de datos.

## Actores Secundarios

**API de Datos** — Provee el JSON del documento.
**Motor DSL** — Utiliza los datos descargados.

---

# 3. Precondiciones

* Existe conectividad con la API.
* La API de datos se encuentra disponible.
* Se conoce el identificador del documento o parámetros de consulta.
* El motor DSL se encuentra inicializado.

---

# 4. Postcondiciones

## En caso de éxito

* Los datos del documento son descargados correctamente.
* Se encuentran disponibles para validación y procesamiento (CU-18, CU-19).
* Se registra la operación realizada.

## En caso de fallo

* No se descargan los datos.
* El sistema informa el error correspondiente.
* Se registra el evento para diagnóstico.

---

# 5. Flujo Principal

1. El sistema solicita la descarga de datos.
2. El sistema construye la petición a la API:

   * identificador de documento
   * parámetros adicionales (opcional)
3. El sistema envía la solicitud HTTP.
4. La API responde con el JSON del documento.
5. El sistema recibe la respuesta.
6. El sistema valida la respuesta:

   * código HTTP
   * formato JSON válido
7. El sistema almacena los datos en memoria o caché.
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

## FA-02 Documento no encontrado

**En el paso 4**

* Si la API retorna 404:

  * El sistema informa error.
  * No continúa el proceso.

---

## FA-03 Respuesta inválida

**En el paso 6**

* Si el JSON es inválido:

  * El sistema rechaza los datos.
  * Informa el error.

---

## FA-04 Error técnico

**En cualquier punto**

* El sistema informa error interno.
* Registra el evento.
* El caso de uso finaliza.

---

# 7. Reglas de Negocio Relacionadas

* RN-01: Los datos pueden provenir de fuentes remotas.
* RN-02: Se debe validar la integridad de la respuesta.
* RN-03: Se deben soportar distintos parámetros de consulta.

---

# 8. Datos Utilizados

## Entrada

* IdentificadorDocumento
* ParámetrosConsulta (opcional)

## Salida

* DocumentoDatosJSON

---

# 9. Criterios de Aceptación

## CA-01 Descarga exitosa

**Dado** una API disponible
**Cuando** se solicita el documento
**Entonces** se obtiene el JSON correctamente.

---

## CA-02 Manejo de errores HTTP

**Dado** una respuesta inválida
**Cuando** se procesa
**Entonces** el sistema informa el error.

---

## CA-03 Soporte de parámetros

**Dado** parámetros de consulta
**Cuando** se envía la solicitud
**Entonces** la API responde correctamente.

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

Este caso de uso permite mantener el sistema actualizado y dinámico, desacoplando completamente la generación del documento de la fuente de datos.

---

**Fin del documento**
