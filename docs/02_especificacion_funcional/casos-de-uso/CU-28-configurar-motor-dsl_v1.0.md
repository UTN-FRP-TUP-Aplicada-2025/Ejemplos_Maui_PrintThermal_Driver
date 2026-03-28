# Caso de Uso: Configurar Motor DSL

**Código:** CU-28
**Archivo:** CU-28-configurar-motor-dsl_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-03-28
**Autor:** Equipo Funcional / Arquitectura

---

# 1. Propósito

Este caso de uso describe el proceso mediante el cual se configura el motor DSL antes de su ejecución, definiendo fuentes de datos, repositorios de plantillas, perfiles de dispositivos y comportamientos generales.

El objetivo es permitir que el motor opere de forma flexible y desacoplada, adaptándose a distintos entornos sin necesidad de modificar su implementación interna.

---

# 2. Actores

## Actor Primario

**Aplicación consumidora** — Define la configuración del motor.

## Actores Secundarios

**Motor DSL** — Recibe y aplica la configuración.
**Servicios externos** — APIs de datos, plantillas y perfiles.

---

# 3. Precondiciones

* El motor DSL se encuentra disponible (CU-27).
* La aplicación ha inicializado el entorno de ejecución.
* Existen fuentes configurables (locales o remotas).

---

# 4. Postcondiciones

## En caso de éxito

* El motor queda configurado correctamente.
* Se definen fuentes y estrategias de ejecución.
* El sistema está listo para procesar documentos.

## En caso de fallo

* La configuración es inválida o incompleta.
* El sistema informa errores.
* No se permite ejecutar el motor.

---

# 5. Flujo Principal

1. La aplicación inicia la configuración del motor.
2. Se definen fuentes de datos:

   * local
   * API REST
3. Se definen repositorios de plantillas:

   * local
   * remoto
4. Se definen repositorios de perfiles de dispositivos.
5. Se configuran parámetros generales:

   * modo debug
   * manejo de errores
   * logging
6. Se registran adaptadores de salida:

   * ESC/POS
   * vista previa
   * texto plano
7. El motor valida la configuración recibida.
8. El sistema confirma la configuración.
9. El caso de uso finaliza.

---

# 6. Flujos Alternativos

## FA-01 Configuración incompleta

**En el paso 7**

* Si faltan elementos requeridos:

  * El sistema informa error.
  * No permite continuar.

---

## FA-02 Fuente inválida

**En el paso 2 o 3**

* Si una fuente no es accesible:

  * El sistema informa error.
  * Puede permitir fallback.

---

## FA-03 Adaptador no registrado

**En el paso 6**

* Si no existe un adaptador requerido:

  * El sistema informa error.
  * No permite ejecución.

---

## FA-04 Error técnico

**En cualquier punto**

* El sistema informa error interno.
* Registra el evento.
* El caso de uso finaliza.

---

# 7. Reglas de Negocio Relacionadas

* RN-01: El motor debe ser configurable sin modificar código.
* RN-02: Debe soportar múltiples fuentes de datos.
* RN-03: Debe permitir distintos adaptadores de salida.

---

# 8. Datos Utilizados

## Entrada

* ConfiguracionMotor

## Salida

* MotorConfigurado

---

# 9. Criterios de Aceptación

## CA-01 Configuración exitosa

**Dado** una configuración válida
**Cuando** se aplica
**Entonces** el motor queda operativo.

---

## CA-02 Validación de configuración

**Dado** una configuración inválida
**Cuando** se procesa
**Entonces** el sistema informa errores.

---

## CA-03 Soporte de múltiples fuentes

**Dado** distintas fuentes
**Cuando** se configuran
**Entonces** el motor las utiliza correctamente.

---

## CA-04 Registro de adaptadores

**Dado** adaptadores definidos
**Cuando** se configura
**Entonces** se registran correctamente.

---

# 10. Cambios respecto a versión 1.0

* Versión inicial del caso de uso.

---

# 11. Notas

Este caso de uso es fundamental para lograr un motor verdaderamente desacoplado, donde su comportamiento se define por configuración y no por implementación, alineándose con el enfoque DSL del sistema.

---

**Fin del documento**
