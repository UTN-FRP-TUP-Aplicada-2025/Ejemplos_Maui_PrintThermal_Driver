
# Necesidades de Negocio

**Proyecto:** Motor de Documentos e Impresión basado en DSL
**Documento:** NB-04-Multi-dispositivo.v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-03-28
**Autor:** Área de Arquitectura / Análisis Técnico

## NB-04 Adaptarse a múltiples dispositivos de salida

La organización necesita que los documentos puedan generarse y renderizarse correctamente en distintos dispositivos y formatos de salida, sin requerir cambios en los datos ni en las plantillas.

El sistema debe abstraer las particularidades del hardware mediante perfiles de dispositivo, permitiendo que un mismo documento pueda adaptarse automáticamente a diferentes capacidades, resoluciones y protocolos de impresión.

Este enfoque garantiza portabilidad y extensibilidad del sistema hacia nuevos dispositivos.

**Ejemplo:**
Un mismo documento debe poder imprimirse en una impresora térmica ESC/POS, visualizarse en pantalla o exportarse a PDF sin redefinir su estructura.

**Impacto:**
Renderizadores