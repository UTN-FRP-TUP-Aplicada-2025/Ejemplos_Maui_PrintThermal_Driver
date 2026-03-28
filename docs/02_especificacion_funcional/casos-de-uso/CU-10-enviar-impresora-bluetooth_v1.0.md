# Caso de Uso: Enviar Documento a Impresora Bluetooth

**Código:** CU-10
**Archivo:** CU-10-enviar-impresora-bluetooth_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-03-28
**Autor:** Equipo Funcional / Arquitectura

---

# 1. Propósito

Este caso de uso describe el proceso mediante el cual el sistema envía un documento ya renderizado en formato ESC/POS a una impresora térmica conectada vía Bluetooth.

El objetivo es permitir la impresión física del documento desde dispositivos móviles, garantizando la correcta transmisión de datos y la ejecución de comandos por parte del dispositivo.

---

# 2. Actores

## Actor Primario

**Usuario de la aplicación** — Persona que solicita la impresión del documento.

## Actores Secundarios

**Sistema consumidor** — Orquesta el envío del documento.
**Módulo de Comunicación Bluetooth** — Gestiona la conexión con la impresora.
**Impresora térmica** — Dispositivo que recibe y ejecuta los comandos ESC/POS.

---

# 3. Precondiciones

* Existe una secuencia de comandos ESC/POS válida (CU-06).
* La impresora está encendida y disponible.
* El dispositivo tiene Bluetooth habilitado.
* La impresora está emparejada o disponible para conexión.
* El sistema tiene permisos de acceso a Bluetooth.

---

# 4. Postcondiciones

## En caso de éxito

* El documento es enviado correctamente a la impresora.
* La impresora ejecuta los comandos y genera la impresión.
* Se confirma la operación al usuario.

## En caso de fallo

* El documento no se imprime.
* El sistema informa el error correspondiente.
* Se registra el evento para diagnóstico.

---

# 5. Flujo Principal

1. El usuario solicita imprimir el documento.
2. El sistema verifica la disponibilidad de la impresora.
3. El sistema inicia la conexión Bluetooth.
4. El módulo Bluetooth establece conexión con la impresora.
5. El sistema envía la secuencia de comandos ESC/POS.
6. La impresora recibe los datos.
7. La impresora ejecuta los comandos:

   * imprime el contenido
   * realiza acciones (corte, salto, etc.)
8. El sistema confirma el envío exitoso.
9. Se notifica al usuario.
10. El caso de uso finaliza.

---

# 6. Flujos Alternativos

## FA-01 Impresora no disponible

**En el paso 2**

* Si no se detecta la impresora:

  * El sistema informa al usuario.
  * No se inicia la conexión.

---

## FA-02 Error de conexión Bluetooth

**En el paso 4**

* Si falla la conexión:

  * El sistema informa el error.
  * Permite reintentar.

---

## FA-03 Error en envío de datos

**En el paso 5**

* Si la transmisión falla:

  * El sistema interrumpe el proceso.
  * Informa el error.

---

## FA-04 Impresora no responde

**En el paso 6**

* Si la impresora no responde:

  * El sistema informa problema de comunicación.
  * Puede reintentar o finalizar.

---

## FA-05 Error técnico

**En cualquier punto**

* El sistema informa error interno.
* Registra el evento.
* El caso de uso finaliza.

---

# 7. Reglas de Negocio Relacionadas

* RN-01: Solo se pueden enviar comandos ESC/POS válidos.
* RN-02: Debe existir conexión activa antes del envío.
* RN-03: El sistema debe informar el estado de la operación.

---

# 8. Datos Utilizados

## Entrada

* SecuenciaComandosESC_POS
* ConfiguracionBluetooth

## Salida

* EstadoImpresion

---

# 9. Criterios de Aceptación

## CA-01 Envío exitoso

**Dado** una impresora disponible
**Cuando** se envía el documento
**Entonces** se imprime correctamente.

---

## CA-02 Conexión Bluetooth

**Dado** una impresora emparejada
**Cuando** se inicia la conexión
**Entonces** se establece correctamente.

---

## CA-03 Manejo de errores

**Dado** un problema de conexión o envío
**Cuando** ocurre
**Entonces** el sistema informa al usuario.

---

## CA-04 Confirmación de operación

**Dado** envío exitoso
**Cuando** finaliza
**Entonces** el usuario recibe confirmación.

---

# 10. Cambios respecto a versión 1.0

* Versión inicial del caso de uso.

---

# 11. Notas

Este caso de uso conecta el mundo lógico del motor DSL con el hardware real, siendo clave para la operación en escenarios móviles con impresoras térmicas.

---

**Fin del documento**
