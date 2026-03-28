# Caso de Uso: Manejar Errores de Impresión

**Código:** CU-32
**Archivo:** CU-32-manejar-errores-impresion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-03-28
**Autor:** Equipo Funcional / Arquitectura

---

# 1. Propósito

Este caso de uso describe el proceso mediante el cual el sistema detecta, gestiona y comunica errores ocurridos durante el envío o ejecución de la impresión en dispositivos (principalmente impresoras térmicas Bluetooth).

El objetivo es garantizar confiabilidad operativa, manejo adecuado de fallos y capacidad de recuperación ante problemas de conectividad, hardware o incompatibilidad.

---

# 2. Actores

## Actor Primario

**Aplicación .NET MAUI** — Ejecuta la impresión y gestiona errores.

## Actores Secundarios

**Motor DSL** — Provee los datos renderizados.
**Dispositivo de Impresión** — Impresora térmica Bluetooth.
**Sistema Operativo** — Gestiona la conectividad (Bluetooth).

---

# 3. Precondiciones

* Existe un documento renderizado (CU-06).
* Existe un perfil de dispositivo válido (CU-22).
* El dispositivo de impresión está configurado.
* La aplicación cuenta con permisos de Bluetooth.

---

# 4. Postcondiciones

## En caso de éxito

* La impresión se realiza correctamente.
* Se registra la operación.

## En caso de fallo

* El error es detectado y gestionado.
* Se informa al usuario o sistema.
* Se registra el evento para diagnóstico o reintento.

---

# 5. Flujo Principal

1. La aplicación inicia el proceso de impresión.
2. Se establece conexión con el dispositivo:

   * Bluetooth
3. Se envían los datos renderizados (ESC/POS).
4. El sistema monitorea la operación.
5. Se detecta el estado de la impresión:

   * éxito
   * error
6. En caso de error, el sistema:

   * identifica el tipo de fallo
   * construye un reporte
7. El sistema notifica al usuario o aplicación.
8. Se registra el evento.
9. El caso de uso finaliza.

---

# 6. Flujos Alternativos

## FA-01 Dispositivo no disponible

**En el paso 2**

* Si la impresora no está conectada:

  * El sistema informa error.
  * Puede sugerir reconexión.

---

## FA-02 Error de conexión Bluetooth

**En el paso 2**

* Si falla la conexión:

  * El sistema informa error.
  * Permite reintentar.

---

## FA-03 Error durante envío

**En el paso 3**

* Si falla el envío de datos:

  * El sistema informa error.
  * Puede cancelar o reintentar.

---

## FA-04 Impresora sin papel o error físico

**En el paso 5**

* Si el dispositivo reporta error:

  * El sistema informa el problema.
  * Detiene la operación.

---

## FA-05 Incompatibilidad de comandos

**En el paso 3**

* Si el dispositivo no soporta comandos:

  * El sistema informa error.
  * Puede aplicar degradación.

---

## FA-06 Error técnico

**En cualquier punto**

* El sistema informa error interno.
* Registra el evento.

---

# 7. Reglas de Negocio Relacionadas

* RN-01: Toda impresión debe monitorearse.
* RN-02: Los errores deben clasificarse.
* RN-03: Debe permitirse reintento de impresión.
* RN-04: Debe registrarse trazabilidad de impresión.

---

# 8. Datos Utilizados

## Entrada

* DocumentoESC_POS
* PerfilDispositivo
* ConfiguracionImpresion

## Salida

* ResultadoImpresion
* ReporteErrorImpresion

---

# 9. Criterios de Aceptación

## CA-01 Impresión exitosa

**Dado** un documento válido
**Cuando** se imprime
**Entonces** se completa correctamente.

---

## CA-02 Detección de error

**Dado** un fallo
**Cuando** ocurre
**Entonces** el sistema lo detecta.

---

## CA-03 Notificación de error

**Dado** un error de impresión
**Cuando** se produce
**Entonces** se informa claramente.

---

## CA-04 Reintento

**Dado** un fallo recuperable
**Cuando** el usuario reintenta
**Entonces** se vuelve a ejecutar.

---

# 10. Cambios respecto a versión 1.0

* Versión inicial del caso de uso.

---

# 11. Notas

Este caso de uso es crítico en entornos reales, donde la impresión depende de factores físicos y de conectividad.

Debe considerarse la implementación de estrategias como:

* reintentos automáticos
* colas de impresión
* almacenamiento temporal

para mejorar la resiliencia del sistema.

---

**Fin del documento**
