# Caso de Uso: Cargar Datos del Documento

**Código:** CU-18
**Archivo:** CU-18-cargar-datos-documento_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-03-28
**Autor:** Equipo Funcional / Arquitectura

---

# 1. Propósito

Este caso de uso describe el proceso mediante el cual el sistema carga los datos del documento desde una fuente externa, típicamente en formato JSON.

El objetivo es disponer de la información necesaria que será utilizada por el motor DSL para generar el documento final, manteniendo el desacople entre datos, plantilla y dispositivo.

---

# 2. Actores

## Actor Primario

**Sistema consumidor** — Solicita la carga de datos.

## Actores Secundarios

**Fuente de Datos** — Provee el JSON (API, archivo, base de datos).
**Motor DSL** — Utiliza los datos para el procesamiento del documento.

---

# 3. Precondiciones

* Existe una fuente de datos disponible.
* El sistema tiene acceso a dicha fuente (local o remota).
* El formato esperado de datos está definido.
* El motor DSL se encuentra inicializado.

---

# 4. Postcondiciones

## En caso de éxito

* Los datos del documento son cargados correctamente.
* El JSON queda disponible para procesamiento.
* Se valida su estructura básica.

## En caso de fallo

* No se cargan los datos.
* El sistema informa el error correspondiente.
* Se registra el evento para diagnóstico.

---

# 5. Flujo Principal

1. El sistema solicita la carga de datos del documento.
2. El sistema determina la fuente de datos:

   * API REST
   * archivo local
   * base de datos
3. El sistema recupera el JSON.
4. El sistema valida la estructura básica:

   * formato JSON válido
   * existencia de nodos principales
5. El sistema normaliza los datos si es necesario.
6. Los datos se cargan en memoria.
7. El sistema confirma la carga exitosa.
8. El caso de uso finaliza.

---

# 6. Flujos Alternativos

## FA-01 Fuente no disponible

**En el paso 2**

* Si la fuente no responde:

  * El sistema informa error.
  * No continúa el proceso.

---

## FA-02 JSON inválido

**En el paso 4**

* Si el JSON es incorrecto:

  * El sistema informa error de parseo.
  * Detiene el proceso.

---

## FA-03 Estructura incompleta

**En el paso 4**

* Si faltan datos requeridos:

  * El sistema informa advertencia o error.
  * Puede continuar si es tolerable.

---

## FA-04 Error técnico

**En cualquier punto**

* El sistema informa error interno.
* Registra el evento.
* El caso de uso finaliza.

---

# 7. Reglas de Negocio Relacionadas

* RN-01: Los datos deben estar en formato JSON válido.
* RN-02: La carga de datos debe ser independiente de la plantilla.
* RN-03: Se debe permitir múltiples fuentes de datos.

---

# 8. Datos Utilizados

## Entrada

* FuenteDatos

## Salida

* DocumentoDatosJSON

---

# 9. Criterios de Aceptación

## CA-01 Carga exitosa

**Dado** una fuente válida
**Cuando** se cargan los datos
**Entonces** el JSON queda disponible.

---

## CA-02 Validación de formato

**Dado** un JSON inválido
**Cuando** se procesa
**Entonces** el sistema lo rechaza.

---

## CA-03 Independencia de fuente

**Dado** distintas fuentes
**Cuando** se cargan datos
**Entonces** el sistema las procesa correctamente.

---

## CA-04 Manejo de errores

**Dado** un problema en la carga
**Cuando** ocurre
**Entonces** el sistema informa el error.

---

# 10. Cambios respecto a versión 1.0

* Versión inicial del caso de uso.

---

# 11. Notas

Este caso de uso es el punto de entrada del flujo de datos, asegurando que el motor DSL reciba información estructurada y consistente para la generación del documento.

---

**Fin del documento**
