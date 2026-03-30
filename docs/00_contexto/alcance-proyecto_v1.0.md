# Alcance del Proyecto

**Proyecto:** Motor de Documentos e Impresión basado en DSL
**Documento:** alcance-proyecto_motor-dsl_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-03-28
**Autor:** Equipo de Arquitectura

---

# 1. Propósito

Este documento define el alcance del proyecto para el desarrollo e implementación de una librería de motor de documentos basada en DSL (Domain Specific Language) orientada a la generación e impresión de documentos estructurados.

El objetivo es establecer con claridad qué incluye el proyecto, qué queda explícitamente fuera y cuáles son los entregables comprometidos, permitiendo alinear expectativas y servir como base para la planificación, estimación y control de cambios.

---

# 2. Descripción General

El proyecto tiene como finalidad construir una librería reutilizable en .NET MAUI que permita generar y renderizar documentos a partir de un modelo desacoplado compuesto por:

* Datos del documento
* Plantilla o diseño del documento (DSL)
* Perfil del dispositivo

El sistema permitirá procesar documentos dinámicos y generar salidas en distintos formatos, principalmente para impresoras térmicas Bluetooth compatibles con ESC/POS, pero con capacidad de extenderse a otros medios como PDF o visualización en pantalla.

La solución estará orientada a ser integrada en múltiples aplicaciones, facilitando la reutilización y estandarización de la generación de documentos.

---

# 3. Objetivos del Proyecto

* Diseñar un motor de documentos basado en DSL declarativo.
* Separar completamente datos, diseño y dispositivo.
* Permitir renderizado en múltiples formatos de salida.
* Soportar impresoras térmicas Bluetooth ESC/POS.
* Facilitar la reutilización mediante una librería .NET.

**Ejemplo de objetivo medible:**
Capacidad de renderizar un documento completo en menos de 500 ms en dispositivos móviles estándar.

---

# 4. Alcance Incluido

## 4.1 Funcionalidades

El proyecto incluye el desarrollo de:

* Interpretación de plantillas DSL en formato JSON.
* Resolución de datos dinámicos contra la plantilla.
* Motor de layout adaptable según perfil de dispositivo.
* Renderización a comandos ESC/POS.
* Soporte de elementos como texto, listas, tablas, imágenes y QR.
* Soporte de estructuras como secciones y bloques.
* Manejo de iteraciones sobre colecciones de datos.
* Evaluación de condiciones simples en plantillas.
* Integración con conexión Bluetooth para envío a impresoras.
* Extensibilidad para nuevos renderizadores (ej. PDF, preview UI).

---

## 4.2 Entregables

* Documentación técnica del motor DSL.
* Definición conceptual del modelo de datos.
* Definición del DSL de plantillas.
* Librería .NET MAUI funcional.
* Casos de uso implementados.
* Ejemplos de integración.
* Guía de uso para desarrolladores.
* Plan de pruebas funcionales.

---

## 4.3 Ambientes

El proyecto contempla:

* Ambiente de desarrollo.
* Ambiente de pruebas.
* Integración en aplicaciones móviles .NET 10 MAUI.

---

# 5. Alcance Excluido

Quedan explícitamente fuera de esta versión:

* Diseño visual avanzado tipo WYSIWYG.
* Editor gráfico de plantillas.
* Integraciones con sistemas de facturación fiscal.
* Soporte de impresión en red avanzada.
* Motor de reglas complejas o scripting avanzado.
* Analítica o monitoreo de impresión.
* **Soporte iOS para apps de ejemplo con impresión Bluetooth.** iOS no permite acceso a Bluetooth clásico (perfil SPP) desde aplicaciones de terceros, que es el protocolo requerido por la mayoría de impresoras térmicas ESC/POS. La librería core (`MotorDsl.Core`, `MotorDsl.Rendering`) es compatible con iOS, pero las apps de ejemplo que usan `ThermalPrinterService` vía Bluetooth son Android-only. Ver [compatibilidad-plataformas_v1.0.md](compatibilidad-plataformas_v1.0.md) para alternativas.

Estos elementos podrán evaluarse mediante solicitudes de cambio futuras.

---

# 6. Supuestos del Proyecto

* Los datos del documento serán provistos por un sistema externo o backend.
* Las plantillas DSL serán definidas previamente y versionadas.
* Las impresoras serán compatibles con el estándar ESC/POS.
* La aplicación tendrá permisos de conexión Bluetooth.
* Los perfiles de impresora estarán disponibles o serán configurables.

Estos supuestos son necesarios para el cumplimiento del alcance.

---

# 7. Restricciones

* La librería deberá ser compatible con .NET MAUI.
* El sistema deberá funcionar en entornos móviles (principalmente Android).
* **El target principal para impresión Bluetooth es Android.** iOS queda fuera del alcance de las apps de ejemplo debido a que Apple no permite acceso a Bluetooth clásico (perfil SPP) desde aplicaciones de terceros. Las impresoras térmicas ESC/POS estándar utilizan SPP para la comunicación, lo que hace inviable la impresión directa por Bluetooth en iOS sin hardware certificado MFi. La librería core es multiplataforma y puede usarse en iOS para generación de documentos (PDF, texto), pero la conexión BT a térmicas requiere Android.
* Se utilizará JSON como formato de intercambio para datos y plantillas.
* El diseño deberá ser desacoplado del hardware.
* El rendimiento deberá ser adecuado para dispositivos móviles.

Las restricciones condicionan las decisiones de diseño y arquitectura.

---

# 8. Criterios de Aceptación del Proyecto

El proyecto será aceptado cuando:

* El motor procese correctamente datos + plantilla + perfil.
* Se generen comandos ESC/POS válidos.
* Se logre impresión en impresoras térmicas Bluetooth.
* Las plantillas permitan representar documentos reales.
* El sistema sea reutilizable como librería independiente.
* La documentación técnica esté completa.

---

# 9. Gestión de Cambios de Alcance

Cualquier funcionalidad no contemplada en este documento deberá canalizarse mediante una solicitud formal de cambio, la cual será evaluada en términos de impacto en tiempo, costo y complejidad técnica.

Las modificaciones aprobadas deberán versionarse y actualizar este documento.

---

# 10. Trazabilidad

| Elemento              | Relación                      |
| --------------------- | ----------------------------- |
| Modelo de datos       | Representación de información |
| DSL de plantillas     | Definición de documentos      |
| Perfil de dispositivo | Adaptación a hardware         |
| Motor de renderizado  | Generación de salida          |

---

# 11. Control de Cambios

| Versión | Fecha      | Descripción                   |
| ------- | ---------- | ----------------------------- |
| 1.0     | 2026-03-28 | Versión inicial del motor DSL |

---

**Fin del documento**
