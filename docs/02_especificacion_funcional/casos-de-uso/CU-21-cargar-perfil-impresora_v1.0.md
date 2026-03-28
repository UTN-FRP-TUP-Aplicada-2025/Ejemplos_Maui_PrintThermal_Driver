# Caso de Uso: Cargar Perfil de Impresora

**Código:** CU-21
**Archivo:** CU-21-cargar-perfil-impresora_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-03-28
**Autor:** Equipo Funcional / Arquitectura

---

# 1. Propósito

Este caso de uso describe el proceso mediante el cual el sistema carga el perfil de una impresora, el cual define sus capacidades, limitaciones y configuraciones específicas.

El objetivo es disponer de una abstracción del dispositivo que permita al motor DSL adaptar y renderizar documentos correctamente sin depender directamente del hardware.

---

# 2. Actores

## Actor Primario

**Sistema consumidor** — Solicita la carga del perfil.

## Actores Secundarios

**Repositorio de Perfiles** — Fuente del perfil (local o remoto).
**Motor DSL** — Utiliza el perfil en etapas posteriores.

---

# 3. Precondiciones

* Existe un perfil de impresora disponible.
* El sistema tiene acceso al repositorio de perfiles.
* El formato del perfil está definido (JSON).
* El motor DSL se encuentra inicializado.

---

# 4. Postcondiciones

## En caso de éxito

* El perfil de impresora es cargado correctamente.
* Queda disponible para adaptación y renderizado.
* Se valida su estructura básica.

## En caso de fallo

* El perfil no se carga.
* El sistema informa el error correspondiente.
* Se registra el evento para diagnóstico.

---

# 5. Flujo Principal

1. El sistema solicita la carga de un perfil de impresora.
2. El sistema determina la fuente:

   * archivo local
   * endpoint REST
3. El sistema recupera el JSON del perfil.
4. El sistema valida la sintaxis del JSON.
5. El sistema valida estructura básica:

   * ancho de papel
   * soporte de imágenes
   * comandos soportados
6. El perfil se carga en memoria.
7. El sistema confirma la carga exitosa.
8. El caso de uso finaliza.

---

# 6. Flujos Alternativos

## FA-01 Perfil no encontrado

**En el paso 2**

* Si el perfil no existe:

  * El sistema informa error.
  * No continúa el proceso.

---

## FA-02 JSON inválido

**En el paso 4**

* Si el JSON es incorrecto:

  * El sistema informa error de parseo.
  * Detiene el proceso.

---

## FA-03 Estructura incompleta

**En el paso 5**

* Si faltan propiedades clave:

  * El sistema informa error o advertencia.
  * Puede rechazar el perfil.

---

## FA-04 Error técnico

**En cualquier punto**

* El sistema informa error interno.
* Registra el evento.
* El caso de uso finaliza.

---

# 7. Reglas de Negocio Relacionadas

* RN-01: Todo perfil debe estar definido en formato JSON.
* RN-02: El perfil debe ser independiente del código.
* RN-03: Debe permitir múltiples dispositivos.

---

# 8. Datos Utilizados

## Entrada

* FuentePerfil

## Salida

* PerfilImpresora

---

# 9. Criterios de Aceptación

## CA-01 Carga exitosa

**Dado** un perfil válido
**Cuando** se carga
**Entonces** queda disponible para uso.

---

## CA-02 Validación de formato

**Dado** un JSON inválido
**Cuando** se procesa
**Entonces** el sistema lo rechaza.

---

## CA-03 Independencia del origen

**Dado** perfiles locales o remotos
**Cuando** se cargan
**Entonces** se procesan correctamente.

---

## CA-04 Manejo de errores

**Dado** un problema en la carga
**Cuando** ocurre
**Entonces** el sistema informa el error.

---

# 10. Cambios respecto a versión 1.0

* Versión inicial del caso de uso.

---

# 11. Notas

Este caso de uso es clave para abstraer las características del hardware, permitiendo que el sistema opere de manera desacoplada y extensible frente a distintos tipos de impresoras.

---

**Fin del documento**
