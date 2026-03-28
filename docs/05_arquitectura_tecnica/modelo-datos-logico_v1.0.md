````markdown id="mdl-log-001"
# Modelo de Datos Lógico — Sistema de Gestión de Reclamos  
**Archivo:** modelo-datos-logico_v1.0.md  
**Versión:** 1.0  
**Fecha:** 2026-03-02  
**Autor:** Equipo Técnico  
**Estado:** Borrador  

---

## 1. Propósito

Este documento define el modelo de datos lógico derivado del modelo conceptual. Describe las tablas, campos, tipos de datos, claves primarias y relaciones necesarias para soportar los casos de uso del sistema.

El modelo lógico está orientado a implementación en base de datos relacional, manteniendo trazabilidad con la especificación funcional y las reglas de negocio.

---

## 2. Alcance

El modelo cubre:

- Registro de reclamos  
- Estados del reclamo  
- Historial de cambios  
- Áreas responsables  

No incluye optimizaciones físicas ni índices avanzados, que se tratarán en el modelo físico.

---

## 3. Entidades principales

El sistema se compone de las siguientes tablas:

- Reclamo  
- EstadoReclamo  
- HistorialEstado  
- Area  

---

## 4. Tabla: Area

Representa las áreas municipales responsables.

### Estructura

| Campo | Tipo | Nulo | Descripción |
|------|------|------|------------|
| AreaId | int (PK) | No | Identificador |
| Nombre | varchar(100) | No | Nombre del área |
| Activa | bit | No | Indicador de vigencia |

### Reglas

- Nombre debe ser único.  
- Deben existir áreas base del sistema.  

---

## 5. Tabla: EstadoReclamo

Catálogo de estados posibles.

### Estructura

| Campo | Tipo | Nulo | Descripción |
|------|------|------|------------|
| EstadoId | int (PK) | No | Identificador |
| Nombre | varchar(50) | No | Nombre del estado |
| Orden | int | No | Orden lógico |

### Estados iniciales

- Registrado  
- Asignado  
- En proceso  
- Cerrado  

---

## 6. Tabla: Reclamo

Entidad principal del sistema.

### Estructura

| Campo | Tipo | Nulo | Descripción |
|------|------|------|------------|
| ReclamoId | bigint (PK) | No | Identificador interno |
| NumeroReclamo | varchar(20) | No | Código público |
| TipoReclamo | varchar(100) | No | Tipo declarado |
| Descripcion | varchar(1000) | No | Texto del ciudadano |
| Ubicacion | varchar(200) | Sí | Dirección |
| MedioContacto | varchar(150) | No | Email o teléfono |
| AreaId | int (FK) | No | Área asignada |
| EstadoActualId | int (FK) | No | Estado actual |
| FechaRegistro | datetime2 | No | Alta |
| FechaUltimaActualizacion | datetime2 | No | Último cambio |
| Activo | bit | No | Soft delete |

---

### Claves

- PK: ReclamoId  
- UK: NumeroReclamo  
- FK: AreaId → Area  
- FK: EstadoActualId → EstadoReclamo  

---

### Reglas de negocio asociadas

- NumeroReclamo debe ser único.  
- Estado inicial = Registrado.  
- AreaId se asigna por RN-01.  
- Descripcion mínimo 10 caracteres.  

---

## 7. Tabla: HistorialEstado

Registra la trazabilidad del reclamo.

### Estructura

| Campo | Tipo | Nulo | Descripción |
|------|------|------|------------|
| HistorialId | bigint (PK) | No | Identificador |
| ReclamoId | bigint (FK) | No | Reclamo |
| EstadoId | int (FK) | No | Estado |
| FechaCambio | datetime2 | No | Momento |
| Observaciones | varchar(500) | Sí | Comentario |
| UsuarioSistema | varchar(100) | Sí | Origen |

---

### Claves

- PK: HistorialId  
- FK: ReclamoId → Reclamo  
- FK: EstadoId → EstadoReclamo  

---

### Reglas

- Debe existir registro al crear el reclamo.  
- Debe mantenerse orden cronológico.  

---

## 8. Relaciones

```text
Area (1) ───── (N) Reclamo
EstadoReclamo (1) ───── (N) Reclamo
Reclamo (1) ───── (N) HistorialEstado
EstadoReclamo (1) ───── (N) HistorialEstado
````

---

## 9. Consideraciones de integridad

### Integridad referencial

* Todas las FK con restricción ON DELETE NO ACTION.
* No se permite borrar áreas en uso.

### Consistencia temporal

* FechaUltimaActualizacion debe actualizarse en cada cambio.
* HistorialEstado es la fuente de verdad del flujo.

---

## 10. Consideraciones de performance (iniciales)

* Índice único en NumeroReclamo
* Índice en Reclamo.AreaId
* Índice en Reclamo.EstadoActualId
* Índice en HistorialEstado.ReclamoId

Optimizaciones adicionales se evaluarán según métricas reales.

---

## 11. Datos semilla requeridos

### Áreas iniciales

* Rentas
* Sistemas
* Servicios Públicos
* Atención al Vecino

### Estados iniciales

* Registrado
* Asignado
* En proceso
* Cerrado

---

## 12. Trazabilidad

Este modelo deriva de:

* modelo-conceptual_v1.0.md
* CU-01-registrar-reclamo
* CU-02-consultar-estado
* RN-01-asignacion-automatica
* contratos-api_v1.0.md

---

## 13. Riesgos conocidos

* Crecimiento del historial puede requerir particionado futuro.
* TipoReclamo podría normalizarse en versiones futuras.
* Posible necesidad de multi-tenant.

---

## 14. Evolución prevista

Posibles extensiones:

* Tabla Usuario
* Tabla Adjuntos
* Geolocalización
* SLA por área
* Prioridad del reclamo

---

## 15. Historial de versiones

| Versión | Fecha      | Autor          | Cambios               |
| ------- | ---------- | -------------- | --------------------- |
| 1.0     | 2026-03-02 | Equipo Técnico | Modelo lógico inicial |

---

```
```
