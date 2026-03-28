# Caso de Uso: Manejar Errores de Plantilla Inválida

**Código:** CU-30
**Archivo:** CU-30-manejar-errores-plantilla_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-03-28
**Autor:** Equipo Funcional / Arquitectura

---

# 1. Propósito

Este caso de uso describe el proceso mediante el cual el sistema detecta, gestiona y comunica errores relacionados con plantillas DSL inválidas.

El objetivo es garantizar robustez en el motor DSL, evitando fallos en tiempo de ejecución y proporcionando información clara para diagnóstico y corrección.

---

# 2. Actores

## Actor Primario

**Motor DSL** — Detecta y maneja los errores.

## Actores Secundarios

**Desarrollador / Configurador** — Corrige la plantilla.
**Sistema consumidor** — Recibe el resultado del error.

---

# 3. Precondiciones

* Existe una plantilla DSL cargada (CU-13).
* La plantilla ha sido sometida a validación (CU-14) o está en proceso de uso.
* El motor DSL se encuentra en ejecución.

---

# 4. Postcondiciones

## En caso de éxito

* El error es detectado correctamente.
* Se genera un reporte detallado.
* El sistema evita continuar con una plantilla inválida.

## En caso de fallo

* El error no es correctamente identificado.
* Puede generarse comportamiento inesperado.
* Se registra el evento.

---

# 5. Flujo Principal

1. El sistema inicia el procesamiento de la plantilla.
2. El motor DSL detecta inconsistencias:

   * sintaxis inválida
   * nodos incorrectos
   * estructuras mal definidas
3. El sistema clasifica el error:

   * error crítico
   * advertencia
4. El sistema construye un reporte:

   * tipo de error
   * ubicación en la plantilla
   * descripción
5. El sistema detiene el procesamiento si el error es crítico.
6. El sistema notifica al consumidor.
7. Se registra el error para diagnóstico.
8. El caso de uso finaliza.

---

# 6. Flujos Alternativos

## FA-01 Error no crítico

**En el paso 3**

* Si el error es una advertencia:

  * El sistema continúa el procesamiento.
  * Registra la advertencia.

---

## FA-02 Error en múltiples puntos

**En el paso 2**

* Si existen múltiples errores:

  * El sistema los agrupa.
  * Genera un reporte consolidado.

---

## FA-03 Error técnico

**En cualquier punto**

* El sistema informa error interno.
* Registra el evento.
* El caso de uso finaliza.

---

# 7. Reglas de Negocio Relacionadas

* RN-01: Toda plantilla debe validarse antes de su uso.
* RN-02: Los errores deben clasificarse por severidad.
* RN-03: El sistema debe ser tolerante a fallos no críticos.

---

# 8. Datos Utilizados

## Entrada

* PlantillaDSL

## Salida

* ReporteErroresPlantilla

---

# 9. Criterios de Aceptación

## CA-01 Detección de errores

**Dado** una plantilla inválida
**Cuando** se procesa
**Entonces** el sistema detecta errores.

---

## CA-02 Clasificación correcta

**Dado** distintos tipos de errores
**Cuando** se analizan
**Entonces** se clasifican correctamente.

---

## CA-03 Generación de reporte

**Dado** un error detectado
**Cuando** se procesa
**Entonces** se informa con detalle.

---

## CA-04 Bloqueo ante error crítico

**Dado** un error grave
**Cuando** se detecta
**Entonces** se detiene la ejecución.

---

# 10. Cambios respecto a versión 1.0

* Versión inicial del caso de uso.

---

# 11. Notas

Este caso de uso es fundamental para la estabilidad del sistema, ya que permite detectar problemas en las plantillas DSL antes de que impacten en la generación o impresión de documentos.

---

**Fin del documento**
