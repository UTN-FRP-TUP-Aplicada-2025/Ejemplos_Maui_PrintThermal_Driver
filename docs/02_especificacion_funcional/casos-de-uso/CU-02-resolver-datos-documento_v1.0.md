# Caso de Uso: Resolver Datos del Documento

**Código:** CU-02
**Archivo:** CU-02-resolver-datos-documento_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-03-28
**Autor:** Equipo Funcional / Arquitectura

---

# 1. Propósito

Este caso de uso describe el proceso mediante el cual el motor DSL vincula los datos del documento con la plantilla previamente interpretada, resolviendo las referencias dinámicas definidas en el DSL.

El objetivo es transformar el modelo de documento abstracto en una estructura concreta con valores reales, lista para ser procesada por el motor de layout y renderizado.

---

# 2. Actores

## Actor Primario

**Sistema consumidor** — Aplicación que provee los datos del documento.

## Actores Secundarios

**Motor DSL** — Resuelve las referencias de datos dentro del documento.
**Resolver de Datos** — Componente encargado de mapear datos a la plantilla.

---

# 3. Precondiciones

* Existe un modelo interno del documento generado a partir de la plantilla (CU-01).
* Existe un conjunto de datos del documento en formato JSON.
* Las referencias a datos están definidas en la plantilla.

---

# 4. Postcondiciones

## En caso de éxito

* Todas las referencias de la plantilla son resueltas con datos reales.
* Se genera un modelo de documento con contenido concreto.
* El documento queda listo para el proceso de layout.

## En caso de fallo

* No se completa la resolución de datos.
* El sistema informa inconsistencias o errores.
* Se registra el error para diagnóstico.

---

# 5. Flujo Principal

1. El sistema consumidor envía los datos del documento al motor DSL.
2. El motor DSL recibe el modelo interno del documento.
3. El resolver de datos recorre la estructura del documento.
4. El sistema identifica referencias a datos en los elementos:

   * campos simples
   * listas
   * tablas
5. El sistema obtiene los valores correspondientes desde el JSON de datos.
6. El sistema reemplaza las referencias por valores reales.
7. Si existen colecciones:

   * se ejecutan iteraciones
   * se generan múltiples elementos dinámicos
8. Se valida que todos los datos requeridos estén presentes.
9. El sistema confirma la resolución exitosa.
10. El caso de uso finaliza.

---

# 6. Flujos Alternativos

## FA-01 Dato inexistente

**En el paso 5**

* Si una referencia no existe en los datos:

  * El sistema registra error o advertencia.
  * Puede usar valor por defecto si está definido.
  * El flujo continúa o finaliza según configuración.

---

## FA-02 Tipo de dato incompatible

**En el paso 5**

* Si el tipo de dato no coincide con lo esperado:

  * El sistema informa error.
  * Se detiene el procesamiento.
  * El caso de uso finaliza.

---

## FA-03 Lista vacía

**En el paso 7**

* Si una colección está vacía:

  * El sistema puede omitir el bloque.
  * O renderizar una estructura vacía según configuración.

---

## FA-04 Error técnico

**En cualquier punto**

* El sistema informa error interno.
* Registra el evento.
* El caso de uso finaliza.

---

# 7. Reglas de Negocio Relacionadas

* RN-01: Todas las referencias deben resolverse contra el modelo de datos.
* RN-02: Las iteraciones deben respetar la estructura definida en la plantilla.
* RN-03: Se pueden definir valores por defecto para datos faltantes.

---

# 8. Datos Utilizados

## Entrada

* DocumentoModeloInterno
* DocumentDataJson

## Salida

* DocumentoConDatosResueltos

---

# 9. Criterios de Aceptación

## CA-01 Resolución exitosa

**Dado** datos válidos y plantilla válida
**Cuando** se ejecuta la resolución
**Entonces** el documento contiene todos los valores reales.

---

## CA-02 Iteración de listas

**Dado** una colección de datos
**Cuando** se procesa
**Entonces** se generan elementos dinámicos correspondientes.

---

## CA-03 Manejo de datos faltantes

**Dado** una referencia inexistente
**Cuando** se procesa
**Entonces** el sistema informa el error o aplica valor por defecto.

---

## CA-04 Validación de tipos

**Dado** un tipo de dato incorrecto
**Cuando** se procesa
**Entonces** el sistema rechaza la operación.

---

# 10. Cambios respecto a versión 1.0

* Versión inicial del caso de uso.

---

# 11. Notas

Este caso de uso es crítico ya que conecta el modelo abstracto de la plantilla con la información real del documento, habilitando la generación dinámica de contenido.

---

**Fin del documento**
