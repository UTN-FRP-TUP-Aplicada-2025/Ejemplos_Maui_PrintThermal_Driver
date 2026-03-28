# Caso de Uso: Adaptar Documento al Perfil del Dispositivo

**Código:** CU-12
**Archivo:** CU-12-adaptar-documento-perfil_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-03-28
**Autor:** Equipo Funcional / Arquitectura

---

# 1. Propósito

Este caso de uso describe el proceso mediante el cual el motor DSL adapta el documento a las capacidades y restricciones definidas en el perfil del dispositivo seleccionado.

El objetivo es garantizar que el documento sea compatible con el hardware de destino, ajustando contenido, formato y comportamiento según las limitaciones del dispositivo.

---

# 2. Actores

## Actor Primario

**Motor DSL** — Responsable de adaptar el documento.

## Actores Secundarios

**Perfil de Dispositivo** — Define restricciones y capacidades.
**Motor de Layout** — Aplica ajustes estructurales.
**Renderizadores** — Consumen el documento adaptado.

---

# 3. Precondiciones

* Existe un documento con datos resueltos (CU-02).
* Existe un perfil de dispositivo seleccionado (CU-11).
* El motor DSL se encuentra inicializado.

---

# 4. Postcondiciones

## En caso de éxito

* El documento es adaptado según el perfil del dispositivo.
* Se ajustan elementos para compatibilidad.
* El documento queda listo para layout y renderizado.

## En caso de fallo

* No se logra adaptar el documento.
* El sistema informa el error correspondiente.
* Se registra el evento para diagnóstico.

---

# 5. Flujo Principal

1. El sistema inicia el proceso de adaptación.
2. El motor DSL recibe:

   * DocumentoConDatosResueltos
   * PerfilDispositivo
3. El sistema analiza las capacidades del perfil:

   * ancho máximo
   * soporte de imágenes
   * soporte de QR
   * estilos disponibles
4. El sistema recorre el documento.
5. Para cada elemento, el sistema:

   * verifica compatibilidad con el dispositivo
   * ajusta propiedades (tamaño, alineación, formato)
6. En caso de contenido no soportado:

   * se transforma (ej. imagen → texto alternativo)
   * o se omite según configuración
7. Se aplican reglas de adaptación globales.
8. Se valida la consistencia del documento adaptado.
9. El sistema confirma la adaptación exitosa.
10. El caso de uso finaliza.

---

# 6. Flujos Alternativos

## FA-01 Elemento no compatible

**En el paso 5**

* Si el dispositivo no soporta el elemento:

  * El sistema lo transforma o elimina.
  * Registra advertencia.

---

## FA-02 Perfil incompleto

**En el paso 3**

* Si faltan datos en el perfil:

  * El sistema informa error.
  * Puede aplicar valores por defecto.

---

## FA-03 Error de adaptación

**En el paso 7**

* Si no se puede aplicar una regla:

  * El sistema detiene el proceso.
  * Informa el error.

---

## FA-04 Error técnico

**En cualquier punto**

* El sistema informa error interno.
* Registra el evento.
* El caso de uso finaliza.

---

# 7. Reglas de Negocio Relacionadas

* RN-01: Todo documento debe adaptarse al perfil antes de renderizarse.
* RN-02: Los elementos deben respetar las capacidades del dispositivo.
* RN-03: Se deben aplicar reglas de degradación controlada.

---

# 8. Datos Utilizados

## Entrada

* DocumentoConDatosResueltos
* PerfilDispositivo

## Salida

* DocumentoAdaptado

---

# 9. Criterios de Aceptación

## CA-01 Adaptación exitosa

**Dado** un documento y perfil válidos
**Cuando** se adapta
**Entonces** el documento es compatible con el dispositivo.

---

## CA-02 Manejo de limitaciones

**Dado** un dispositivo limitado
**Cuando** se procesa
**Entonces** el contenido se ajusta correctamente.

---

## CA-03 Transformación de elementos

**Dado** elementos no soportados
**Cuando** se adapta
**Entonces** se transforman o eliminan.

---

## CA-04 Manejo de errores

**Dado** un problema en la adaptación
**Cuando** ocurre
**Entonces** el sistema informa el error.

---

# 10. Cambios respecto a versión 1.0

* Versión inicial del caso de uso.

---

# 11. Notas

Este caso de uso es fundamental para garantizar la portabilidad del sistema, permitiendo que un mismo documento funcione correctamente en diferentes dispositivos sin necesidad de redefinir su estructura.

---

**Fin del documento**
