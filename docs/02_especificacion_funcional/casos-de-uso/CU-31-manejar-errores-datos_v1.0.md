# Caso de Uso: Manejar Errores de Datos Inconsistentes

**Código:** CU-31
**Archivo:** CU-31-manejar-errores-datos_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-03-28
**Autor:** Equipo Funcional / Arquitectura

---

# 1. Propósito

Este caso de uso describe el proceso mediante el cual el sistema detecta, gestiona y comunica inconsistencias en los datos del documento (JSON) durante el procesamiento del motor DSL.

El objetivo es garantizar la integridad del flujo de generación, evitando resultados incorrectos derivados de datos incompletos, inválidos o incoherentes.

---

# 2. Actores

## Actor Primario

**Motor DSL** — Detecta y maneja inconsistencias de datos.

## Actores Secundarios

**Sistema consumidor** — Provee los datos y recibe el resultado.
**Desarrollador / Configurador** — Analiza y corrige los datos.

---

# 3. Precondiciones

* Existe un documento de datos cargado (CU-18).
* El documento fue validado estructuralmente (CU-19).
* El motor DSL se encuentra en ejecución.

---

# 4. Postcondiciones

## En caso de éxito

* Las inconsistencias son detectadas y gestionadas.
* Se genera un reporte claro de errores o advertencias.
* El sistema evita resultados inválidos.

## En caso de fallo

* Las inconsistencias no son detectadas correctamente.
* Se pueden generar resultados incorrectos.
* Se registra el evento para diagnóstico.

---

# 5. Flujo Principal

1. El sistema inicia el procesamiento de datos.
2. El motor DSL analiza el contenido:

   * valores nulos
   * datos faltantes
   * inconsistencias lógicas
3. El sistema detecta problemas:

   * campos obligatorios vacíos
   * relaciones inválidas
   * datos fuera de rango
4. El sistema clasifica las inconsistencias:

   * error crítico
   * advertencia
5. El sistema construye un reporte:

   * campo afectado
   * tipo de inconsistencia
   * descripción
6. El sistema decide el comportamiento:

   * detener ejecución (error crítico)
   * continuar con advertencias
7. El sistema notifica al consumidor.
8. Se registra el evento para auditoría.
9. El caso de uso finaliza.

---

# 6. Flujos Alternativos

## FA-01 Datos faltantes

**En el paso 3**

* Si faltan datos obligatorios:

  * El sistema informa error.
  * Puede detener la ejecución.

---

## FA-02 Valores inválidos

**En el paso 3**

* Si un valor es incorrecto:

  * El sistema informa inconsistencia.
  * Puede continuar o detenerse.

---

## FA-03 Inconsistencia lógica

**En el paso 2**

* Si los datos no son coherentes:

  * El sistema informa error.
  * Registra el evento.

---

## FA-04 Error técnico

**En cualquier punto**

* El sistema informa error interno.
* Registra el evento.
* El caso de uso finaliza.

---

# 7. Reglas de Negocio Relacionadas

* RN-01: Los datos deben ser consistentes antes de su uso.
* RN-02: Las inconsistencias deben clasificarse por severidad.
* RN-03: El sistema debe permitir tolerancia a errores no críticos.

---

# 8. Datos Utilizados

## Entrada

* DocumentoDatosJSON

## Salida

* ReporteErroresDatos

---

# 9. Criterios de Aceptación

## CA-01 Detección de inconsistencias

**Dado** datos incorrectos
**Cuando** se procesan
**Entonces** el sistema detecta errores.

---

## CA-02 Clasificación correcta

**Dado** distintos problemas
**Cuando** se analizan
**Entonces** se clasifican adecuadamente.

---

## CA-03 Generación de reporte

**Dado** inconsistencias detectadas
**Cuando** se procesan
**Entonces** se informa claramente.

---

## CA-04 Bloqueo ante error crítico

**Dado** una inconsistencia grave
**Cuando** se detecta
**Entonces** se detiene la ejecución.

---

# 10. Cambios respecto a versión 1.0

* Versión inicial del caso de uso.

---

# 11. Notas

Este caso de uso es fundamental para asegurar la calidad del resultado final, ya que los datos inconsistentes pueden impactar directamente en la correcta generación del documento.

Complementa la validación estructural (CU-19) agregando una capa de validación semántica y lógica.

---

**Fin del documento**
