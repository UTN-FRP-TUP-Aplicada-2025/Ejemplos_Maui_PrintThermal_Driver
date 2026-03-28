
# Necesidades de Negocio

**Proyecto:** Motor de Documentos e Impresión basado en DSL
**Documento:** NB-03-Cambios-sin-codigo.v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-03-28
**Autor:** Área de Arquitectura / Análisis Técnico

## NB-03 Permitir cambios de diseño sin modificar código

La organización necesita poder modificar la estructura y el diseño de los documentos sin requerir cambios en el código de las aplicaciones ni nuevos despliegues.

Para ello, el sistema debe basarse en plantillas DSL externas y versionables que puedan ser actualizadas de forma independiente, permitiendo adaptar rápidamente los documentos a nuevos requerimientos de negocio.

Este enfoque reduce el impacto operativo de los cambios y mejora la capacidad de respuesta ante necesidades de evolución.

**Ejemplo:**
Agregar un nuevo campo o modificar el formato de un ticket actualizando únicamente la plantilla DSL, sin recompilar la aplicación.

**Impacto:**
Plantillas dinámicas