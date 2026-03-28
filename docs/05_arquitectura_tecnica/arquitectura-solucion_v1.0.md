# Arquitectura de la Solución — Sistema de Gestión de Reclamos  
**Archivo:** arquitectura-solucion_v1.0.md  
**Versión:** 1.0  
**Fecha:** 2026-03-02  
**Autor:** Equipo Técnico  
**Estado:** Borrador  

---

## 1. Propósito

Este documento describe la arquitectura técnica propuesta para el Sistema de Gestión de Reclamos. Define la estructura de alto nivel, los componentes principales, las responsabilidades y las tecnologías seleccionadas para implementar la solución.

El objetivo es establecer una base técnica coherente que permita construir el sistema de forma mantenible, escalable y alineada con la especificación funcional vigente.

---

## 2. Alcance

La arquitectura cubre:

- Aplicación cliente
- API backend
- Servicio de clasificación AI
- Persistencia de datos
- Integraciones externas

No incluye detalles de infraestructura física ni configuración de despliegue fino, que serán tratados en documentos posteriores.

---

## 3. Estilo arquitectónico

Se adopta una **arquitectura en capas con principios de Clean Architecture**, separando claramente:

- Presentación  
- Aplicación  
- Dominio  
- Infraestructura  

Esta decisión permite aislar reglas de negocio, facilitar testing y reducir acoplamiento tecnológico.

---

## 4. Visión de alto nivel

```text
[App MAUI]
     │
     ▼
[API REST ASP.NET]
     │
     ├── [Servicio de Dominio]
     ├── [Servicio AI Clasificación]
     └── [Repositorio / EF]
             │
             ▼
        [Base de Datos]
````

---

## 5. Componentes principales

### 5.1 Aplicación Cliente (Frontend)

**Responsabilidad**

La aplicación cliente permite al ciudadano:

* Registrar reclamos
* Consultar estado
* Visualizar historial

**Tecnología seleccionada**

* .NET MAUI
* Patrón MVVM
* Consumo de API REST

**Justificación**

Permite aplicación multiplataforma móvil con reutilización de código y buena integración con el ecosistema .NET.

---

### 5.2 API Backend

**Responsabilidad**

Exponer servicios HTTP para:

* Gestión de reclamos
* Consultas
* Integración con clasificación automática

**Tecnología**

* ASP.NET Web API (.NET 10)
* JSON sobre HTTP
* Scalar/OpenAPI

**Principios**

* Stateless
* Validación en capa de aplicación
* Manejo centralizado de errores

---

### 5.3 Capa de Dominio

**Responsabilidad**

Contener:

* Entidades de negocio
* Reglas de negocio (RN-01, etc.)
* Servicios de dominio

Esta capa no debe depender de frameworks externos.

**Elementos clave**

* Entidad Reclamo
* Servicio de Clasificación
* Value Objects (si aplica)

---

### 5.4 Servicio de Clasificación AI

**Responsabilidad**

Clasificar automáticamente el reclamo según su descripción utilizando el prompt definido en:

* `prompt-clasificacion-reclamo_v1.1.md`

**Modo de operación**

* Invocado desde la capa de aplicación
* Respuesta estructurada
* Timeout controlado
* Logging de decisiones

**Estrategia inicial**

* Integración vía API de modelo LLM
* Prompt versionado
* Posibilidad futura de modelo propio

---

### 5.5 Persistencia de Datos

**Responsabilidad**

Almacenar:

* Reclamos
* Estados
* Historial
* Tablas de clasificación

**Tecnología**

* SQL Server (inicial)
* Entity Framework Core
* Migraciones versionadas

**Principios**

* Modelo relacional normalizado
* Soft delete (si aplica)
* Auditoría básica

---

## 6. Flujo principal — Registrar reclamo

```text
Usuario (MAUI)
   ↓
POST /api/reclamos
   ↓
API valida datos
   ↓
Servicio AI clasifica
   ↓
Se aplica RN-01
   ↓
Se persiste Reclamo
   ↓
Se devuelve número de reclamo
```

---

## 7. Decisiones técnicas clave

| Decisión      | Elección           | Motivo            |
| ------------- | ------------------ | ----------------- |
| Frontend      | .NET MAUI          | Multiplataforma   |
| Backend       | ASP.NET Web API    | Ecosistema .NET   |
| ORM           | EF Core            | Productividad     |
| Base de datos | SQL Server         | Compatibilidad    |
| Arquitectura  | Clean Architecture | Bajo acoplamiento |
| AI            | LLM externo        | Rapidez inicial   |

---

## 8. Consideraciones de calidad

### Escalabilidad

* API stateless
* Posible horizontal scaling
* AI desacoplada

### Mantenibilidad

* Separación por capas
* Prompt versionado
* Reglas centralizadas

### Observabilidad

* Logging estructurado
* CorrelationId por request
* Métricas futuras

---

## 9. Riesgos conocidos

* Dependencia de servicio AI externo
* Clasificación ambigua en textos cortos
* Posible crecimiento de reglas de negocio

---

## 10. Evolución prevista

Futuras versiones podrán incluir:

* Cache de clasificaciones
* Cola de procesamiento
* Multi-tenant
* Motor de reglas configurable
* Modelo AI entrenado propio

---

## 11. Relación con otros documentos

Esta arquitectura se basa en:

* especificacion-funcional_v1.1.md
* CU-01 y CU-02
* RN-01
* modelo-conceptual_v1.0.md
* prompts AI v1.1

---

## 12. Historial de versiones

| Versión | Fecha      | Autor          | Cambios                         |
| ------- | ---------- | -------------- | ------------------------------- |
| 1.0     | 2026-03-02 | Equipo Técnico | Versión inicial de arquitectura |

---

```
