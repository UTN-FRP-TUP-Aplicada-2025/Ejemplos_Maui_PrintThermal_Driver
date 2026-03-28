# Caso de Uso: Descargar Perfil de Impresora

**Código:** CU-26
**Archivo:** CU-26-descargar-perfil-impresora_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-03-28
**Autor:** Equipo Funcional / Arquitectura

---

# 1. Propósito

Este caso de uso describe el proceso mediante el cual el sistema descarga el perfil de un dispositivo (impresora) desde un servicio remoto (API REST).

El objetivo es permitir la incorporación dinámica de nuevos dispositivos o la actualización de configuraciones existentes sin necesidad de modificar o redeployar la aplicación.

---

# 2. Actores

## Actor Primario

**Sistema consumidor** — Solicita la descarga del perfil.

## Actores Secundarios

**API de Perfiles** — Provee los perfiles de dispositivos.
**Motor DSL** — Utiliza el perfil en etapas posteriores.

---

# 3. Precondiciones

* Existe conectividad con la API.
* La API de perfiles se encuentra disponible.
* Se conoce el identificador del perfil o del dispositivo.
* El motor DSL se encuentra inicializado.

---

# 4. Postcondiciones

## En caso de éxito

* El perfil de impresora es descargado correctamente.
* Queda disponible para carga y validación (CU-21, CU-22).
* Se registra la versión del perfil.

## En caso de fallo

* No se descarga el perfil.
* El sistema informa el error correspondiente.
* Se registra el evento para diagnóstico.

---

# 5. Flujo Principal

1. El sistema solicita la descarga del perfil de impresora.
2. El sistema construye la petición a la API:

   * identificador del perfil
   * versión (opcional)
3. El sistema envía la solicitud HTTP.
4. La API responde con el JSON del perfil.
5. El sistema recibe la respuesta.
6. El sistema valida la respuesta:

   * código HTTP
   * formato JSON válido
7. El sistema almacena el perfil en memoria o caché.
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

## FA-02 Perfil no encontrado

**En el paso 4**

* Si la API retorna 404:

  * El sistema informa error.
  * No continúa el proceso.

---

## FA-03 Respuesta inválida

**En el paso 6**

* Si el JSON es inválido:

  * El sistema rechaza el perfil.
  * Informa el error.

---

## FA-04 Error técnico

**En cualquier punto**

* El sistema informa error interno.
* Registra el evento.
* El caso de uso finaliza.

---

# 7. Reglas de Negocio Relacionadas

* RN-01: Los perfiles pueden gestionarse remotamente.
* RN-02: Debe soportarse versionado de perfiles.
* RN-03: Se debe validar la integridad de la respuesta.

---

# 8. Datos Utilizados

## Entrada

* IdentificadorPerfil
* Version (opcional)

## Salida

* PerfilDispositivo

---

# 9. Criterios de Aceptación

## CA-01 Descarga exitosa

**Dado** una API disponible
**Cuando** se solicita el perfil
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

Este caso de uso es clave para soportar la evolución del hardware, permitiendo que nuevos dispositivos puedan integrarse al sistema mediante configuración dinámica.

---

**Fin del documento**
