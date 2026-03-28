# Caso de Uso: Construir Modelo Interno del Documento

**Código:** CU-03
**Archivo:** CU-03-construir-modelo-interno_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-03-28
**Autor:** Equipo Funcional / Arquitectura

---

# 1. Propósito

Este caso de uso describe el proceso mediante el cual el motor DSL construye una representación interna estructurada del documento a partir de la plantilla interpretada.

El objetivo es generar un modelo intermedio normalizado, independiente del formato de salida, que sirva como base para la resolución de datos, el cálculo de layout y el renderizado.

---

# 2. Actores

## Actor Primario

**Motor DSL** — Responsable de construir el modelo interno.

## Actores Secundarios

**Parser de Plantillas** — Provee la estructura interpretada del DSL.
**Sistema consumidor** — Inicia el proceso.

---

# 3. Precondiciones

* Existe una plantilla DSL válida.
* La plantilla fue interpretada correctamente (CU-01).
* El motor DSL se encuentra inicializado.

---

# 4. Postcondiciones

## En caso de éxito

* Se genera una estructura interna del documento.
* El modelo representa jerárquicamente el contenido.
* El documento queda listo para resolución de datos (CU-02).

## En caso de fallo

* No se genera el modelo interno.
* Se informa el error correspondiente.
* Se registra el evento para diagnóstico.

---

# 5. Flujo Principal

1. El sistema inicia la construcción del modelo interno.
2. El motor DSL recibe la estructura interpretada de la plantilla.
3. El sistema crea el nodo raíz del documento.
4. El motor recorre la plantilla jerárquicamente:

   * Secciones
   * Bloques
   * Elementos
5. Para cada elemento, el sistema:

   * crea un nodo interno correspondiente
   * asigna propiedades estructurales
6. El sistema organiza los nodos en una estructura jerárquica.
7. Se establecen relaciones padre-hijo entre los elementos.
8. El sistema valida la consistencia estructural.
9. Se confirma la creación del modelo interno.
10. El caso de uso finaliza.

---

# 6. Flujos Alternativos

## FA-01 Estructura inconsistente

**En el paso 8**

* Si la jerarquía no es válida:

  * El sistema informa error.
  * Se detiene el proceso.
  * El caso de uso finaliza.

---

## FA-02 Elemento incompleto

**En el paso 5**

* Si un elemento no tiene propiedades necesarias:

  * El sistema registra advertencia o error.
  * Puede omitir el elemento o detener el proceso.

---

## FA-03 Error técnico

**En cualquier punto**

* El sistema informa error interno.
* Registra el evento.
* El caso de uso finaliza.

---

# 7. Reglas de Negocio Relacionadas

* RN-01: Todo documento debe tener un nodo raíz.
* RN-02: La estructura debe ser jerárquica y consistente.
* RN-03: Los elementos deben cumplir con su definición mínima.

---

# 8. Datos Utilizados

## Entrada

* PlantillaInterpretada

## Salida

* DocumentoModeloInterno

---

# 9. Criterios de Aceptación

## CA-01 Creación del modelo

**Dado** una plantilla válida
**Cuando** se construye el modelo
**Entonces** se genera una estructura interna consistente.

---

## CA-02 Jerarquía correcta

**Dado** una plantilla con múltiples niveles
**Cuando** se procesa
**Entonces** se respeta la estructura padre-hijo.

---

## CA-03 Validación estructural

**Dado** una plantilla inconsistente
**Cuando** se procesa
**Entonces** el sistema rechaza la construcción.

---

## CA-04 Manejo de errores

**Dado** un error interno
**Cuando** ocurre
**Entonces** el sistema informa y registra el evento.

---

# 10. Cambios respecto a versión 1.0

* Versión inicial del caso de uso.

---

# 11. Notas

Este modelo interno es clave para desacoplar la plantilla del renderizado, permitiendo que el mismo documento pueda ser procesado por distintos motores de salida.

---

**Fin del documento**
