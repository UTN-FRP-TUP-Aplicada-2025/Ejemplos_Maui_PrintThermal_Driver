
# Necesidades de Negocio

**Proyecto:** Motor de Documentos e Impresión basado en DSL
**Documento:** NB-06-Reutilizacion.v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-03-28
**Autor:** Área de Arquitectura / Análisis Técnico

## NB-06 Habilitar reutilización entre proyectos

La organización necesita contar con una solución centralizada que permita reutilizar la lógica de generación y renderizado de documentos en múltiples aplicaciones y contextos, evitando desarrollos aislados.

El motor debe implementarse como una librería desacoplada, con interfaces claras y extensibles, que pueda integrarse fácilmente en distintos proyectos sin depender de una aplicación específica.

Este enfoque favorece la consistencia técnica, reduce la duplicación de esfuerzos y facilita la evolución del sistema a lo largo del tiempo.

**Ejemplo:**
Una misma librería de generación de documentos utilizada tanto en una aplicación móvil .NET MAUI como en un servicio backend.

**Impacto:**
Integración en múltiples aplicaciones