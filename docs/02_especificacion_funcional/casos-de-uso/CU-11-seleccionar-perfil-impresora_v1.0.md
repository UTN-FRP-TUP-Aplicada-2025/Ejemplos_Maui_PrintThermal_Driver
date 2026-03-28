# Caso de Uso: Seleccionar Perfil de Impresora

**Código:** CU-11
**Archivo:** CU-11-seleccionar-perfil-impresora_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-03-28
**Autor:** Equipo Funcional / Arquitectura

---

# 1. Propósito

Este caso de uso describe el proceso mediante el cual el sistema selecciona el perfil de impresora adecuado para adaptar el documento a las capacidades del dispositivo de salida.

El objetivo es garantizar que el procesamiento del documento (layout y renderizado) se realice considerando las restricciones físicas y funcionales de la impresora.

---

# 2. Actores

## Actor Primario

**Sistema consumidor** — Aplicación que determina o solicita el perfil.

## Actores Secundarios

**Motor DSL** — Utiliza el perfil para adaptar el documento.
**Repositorio de perfiles** — Provee los perfiles disponibles.
**Usuario (opcional)** — Puede seleccionar manualmente el perfil.

---

# 3. Precondiciones

* Existen perfiles de impresora definidos.
* El sistema tiene acceso al repositorio de perfiles.
* El motor DSL se encuentra inicializado.

---

# 4. Postcondiciones

## En caso de éxito

* Se selecciona un perfil de impresora válido.
* El perfil queda disponible para el procesamiento del documento.
* Se asocian las capacidades del dispositivo al flujo de renderizado.

## En caso de fallo

* No se selecciona ningún perfil.
* El sistema informa el error correspondiente.
* Se registra el evento para diagnóstico.

---

# 5. Flujo Principal

1. El sistema requiere un perfil de impresora.
2. El sistema obtiene la lista de perfiles disponibles.
3. El sistema determina el perfil a utilizar:

   * por configuración previa
   * por detección automática
   * por selección del usuario (si aplica)
4. El sistema carga el perfil seleccionado.
5. El sistema valida el perfil:

   * formato correcto
   * propiedades requeridas
6. El perfil se asigna al contexto del documento.
7. El sistema confirma la selección.
8. El caso de uso finaliza.

---

# 6. Flujos Alternativos

## FA-01 Perfil no encontrado

**En el paso 3**

* Si no existe el perfil requerido:

  * El sistema informa error.
  * Puede usar un perfil por defecto.

---

## FA-02 Perfil inválido

**En el paso 5**

* Si el perfil no cumple la estructura:

  * El sistema rechaza el perfil.
  * Solicita uno válido.

---

## FA-03 Selección manual

**En el paso 3**

* El usuario selecciona un perfil:

  * El sistema valida la selección.
  * Continúa el flujo.

---

## FA-04 Error técnico

**En cualquier punto**

* El sistema informa error interno.
* Registra el evento.
* El caso de uso finaliza.

---

# 7. Reglas de Negocio Relacionadas

* RN-01: Todo documento debe procesarse con un perfil de dispositivo.
* RN-02: Debe existir un perfil por defecto.
* RN-03: El perfil debe ser válido antes de su uso.

---

# 8. Datos Utilizados

## Entrada

* IdentificadorPerfil (opcional)
* ListaPerfilesDisponibles

## Salida

* PerfilDispositivoSeleccionado

---

# 9. Criterios de Aceptación

## CA-01 Selección exitosa

**Dado** perfiles disponibles
**Cuando** se selecciona uno válido
**Entonces** el sistema lo asigna correctamente.

---

## CA-02 Perfil por defecto

**Dado** ausencia de selección
**Cuando** se requiere un perfil
**Entonces** se utiliza el perfil por defecto.

---

## CA-03 Validación de perfil

**Dado** un perfil inválido
**Cuando** se selecciona
**Entonces** el sistema lo rechaza.

---

## CA-04 Manejo de errores

**Dado** un problema en la selección
**Cuando** ocurre
**Entonces** el sistema informa el error.

---

# 10. Cambios respecto a versión 1.0

* Versión inicial del caso de uso.

---

# 11. Notas

Este caso de uso es clave para el desacople del hardware, permitiendo que el mismo documento se adapte dinámicamente a distintas impresoras sin cambios en la lógica del sistema.

---

**Fin del documento**
