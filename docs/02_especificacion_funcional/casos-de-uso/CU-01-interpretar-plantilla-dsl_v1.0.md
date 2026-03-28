# Caso de Uso: Interpretar Plantilla DSL

**Código:** CU-01
**Archivo:** CU-01-interpretar-plantilla-dsl_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-03-28
**Autor:** Equipo Funcional / Arquitectura

---

# 1. Propósito

Este caso de uso describe el proceso mediante el cual el sistema interpreta una plantilla DSL definida en formato JSON, construyendo una representación interna del documento.

El objetivo es transformar una definición declarativa en una estructura procesable que permita posteriormente resolver datos, calcular layout y renderizar el documento en distintos formatos.

---

# 2. Actores

## Actor Primario

**Sistema consumidor** — Aplicación que utiliza la librería del motor DSL.

## Actores Secundarios

**Motor DSL** — Interpreta la plantilla y construye el modelo interno.
**Validador de Plantillas** — Verifica la estructura y consistencia del DSL.

---

# 3. Precondiciones

* Existe una plantilla DSL disponible en formato JSON.
* La plantilla cumple con la estructura esperada del DSL.
* El motor DSL se encuentra inicializado.

---

# 4. Postcondiciones

## En caso de éxito

* Se genera un modelo interno del documento.
* La estructura del documento queda validada.
* La plantilla queda lista para resolución de datos.

## En caso de fallo

* No se genera el modelo interno.
* El sistema informa errores de validación.
* Se registra el error para diagnóstico.

---

# 5. Flujo Principal

1. El sistema consumidor solicita interpretar una plantilla DSL.
2. El motor DSL recibe el JSON de la plantilla.
3. El motor parsea la estructura del documento.
4. El motor identifica los elementos:

   * Secciones
   * Bloques
   * Elementos (texto, listas, tablas, etc.)
5. El validador verifica:

   * Estructura válida
   * Tipos de elementos soportados
6. El motor construye el modelo interno del documento.
7. El sistema confirma la interpretación exitosa.
8. El caso de uso finaliza.

---

# 6. Flujos Alternativos

## FA-01 Plantilla inválida

**En el paso 5**

* Si la estructura no es válida:

  * El sistema informa error de validación.
  * Se detiene el procesamiento.
  * El caso de uso finaliza.

---

## FA-02 Elemento no soportado

**En el paso 4**

* Si se detecta un tipo de elemento desconocido:

  * El sistema registra advertencia o error.
  * Puede rechazar la plantilla o ignorar el elemento según configuración.
  * El caso de uso finaliza o continúa.

---

## FA-03 Error de parsing

**En el paso 3**

* Si el JSON es inválido:

  * El sistema informa error de formato.
  * Se detiene el procesamiento.
  * El caso de uso finaliza.

---

## FA-04 Error técnico

**En cualquier punto**

* El sistema informa error interno.
* Registra el evento.
* El caso de uso finaliza.

---

# 7. Reglas de Negocio Relacionadas

* RN-01: Toda plantilla debe cumplir el esquema del DSL.
* RN-02: Solo se permiten tipos de elementos soportados.
* RN-03: La estructura del documento debe ser jerárquica.

---

# 8. Datos Utilizados

## Entrada

* TemplateJson

## Salida

* DocumentoModeloInterno
* EstadoValidación

---

# 9. Criterios de Aceptación

## CA-01 Interpretación exitosa

**Dado** una plantilla válida
**Cuando** se procesa
**Entonces** se genera el modelo interno del documento.

---

## CA-02 Validación de estructura

**Dado** una plantilla inválida
**Cuando** se procesa
**Entonces** el sistema rechaza la plantilla.

---

## CA-03 Manejo de elementos

**Dado** una plantilla con elementos soportados
**Cuando** se interpreta
**Entonces** todos los elementos son reconocidos correctamente.

---

## CA-04 Manejo de error

**Dado** un JSON inválido
**Cuando** se procesa
**Entonces** el sistema informa error y no continúa.

---

# 10. Cambios respecto a versión 1.0

* Versión inicial del caso de uso.

---

# 11. Notas

Este caso de uso es fundamental para el funcionamiento del motor DSL, ya que define la base sobre la cual se ejecutan los procesos posteriores de resolución de datos y renderizado.

---

**Fin del documento**
