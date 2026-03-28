# Caso de Uso: Integrar Motor en Aplicación .NET MAUI

**Código:** CU-27
**Archivo:** CU-27-integrar-motor-maui_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-03-28
**Autor:** Equipo Funcional / Arquitectura

---

# 1. Propósito

Este caso de uso describe el proceso mediante el cual el motor DSL es integrado dentro de una aplicación desarrollada en .NET MAUI.

El objetivo es permitir que la aplicación consuma la librería del motor para generar, visualizar e imprimir documentos de manera desacoplada, reutilizable y multiplataforma.

---

# 2. Actores

## Actor Primario

**Aplicación .NET MAUI** — Consume el motor DSL.

## Actores Secundarios

**Motor DSL (Librería)** — Provee la funcionalidad de generación de documentos.
**Sistema Operativo** — Android / macOS.
**Dispositivo de Impresión** — Impresora térmica Bluetooth.

---

# 3. Precondiciones

* Existe la librería del motor DSL compilada.
* La aplicación .NET MAUI está configurada.
* Se dispone de permisos necesarios (Bluetooth, red).
* Existen plantillas, datos y perfiles accesibles.

---

# 4. Postcondiciones

## En caso de éxito

* El motor DSL es consumido correctamente por la aplicación.
* La aplicación puede generar documentos.
* Se habilita la impresión y vista previa.

## En caso de fallo

* El motor no se integra correctamente.
* La aplicación no puede utilizar sus funcionalidades.
* Se informa el error correspondiente.

---

# 5. Flujo Principal

1. El desarrollador agrega la librería del motor DSL al proyecto MAUI.
2. La aplicación inicializa el motor.
3. La aplicación configura dependencias:

   * fuente de datos
   * repositorio de plantillas
   * perfiles de dispositivos
4. La aplicación invoca el motor para generar un documento.
5. El motor ejecuta el flujo completo:

   * carga de datos
   * carga de plantilla
   * resolución
   * layout
   * adaptación
   * renderizado
6. La aplicación recibe el resultado:

   * vista previa
   * bytes para impresión
7. La aplicación utiliza el resultado:

   * muestra en UI
   * envía a impresora Bluetooth
8. El sistema confirma la integración funcional.
9. El caso de uso finaliza.

---

# 6. Flujos Alternativos

## FA-01 Error de integración

**En el paso 1**

* Si la librería no es compatible:

  * El sistema informa error.
  * No continúa el proceso.

---

## FA-02 Error de inicialización

**En el paso 2**

* Si el motor no inicia:

  * El sistema informa error.
  * Detiene el flujo.

---

## FA-03 Error en ejecución

**En el paso 5**

* Si falla alguna etapa:

  * El sistema informa el error.
  * No se genera el documento.

---

## FA-04 Permisos insuficientes

**En el paso 7**

* Si no hay permisos (ej. Bluetooth):

  * El sistema informa el problema.
  * No realiza la acción.

---

# 7. Reglas de Negocio Relacionadas

* RN-01: El motor debe ser consumido como librería.
* RN-02: La aplicación no debe contener lógica de renderizado.
* RN-03: Debe soportarse multiplataforma.

---

# 8. Datos Utilizados

## Entrada

* ConfiguraciónAplicación
* PlantillaDSL
* DocumentoDatosJSON
* PerfilDispositivo

## Salida

* DocumentoRenderizado
* ResultadoImpresión

---

# 9. Criterios de Aceptación

## CA-01 Integración exitosa

**Dado** una aplicación configurada
**Cuando** se integra el motor
**Entonces** puede generar documentos.

---

## CA-02 Ejecución completa

**Dado** datos válidos
**Cuando** se invoca el motor
**Entonces** se ejecuta el flujo completo.

---

## CA-03 Soporte multiplataforma

**Dado** distintos sistemas operativos
**Cuando** se ejecuta
**Entonces** funciona correctamente.

---

## CA-04 Manejo de errores

**Dado** un problema en la integración
**Cuando** ocurre
**Entonces** el sistema informa el error.

---

# 10. Cambios respecto a versión 1.0

* Versión inicial del caso de uso.

---

# 11. Notas

Este caso de uso materializa el objetivo principal del sistema: encapsular toda la lógica en un motor reutilizable que pueda ser consumido por distintas aplicaciones, manteniendo un enfoque desacoplado y basado en configuración.

---

**Fin del documento**
