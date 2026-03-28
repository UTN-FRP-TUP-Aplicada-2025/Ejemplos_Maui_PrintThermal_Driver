# Backlog Técnico — Sistema de Gestión de Reclamos  
**Archivo:** backlog-tecnico_v1.0.md  
**Versión:** 1.0  
**Fecha:** 2026-03-02  
**Autor:** Equipo Técnico  
**Estado:** Activo  

---

## 1. Propósito

Este documento descompone la especificación funcional y la arquitectura en tareas técnicas implementables. Constituye la fuente de trabajo para el equipo de desarrollo y la base para la utilización de Copilot por unidad de trabajo.

El backlog se organiza por épicas técnicas y mantiene trazabilidad con los casos de uso, reglas de negocio y contratos API.

---

## 2. Convenciones

- **ID**: identificador único de la tarea  
- **Tipo**: Infraestructura / Backend / Frontend / AI / Testing  
- **Prioridad**: Alta / Media / Baja  
- **Fuente**: documento origen (CU, RN, etc.)  
- **Estado inicial**: Pendiente  

---

# ÉPICA 1 — Fundaciones del proyecto

## OBJETIVO

Preparar la solución base y estructura técnica.

---

### BT-001 — Crear solución base

- **Tipo:** Infraestructura  
- **Prioridad:** Alta  
- **Fuente:** arquitectura-solucion_v1.0.md  

**Descripción**

Crear la solución .NET con los proyectos base:

- API  
- Dominio  
- Infraestructura  
- MAUI App  

**Criterios de aceptación**

- La solución compila  
- Referencias entre capas correctas  
- Estructura Clean Architecture respetada  

---

### BT-002 — Configurar proyecto API

- **Tipo:** Backend  
- **Prioridad:** Alta  
- **Fuente:** arquitectura-solucion_v1.0.md  

**Descripción**

Crear proyecto ASP.NET Web API con:

- Controllers habilitados  
- Swagger  
- Manejo básico de errores  

**Criterios de aceptación**

- Swagger accesible  
- Endpoint health funcionando  

---

### BT-003 — Configurar Entity Framework Core

- **Tipo:** Backend  
- **Prioridad:** Alta  
- **Fuente:** modelo-datos-logico_v1.0.md  

**Descripción**

Agregar EF Core y configurar DbContext base.

**Criterios**

- Conexión a SQL Server  
- Migración inicial ejecutable  

---

# ÉPICA 2 — Modelo de dominio

## OBJETIVO

Implementar las entidades y repositorios.

---

### BT-010 — Crear entidad Area

- **Tipo:** Backend  
- **Prioridad:** Alta  
- **Fuente:** modelo-datos-logico  

**Criterios**

- Propiedades correctas  
- Configuración EF válida  

---

### BT-011 — Crear entidad EstadoReclamo

- **Tipo:** Backend  
- **Prioridad:** Alta  
- **Fuente:** modelo-datos-logico  

---

### BT-012 — Crear entidad Reclamo

- **Tipo:** Backend  
- **Prioridad:** Alta  
- **Fuente:** CU-01, modelo lógico  

**Criterios**

- Validaciones básicas  
- Navegaciones EF  

---

### BT-013 — Crear entidad HistorialEstado

- **Tipo:** Backend  
- **Prioridad:** Alta  
- **Fuente:** modelo lógico  

---

### BT-014 — Crear migración inicial

- **Tipo:** Backend  
- **Prioridad:** Alta  
- **Fuente:** modelo lógico  

**Criterios**

- Tablas creadas  
- FKs correctas  
- Índices básicos  

---

# ÉPICA 3 — Caso de uso: Registrar Reclamo

## OBJETIVO

Implementar CU-01 end-to-end.

---

### BT-020 — Crear DTO RegistrarReclamoRequest

- **Tipo:** Backend  
- **Prioridad:** Alta  
- **Fuente:** contratos-api  

---

### BT-021 — Crear DTO RegistrarReclamoResponse

- **Tipo:** Backend  
- **Prioridad:** Alta  
- **Fuente:** contratos-api  

---

### BT-022 — Implementar servicio de aplicación RegistrarReclamo

