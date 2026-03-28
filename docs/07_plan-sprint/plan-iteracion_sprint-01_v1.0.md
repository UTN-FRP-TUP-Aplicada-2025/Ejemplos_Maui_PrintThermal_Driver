# Plan de Iteración — Sprint 01

## 📋 Información general

- Proyecto: Sistema de Gestión de Reclamos Municipales
- Sprint: 01
- Duración: 2 semanas
- Fecha inicio: 01/04/2026
- Fecha fin: 14/04/2026
- Objetivo del sprint: Permitir registrar reclamos desde la app móvil y almacenarlos en el sistema.

---

## 🎯 Objetivo del Sprint

Implementar el flujo mínimo funcional (MVP técnico):

- Registrar reclamo
- Guardar en base de datos
- Asignación automática básica
- Endpoint de consulta

---

## 🧩 Historias / tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación |
|----|------|------------|----------|-----------|
| US-01 | Historia | Registrar reclamo | Alta | 8 pts |
| TK-01 | Técnica | Crear API POST /reclamos | Alta | 5 pts |
| TK-02 | Técnica | Modelo de datos Reclamo | Alta | 3 pts |
| TK-03 | Técnica | Servicio de asignación automática | Media | 5 pts |
| TK-04 | Técnica | Endpoint GET /reclamos/{id} | Media | 3 pts |

---

## 🏗️ Alcance técnico del sprint

### Backend

- API REST básica
- Persistencia en base de datos
- Validaciones mínimas

### Frontend (si aplica)

- Pantalla de registro simple
- Envío de formulario a API

---

## ✅ Criterios de terminado (Definition of Done)

Una tarea se considera terminada cuando:

- El código compila sin errores
- Hay pruebas unitarias básicas
- El endpoint responde correctamente
- Se actualiza la documentación si aplica
- Se despliega en entorno de testing

---

## 🧪 Plan de pruebas del sprint

### Pruebas unitarias

- Validación de datos de reclamo
- Asignación automática

### Pruebas de integración

- POST /reclamos crea correctamente
- GET /reclamos/{id} devuelve datos

---

## 🚀 Entregables del sprint

- API funcional en entorno dev
- Base de datos creada
- Documentación actualizada
- Endpoints probados

---

## ⚠️ Riesgos identificados

- Definición incompleta de reglas de asignación
- Integración futura con notificaciones
- Posibles cambios en modelo de datos

---

## 🔄 Métricas de seguimiento

- Velocidad del equipo
- Tareas completadas vs comprometidas
- Defectos encontrados en testing