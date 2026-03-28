```markdown id="dec-arq-001"
# Decisiones de Arquitectura (ADR) — Sistema de Gestión de Reclamos  
**Archivo:** decisiones-arquitectura_v1.0.md  
**Versión:** 1.0  
**Fecha:** 2026-03-02  
**Autor:** Equipo Técnico  
**Estado:** Activo  

---

## 1. Propósito

Este documento registra las decisiones arquitectónicas relevantes (Architecture Decision Records — ADR) tomadas para el Sistema de Gestión de Reclamos. Cada decisión incluye contexto, alternativas consideradas, decisión final y consecuencias.

El objetivo es preservar la trazabilidad técnica y facilitar futuras evoluciones del sistema.

---

# ADR-001 — Estilo arquitectónico

## Estado  
Aceptado

## Contexto

El sistema debe permitir evolución gradual, testing aislado de reglas de negocio y bajo acoplamiento entre componentes. Se espera crecimiento funcional en el tiempo.

## Alternativas consideradas

- Arquitectura en capas tradicional  
- Clean Architecture  
- Microservicios  

## Decisión

Se adopta **Clean Architecture con estructura en capas**.

## Consecuencias

**Positivas**

- Separación clara de responsabilidades  
- Reglas de negocio aisladas  
- Mejor testabilidad  

**Negativas**

- Mayor complejidad inicial  
- Más proyectos en la solución  

---

# ADR-002 — Tipo de frontend

## Estado  
Aceptado

## Contexto

El sistema requiere aplicación móvil multiplataforma para ciudadanos con capacidad de crecimiento futuro.

## Alternativas consideradas

- Aplicación web responsive  
- .NET MAUI  
- Flutter  

## Decisión

Se selecciona **.NET MAUI con patrón MVVM**.

## Consecuencias

**Positivas**

- Reutilización de código C#  
- Integración con ecosistema .NET  
- Soporte multiplataforma  

**Negativas**

- Curva de aprendizaje MVVM  
- Dependencia del stack .NET  

---

# ADR-003 — Backend API

## Estado  
Aceptado

## Contexto

Se requiere exponer servicios REST para operaciones de reclamos con buena integración al frontend MAUI.

## Alternativas consideradas

- Node.js  
- ASP.NET Web API  
- Minimal APIs  

## Decisión

Se adopta **ASP.NET Web API (.NET 8)**.

## Consecuencias

**Positivas**

- Ecosistema unificado  
- Buen soporte de tooling  
- Integración con EF Core  

**Negativas**

- Requiere hosting .NET  
- Overhead frente a minimal APIs  

---

# ADR-004 — Persistencia de datos

## Estado  
Aceptado

## Contexto

El sistema necesita persistencia relacional confiable con soporte transaccional y facilidad de modelado.

## Alternativas consideradas

- SQL Server  
- PostgreSQL  
- MongoDB  

## Decisión

Se selecciona **SQL Server + Entity Framework Core**.

## Consecuencias

**Positivas**

- Madurez tecnológica  
- Buen soporte en .NET  
- Consultas relacionales claras  

**Negativas**

- Menor flexibilidad que NoSQL  
- Dependencia de motor relacional  

---

# ADR-005 — Estrategia de clasificación AI

## Estado  
Aceptado

## Contexto

Se requiere clasificación semántica rápida sin construir inicialmente un modelo propio.

## Alternativas consideradas

- Reglas hardcodeadas  
- Modelo ML propio  
- LLM externo mediante prompt  

## Decisión

Se adopta **LLM externo con prompt versionado**.

## Consecuencias

**Positivas**

- Implementación rápida  
- Buena capacidad semántica  
- Fácil evolución del prompt  

**Negativas**

- Dependencia externa  
- Variabilidad del modelo  
- Costo por uso  

---

# ADR-006 — Comunicación entre capas

## Estado  
Aceptado

## Contexto

Se busca bajo acoplamiento entre aplicación, dominio e infraestructura.

## Alternativas consideradas

- Acceso directo a EF desde controllers  
- Servicios de aplicación  
- Mediator pattern  

## Decisión

Se utilizarán **servicios de aplicación con interfaces de repositorio**.

## Consecuencias

**Positivas**

- Dominio desacoplado  
- Mejor testabilidad  
- Flexibilidad futura  

**Negativas**

- Más código boilerplate  
- Curva de aprendizaje para el equipo  

---

# ADR-007 — Manejo de errores

## Estado  
Aceptado

## Contexto

La API debe ser consistente y no exponer detalles internos.

## Alternativas consideradas

- Manejo por controller  
- Middleware global  
- Filtros de excepción  

## Decisión

Se implementará **middleware global de manejo de errores**.

## Consecuencias

**Positivas**

- Respuestas uniformes  
- Menos duplicación  
- Mejor observabilidad  

**Negativas**

- Requiere disciplina en excepciones  
- Debug inicial más complejo  

---

# ADR-008 — Versionado de prompts

## Estado  
Aceptado

## Contexto

La clasificación AI depende de prompts que evolucionarán.

## Alternativas consideradas

- Prompt hardcodeado  
- Prompt en base de datos  
- Prompt versionado en repositorio  

## Decisión

Se adopta **prompt versionado en repositorio**.

## Consecuencias

**Positivas**

- Trazabilidad  
- Reproducibilidad  
- Control por Git  

**Negativas**

- Requiere disciplina de versionado  
- Cambios implican deploy  

---

## Historial de versiones

| Versión | Fecha | Autor | Cambios |
|--------|------|------|---------|
| 1.0 | 2026-03-02 | Equipo Técnico | ADR iniciales |

---
```