- **Tipo:** Backend  
- **Prioridad:** Alta  
- **Fuente:** CU-01  

**Debe**

- Validar datos  
- Invocar clasificación  
- Aplicar RN-01  
- Persistir reclamo  
- Crear historial  

---

### BT-023 — Implementar endpoint POST /api/reclamos

- **Tipo:** Backend  
- **Prioridad:** Alta  
- **Fuente:** contratos-api  

**Criterios**

- Respuesta 201  
- Validaciones activas  
- Manejo de errores estándar  

---

# ÉPICA 4 — Clasificación automática (AI)

## OBJETIVO

Integrar el agente de clasificación.

---

### BT-030 — Implementar cliente de clasificación AI

- **Tipo:** AI  
- **Prioridad:** Alta  
- **Fuente:** prompt-clasificacion-reclamo  

**Debe**

- Enviar prompt  
- Parsear JSON  
- Manejar timeout  
- Manejar errores  

---

### BT-031 — Implementar servicio de dominio de clasificación

- **Tipo:** Backend  
- **Prioridad:** Alta  
- **Fuente:** RN-01  

---

### BT-032 — Mapear área por resultado AI

- **Tipo:** Backend  
- **Prioridad:** Alta  
- **Fuente:** RN-01  

---

# ÉPICA 5 — Consulta de estado

## OBJETIVO

Implementar CU-02.

---

### BT-040 — Implementar servicio ConsultarEstado

- **Tipo:** Backend  
- **Prioridad:** Alta  
- **Fuente:** CU-02  

---

### BT-041 — Implementar endpoint GET /api/reclamos/{id}

- **Tipo:** Backend  
- **Prioridad:** Alta  
- **Fuente:** contratos-api  

---

# ÉPICA 6 — Frontend MAUI

## OBJETIVO

Interfaz mínima operativa.

---

### BT-050 — Crear pantalla Registrar Reclamo

- **Tipo:** Frontend  
- **Prioridad:** Media  
- **Fuente:** wireframes  

---

### BT-051 — Implementar ViewModel RegistrarReclamo

- **Tipo:** Frontend  
- **Prioridad:** Media  

---

### BT-052 — Integrar llamada a API

- **Tipo:** Frontend  
- **Prioridad:** Media  

---

### BT-053 — Crear pantalla Consultar Estado

- **Tipo:** Frontend  
- **Prioridad:** Media  

---

# ÉPICA 7 — Testing

## OBJETIVO

Garantizar calidad.

---

### BT-060 — Unit tests servicio RegistrarReclamo

- **Tipo:** Testing  
- **Prioridad:** Alta  

---

### BT-061 — Unit tests clasificación AI (mock)

- **Tipo:** Testing  
- **Prioridad:** Alta  

---

### BT-062 — Integration test POST /api/reclamos

- **Tipo:** Testing  
- **Prioridad:** Alta  

---

### BT-063 — Integration test GET /api/reclamos

- **Tipo:** Testing  
- **Prioridad:** Alta  

---

# ÉPICA 8 — Observabilidad (inicial)

---

### BT-070 — Logging estructurado

- **Tipo:** Infraestructura  
- **Prioridad:** Media  

---

### BT-071 — Middleware de manejo de errores

- **Tipo:** Backend  
- **Prioridad:** Alta  

---

# 🧭 Orden recomendado de ejecución

```text
1️⃣ Épica 1 — Fundaciones  
2️⃣ Épica 2 — Dominio  
3️⃣ Épica 3 — Registrar reclamo  
4️⃣ Épica 4 — AI  
5️⃣ Épica 5 — Consulta  
6️⃣ Épica 6 — MAUI  
7️⃣ Épica 7 — Testing  
8️⃣ Épica 8 — Observabilidad  
````

---

# 10. Definición de Done (DoD)

Una tarea se considera finalizada cuando:

* Compila
* Tiene tests (si aplica)
* Cumple contrato
* Pasa validación funcional
* Está revisada por pares

---

# 11. Historial de versiones

| Versión | Fecha      | Autor          | Cambios         |
| ------- | ---------- | -------------- | --------------- |
| 1.0     | 2026-03-02 | Equipo Técnico | Backlog inicial |

---

```
