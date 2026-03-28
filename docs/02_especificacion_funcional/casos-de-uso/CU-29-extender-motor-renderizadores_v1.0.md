# Caso de Uso: Extender Motor con Nuevos Renderizadores

**Código:** CU-29
**Archivo:** CU-29-extender-motor-renderizadores_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-03-28
**Autor:** Equipo Funcional / Arquitectura

---

# 1. Propósito

Este caso de uso describe el proceso mediante el cual el motor DSL es extendido mediante la incorporación de nuevos renderizadores (adaptadores de salida).

El objetivo es permitir la generación de documentos en distintos formatos o dispositivos sin modificar el núcleo del motor, manteniendo un diseño desacoplado y extensible.

---

# 2. Actores

## Actor Primario

**Desarrollador** — Implementa nuevos renderizadores.

## Actores Secundarios

**Motor DSL** — Registra y utiliza los renderizadores.
**Aplicación consumidora** — Invoca el renderizador correspondiente.

---

# 3. Precondiciones

* El motor DSL se encuentra disponible y configurado (CU-28).
* Existe una interfaz o contrato de renderización definido.
* El desarrollador tiene acceso al código de extensión.

---

# 4. Postcondiciones

## En caso de éxito

* El nuevo renderizador queda registrado en el motor.
* Puede ser utilizado en el flujo de generación de documentos.
* No se modifica el núcleo del motor.

## En caso de fallo

* El renderizador no es compatible.
* El sistema informa errores.
* No se registra en el motor.

---

# 5. Flujo Principal

1. El desarrollador define un nuevo renderizador:

   * implementa la interfaz del motor
2. El renderizador recibe:

   * documento abstracto
   * perfil de dispositivo (opcional)
3. El renderizador implementa la lógica de salida:

   * formato específico (ej. PDF, imagen, etc.)
4. El desarrollador registra el renderizador en el motor.
5. La aplicación configura el uso del nuevo renderizador.
6. El motor invoca el renderizador durante la ejecución.
7. Se genera la salida correspondiente.
8. El sistema confirma la extensión exitosa.
9. El caso de uso finaliza.

---

# 6. Flujos Alternativos

## FA-01 Implementación inválida

**En el paso 1**

* Si no cumple la interfaz:

  * El sistema rechaza el renderizador.
  * Informa error.

---

## FA-02 Registro fallido

**En el paso 4**

* Si no se puede registrar:

  * El sistema informa error.
  * No se utiliza el renderizador.

---

## FA-03 Error en ejecución

**En el paso 6**

* Si el renderizador falla:

  * El sistema informa error.
  * Puede intentar otro renderizador.

---

## FA-04 Error técnico

**En cualquier punto**

* El sistema informa error interno.
* Registra el evento.
* El caso de uso finaliza.

---

# 7. Reglas de Negocio Relacionadas

* RN-01: El motor debe ser extensible sin modificación del core.
* RN-02: Los renderizadores deben implementar un contrato común.
* RN-03: Debe permitirse la incorporación de nuevos formatos.

---

# 8. Datos Utilizados

## Entrada

* DocumentoAbstracto
* PerfilDispositivo (opcional)

## Salida

* DocumentoRenderizado

---

# 9. Criterios de Aceptación

## CA-01 Extensión exitosa

**Dado** un renderizador válido
**Cuando** se registra
**Entonces** puede utilizarse.

---

## CA-02 Cumplimiento de interfaz

**Dado** una implementación
**Cuando** se valida
**Entonces** cumple el contrato del motor.

---

## CA-03 Ejecución correcta

**Dado** un documento
**Cuando** se renderiza
**Entonces** se genera la salida esperada.

---

## CA-04 Independencia del núcleo

**Dado** la incorporación de un renderizador
**Cuando** se integra
**Entonces** no se modifica el core del motor.

---

# 10. Cambios respecto a versión 1.0

* Versión inicial del caso de uso.

---

# 11. Notas

Este caso de uso es clave para la evolución del sistema, permitiendo incorporar nuevos formatos y dispositivos a futuro sin afectar la arquitectura central basada en DSL.

---

**Fin del documento**
