# Caso de Uso: Validar Perfil de Dispositivo

**Código:** CU-22
**Archivo:** CU-22-validar-perfil-dispositivo_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-03-28
**Autor:** Equipo Funcional / Arquitectura

---

# 1. Propósito

Este caso de uso describe el proceso mediante el cual el sistema valida el perfil de un dispositivo (impresora u otro medio de salida) para asegurar que cumple con la estructura, reglas y consistencia requeridas por el motor DSL.

El objetivo es garantizar que el perfil pueda ser utilizado de forma segura en las etapas de adaptación y renderizado, evitando errores derivados de configuraciones inválidas o incompletas.

---

# 2. Actores

## Actor Primario

**Motor DSL** — Responsable de validar el perfil.

## Actores Secundarios

**Sistema consumidor** — Provee el perfil del dispositivo.
**Repositorio de Perfiles** — Fuente de origen del perfil (opcional).

---

# 3. Precondiciones

* Existe un perfil de dispositivo cargado (CU-21).
* El esquema esperado del perfil está definido.
* El motor DSL se encuentra inicializado.

---

# 4. Postcondiciones

## En caso de éxito

* El perfil es considerado válido.
* Se habilita su uso en el flujo de procesamiento.
* Se registra la validación exitosa.

## En caso de fallo

* El perfil es rechazado.
* Se informan los errores detectados.
* No puede utilizarse en etapas posteriores.

---

# 5. Flujo Principal

1. El sistema inicia la validación del perfil.
2. El motor DSL recibe el perfil de dispositivo.
3. El sistema valida la sintaxis del JSON.
4. El sistema valida la estructura:

   * propiedades obligatorias (ej. ancho, encoding)
   * capacidades declaradas
5. El sistema valida reglas del perfil:

   * valores permitidos
   * coherencia entre propiedades
6. El sistema valida compatibilidad general con el motor.
7. Se construye un resultado de validación.
8. El sistema confirma la validación.
9. El caso de uso finaliza.

---

# 6. Flujos Alternativos

## FA-01 JSON inválido

**En el paso 3**

* Si el JSON no es válido:

  * El sistema informa error de sintaxis.
  * Detiene el proceso.

---

## FA-02 Estructura incompleta

**En el paso 4**

* Si faltan propiedades obligatorias:

  * El sistema rechaza el perfil.
  * Informa inconsistencias.

---

## FA-03 Valores inválidos

**En el paso 5**

* Si los valores no son permitidos:

  * El sistema informa error.
  * Detiene el proceso.

---

## FA-04 Incompatibilidad con el motor

**En el paso 6**

* Si el perfil no es compatible:

  * El sistema informa error.
  * No permite su uso.

---

## FA-05 Error técnico

**En cualquier punto**

* El sistema informa error interno.
* Registra el evento.
* El caso de uso finaliza.

---

# 7. Reglas de Negocio Relacionadas

* RN-01: Todo perfil debe validarse antes de su uso.
* RN-02: Debe cumplir el esquema definido por el motor.
* RN-03: Las capacidades declaradas deben ser coherentes.

---

# 8. Datos Utilizados

## Entrada

* PerfilDispositivo

## Salida

* ResultadoValidacionPerfil

---

# 9. Criterios de Aceptación

## CA-01 Validación exitosa

**Dado** un perfil correcto
**Cuando** se valida
**Entonces** se aprueba sin errores.

---

## CA-02 Detección de errores

**Dado** un perfil incorrecto
**Cuando** se valida
**Entonces** se informan inconsistencias.

---

## CA-03 Rechazo de perfil inválido

**Dado** errores críticos
**Cuando** se valida
**Entonces** no se permite su uso.

---

## CA-04 Consistencia de capacidades

**Dado** un perfil válido
**Cuando** se valida
**Entonces** las capacidades son coherentes.

---

# 10. Cambios respecto a versión 1.0

* Versión inicial del caso de uso.

---

# 11. Notas

Este caso de uso es fundamental para garantizar la correcta interacción entre el motor DSL y los dispositivos, asegurando que las capacidades declaradas sean confiables y utilizables.

---

**Fin del documento**
