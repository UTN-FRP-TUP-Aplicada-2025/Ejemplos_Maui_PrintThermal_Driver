# Caso de Uso: Aplicar Restricciones del Dispositivo

**Código:** CU-23
**Archivo:** CU-23-aplicar-restricciones-dispositivo_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-03-28
**Autor:** Equipo Funcional / Arquitectura

---

# 1. Propósito

Este caso de uso describe el proceso mediante el cual el motor DSL aplica las restricciones definidas en el perfil del dispositivo sobre el documento generado.

El objetivo es garantizar que el documento final cumpla con las limitaciones físicas y lógicas del dispositivo, evitando errores de impresión o representaciones incorrectas.

---

# 2. Actores

## Actor Primario

**Motor DSL** — Responsable de aplicar las restricciones.

## Actores Secundarios

**Perfil de Dispositivo** — Define las limitaciones.
**Documento Adaptado** — Documento previo a la aplicación de restricciones.

---

# 3. Precondiciones

* Existe un documento procesado (iteraciones, condiciones, datos resueltos).
* Existe un perfil de dispositivo válido (CU-22).
* El documento ya fue adaptado a nivel general (CU-12).
* El motor DSL se encuentra inicializado.

---

# 4. Postcondiciones

## En caso de éxito

* El documento respeta las restricciones del dispositivo.
* Se ajustan tamaños, formatos y contenido.
* El documento queda listo para renderizado final.

## En caso de fallo

* No se logra aplicar correctamente alguna restricción.
* El sistema informa errores o advertencias.
* Se registra el evento para diagnóstico.

---

# 5. Flujo Principal

1. El sistema inicia la aplicación de restricciones.
2. El motor DSL recibe:

   * DocumentoAdaptado
   * PerfilDispositivo
3. El sistema analiza restricciones del perfil:

   * ancho máximo de línea
   * cantidad de caracteres por fila
   * soporte de imágenes
   * soporte de QR
4. El sistema recorre el documento.
5. Para cada elemento:

   * valida cumplimiento de restricciones
   * ajusta contenido si es necesario
6. Se aplican reglas específicas:

   * truncamiento de texto
   * ajuste de alineación
   * división de líneas
7. Se valida la consistencia final del documento.
8. El sistema confirma la aplicación de restricciones.
9. El caso de uso finaliza.

---

# 6. Flujos Alternativos

## FA-01 Contenido excede límites

**En el paso 5**

* Si el contenido supera el ancho permitido:

  * El sistema lo divide o trunca.
  * Registra advertencia.

---

## FA-02 Elemento no soportado

**En el paso 5**

* Si el dispositivo no soporta el elemento:

  * El sistema lo elimina o reemplaza.
  * Registra advertencia.

---

## FA-03 Restricción inconsistente

**En el paso 3**

* Si el perfil contiene restricciones contradictorias:

  * El sistema informa error.
  * Detiene el proceso.

---

## FA-04 Error técnico

**En cualquier punto**

* El sistema informa error interno.
* Registra el evento.
* El caso de uso finaliza.

---

# 7. Reglas de Negocio Relacionadas

* RN-01: Todo documento debe respetar el perfil del dispositivo.
* RN-02: Las restricciones tienen prioridad sobre la plantilla.
* RN-03: Se deben aplicar reglas de degradación controlada.

---

# 8. Datos Utilizados

## Entrada

* DocumentoAdaptado
* PerfilDispositivo

## Salida

* DocumentoRestringido

---

# 9. Criterios de Aceptación

## CA-01 Aplicación correcta

**Dado** un documento y perfil válidos
**Cuando** se aplican restricciones
**Entonces** el documento cumple con los límites.

---

## CA-02 Ajuste de contenido

**Dado** contenido excedido
**Cuando** se procesa
**Entonces** se ajusta correctamente.

---

## CA-03 Eliminación de elementos no soportados

**Dado** elementos incompatibles
**Cuando** se procesa
**Entonces** se eliminan o reemplazan.

---

## CA-04 Manejo de errores

**Dado** un problema en la aplicación
**Cuando** ocurre
**Entonces** el sistema informa el error.

---

# 10. Cambios respecto a versión 1.0

* Versión inicial del caso de uso.

---

# 11. Notas

Este caso de uso es clave para garantizar la compatibilidad final con el hardware, siendo el último punto de control antes del renderizado específico del dispositivo.

---

**Fin del documento**
