# Regla Conceptual: Estructura Jerárquica del Documento

**Código:** RC-03
**Archivo:** RC-03-estructura-jerarquica_v1.0.md
**Versión:** 1.0
**Estado:** Aprobada
**Fecha:** 2026-03-28
**Autor:** Equipo Funcional / Arquitectura

---

# 1. Descripción

Esta regla conceptual establece que todo documento procesado por el sistema debe representarse mediante una estructura jerárquica, organizada en forma de árbol de nodos con relaciones padre-hijo.

El objetivo es garantizar una composición coherente del documento, permitiendo al motor DSL interpretar correctamente la organización del contenido y su posterior renderizado.

---

# 2. Motivación

La representación jerárquica permite:

* modelar documentos complejos de manera estructurada
* establecer relaciones claras entre elementos
* facilitar la interpretación por el motor DSL
* habilitar renderizadores flexibles y extensibles

Sin jerarquía, el sistema no podría determinar el orden, agrupación ni contexto de los elementos del documento.

---

# 3. Definición de la Regla

* Todo documento debe estar compuesto por un nodo raíz único.
* Cada nodo puede contener cero o más nodos hijos.
* La relación entre nodos define la estructura del documento.
* No se permiten estructuras planas ni sin relación jerárquica.
* No deben existir ciclos en la relación entre nodos.

---

# 4. Condiciones de Aplicación

* Aplica a todos los documentos antes de su procesamiento por el motor DSL.
* Se valida durante la construcción del modelo interno del documento (CU-05).
* También aplica en la interpretación de plantillas y mapeo de datos.

---

# 5. Resultados Esperados

* El documento se representa como un árbol de nodos válido.
* Cada elemento tiene una posición y relación claramente definida.
* El motor puede recorrer el documento de manera determinística.
* Los renderizadores pueden reproducir fielmente la estructura.

---

# 6. Excepciones

## EX-01 Nodo raíz inexistente o múltiple

Si el documento:

* no posee nodo raíz, o
* posee más de un nodo raíz

Entonces:

* el sistema debe rechazar la estructura
* se debe registrar un error de validación

---

## EX-02 Estructura con ciclos

Si se detectan referencias cíclicas entre nodos:

* el sistema debe invalidar el documento
* se debe detener el procesamiento
* se debe informar el error correspondiente

---

# 7. Criterios de Validación

## CV-01 Nodo raíz único

**Dado** un documento
**Cuando** se analiza su estructura
**Entonces** existe un único nodo raíz

---

## CV-02 Relación jerárquica válida

**Dado** un nodo
**Cuando** se valida
**Entonces** sus hijos están correctamente definidos como descendientes

---

## CV-03 Ausencia de ciclos

**Dado** la estructura del documento
**Cuando** se valida
**Entonces** no existen dependencias cíclicas entre nodos

---

## CV-04 Recorrido determinístico

**Dado** un documento jerárquico válido
**Cuando** el motor lo procesa
**Entonces** puede recorrer todos los nodos siguiendo la jerarquía

---

# 8. Impacto

Esta regla impacta directamente en:

* CU-03 Construcción del modelo interno
* CU-04 Ejecución del motor DSL
* CU-05 Generación de representación abstracta
* CU-14 Validación de plantilla

Es una regla estructural fundamental para el funcionamiento del motor de documentos.

---

# 9. Control de Cambios

| Versión | Fecha      | Descripción                 |
| ------- | ---------- | --------------------------- |
| 1.0     | 2026-03-28 | Versión inicial de la regla |

---

**Fin del documento**
