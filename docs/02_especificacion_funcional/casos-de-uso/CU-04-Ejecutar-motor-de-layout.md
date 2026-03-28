# Caso de Uso: Ejecutar Motor de Layout

**Código:** CU-04
**Archivo:** CU-04-ejecutar-motor-layout_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-03-28
**Autor:** Equipo Funcional / Arquitectura

---

# 1. Propósito

Este caso de uso describe el proceso mediante el cual el motor DSL calcula la disposición física del contenido del documento en función del perfil del dispositivo.

El objetivo es transformar el modelo del documento con datos resueltos en una representación espacial organizada (layout), considerando restricciones como ancho de papel, cantidad de caracteres por línea y capacidades del dispositivo.

---

# 2. Actores

## Actor Primario

**Motor DSL** — Responsable de ejecutar el cálculo de layout.

## Actores Secundarios

**Motor de Layout** — Calcula la disposición del contenido.
**Perfil de Dispositivo** — Provee restricciones y capacidades.

---

# 3. Precondiciones

* Existe un documento con datos resueltos (CU-02).
* Existe un modelo interno del documento (CU-03).
* Se dispone de un perfil de dispositivo válido.
* El motor DSL se encuentra inicializado.

---

# 4. Postcondiciones

## En caso de éxito

* Se genera una representación del documento con layout calculado.
* El contenido queda organizado en líneas, bloques y posiciones.
* El documento queda listo para renderizado.

## En caso de fallo

* No se genera el layout.
* El sistema informa el error correspondiente.
* Se registra el evento para diagnóstico.

---

# 5. Flujo Principal

1. El sistema inicia el cálculo de layout.
2. El motor DSL recibe:

   * Documento con datos resueltos
   * Perfil del dispositivo
3. El motor de layout analiza las restricciones:

   * ancho disponible
   * cantidad de caracteres por línea
   * capacidades del dispositivo
4. El sistema recorre el documento:

   * secciones
   * bloques
   * elementos
5. Para cada elemento, el sistema:

   * calcula tamaño y posición
   * aplica alineación
   * determina saltos de línea
6. En caso de texto:

   * se aplica ajuste de líneas (wrap)
   * se respeta el ancho máximo
7. En caso de listas o tablas:

   * se distribuyen columnas
   * se ajusta contenido al espacio disponible
8. Se genera la estructura final de layout (líneas y posiciones).
9. Se valida consistencia del layout.
10. El sistema confirma la ejecución exitosa.
11. El caso de uso finaliza.

---

# 6. Flujos Alternativos

## FA-01 Desbordamiento de contenido

**En el paso 5**

* Si el contenido excede el ancho:

  * El sistema aplica corte o wrap.
  * O ajusta el contenido según configuración.

---

## FA-02 Elemento no soportado por el dispositivo

**En el paso 3**

* Si el dispositivo no soporta un elemento:

  * El sistema omite o degrada el contenido.
  * Registra advertencia.

---

## FA-03 Error de cálculo

**En el paso 5**

* Si no se puede calcular la posición:

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

* RN-01: El layout debe respetar el ancho del dispositivo.
* RN-02: Todo contenido debe ajustarse a las restricciones del perfil.
* RN-03: El texto debe adaptarse mediante ajuste automático de línea.

---

# 8. Datos Utilizados

## Entrada

* DocumentoConDatosResueltos
* PerfilDispositivo

## Salida

* DocumentoConLayout

---

# 9. Criterios de Aceptación

## CA-01 Layout válido

**Dado** un documento válido
**Cuando** se ejecuta el layout
**Entonces** se genera una estructura organizada.

---

## CA-02 Ajuste de texto

**Dado** texto largo
**Cuando** se procesa
**Entonces** se ajusta al ancho disponible.

---

## CA-03 Adaptación al dispositivo

**Dado** un perfil de impresora
**Cuando** se procesa
**Entonces** el layout respeta sus restricciones.

---

## CA-04 Manejo de errores

**Dado** un problema en el cálculo
**Cuando** ocurre
**Entonces** el sistema informa el error.

---

# 10. Cambios respecto a versión 1.0

* Versión inicial del caso de uso.

---

# 11. Notas

Este caso de uso es uno de los más críticos del sistema, ya que define cómo el documento se adapta al espacio físico disponible, especialmente en dispositivos con limitaciones como impresoras térmicas.

---

**Fin del documento**
