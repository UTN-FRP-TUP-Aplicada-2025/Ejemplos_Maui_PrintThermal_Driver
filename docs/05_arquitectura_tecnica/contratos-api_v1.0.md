````markdown id="contratos-api-001"
# Contratos API — Sistema de Gestión de Reclamos  
**Archivo:** contratos-api_v1.0.md  
**Versión:** 1.0  
**Fecha:** 2026-03-02  
**Autor:** Equipo Técnico  
**Estado:** Borrador  

---

## 1. Propósito

Este documento define los contratos HTTP del backend del Sistema de Gestión de Reclamos. Establece endpoints, estructuras de request/response, códigos de estado y reglas de validación para asegurar integración consistente entre cliente y servidor.

Los contratos aquí definidos deben considerarse la fuente de verdad para el desarrollo del frontend, pruebas automatizadas e integraciones externas.

---

## 2. Convenciones generales

**Base URL**

```text
/api
````

**Formato de datos**

* Content-Type: application/json
* Encoding: UTF-8
* Fechas: ISO 8601
* Identificadores: string

**Reglas**

* La API es stateless
* Todas las respuestas de error usan estructura estándar
* Se utiliza HTTP status code semántico

---

## 3. Modelo de error estándar

### Response de error

```json id="err-model-01"
{
  "error": {
    "codigo": "VALIDATION_ERROR",
    "mensaje": "Descripción del error",
    "detalle": []
  }
}
```

---

## 4. Endpoint — Registrar Reclamo

### POST /api/reclamos

Crea un nuevo reclamo ciudadano.

---

### Request

```json id="req-post-reclamo"
{
  "tipoReclamo": "Alumbrado",
  "descripcion": "La luz de la calle no funciona",
  "ubicacion": "Calle 123",
  "medioContacto": "correo@ejemplo.com"
}
```

---

### Validaciones

* tipoReclamo: requerido
* descripcion: requerido, mínimo 10 caracteres
* medioContacto: requerido
* ubicacion: opcional

---

### Response — 201 Created

```json id="res-post-reclamo"
{
  "numeroReclamo": "RC-00012345",
  "estado": "Registrado",
  "areaAsignada": "Servicios Públicos",
  "fechaRegistro": "2026-03-02T21:00:00Z"
}
```

---

### Posibles errores

| HTTP | Código           | Motivo           |
| ---- | ---------------- | ---------------- |
| 400  | VALIDATION_ERROR | Datos inválidos  |
| 500  | INTERNAL_ERROR   | Error inesperado |

---

## 5. Endpoint — Consultar Estado

### GET /api/reclamos/{numeroReclamo}

Obtiene el estado actual de un reclamo.

---

### Parámetros

| Nombre        | Tipo   | Requerido |
| ------------- | ------ | --------- |
| numeroReclamo | string | sí        |

---

### Response — 200 OK

```json id="res-get-reclamo"
{
  "numeroReclamo": "RC-00012345",
  "tipo": "Alumbrado",
  "estado": "En proceso",
  "area": "Servicios Públicos",
  "fechaRegistro": "2026-03-02T21:00:00Z",
  "historial": [
    {
      "fecha": "2026-03-02T21:00:00Z",
      "estado": "Registrado"
    },
    {
      "fecha": "2026-03-03T10:15:00Z",
      "estado": "Asignado"
    }
  ]
}
```

---

### Posibles errores

| HTTP | Código           | Motivo              |
| ---- | ---------------- | ------------------- |
| 404  | NOT_FOUND        | Reclamo inexistente |
| 400  | VALIDATION_ERROR | Formato inválido    |

---

## 6. Endpoint — Listar Reclamos del Usuario (futuro)

### GET /api/reclamos

Lista reclamos asociados al medio de contacto.

**Estado:** previsto para versión futura.

---

### Parámetros (query)

| Nombre        | Tipo   | Requerido |
| ------------- | ------ | --------- |
| medioContacto | string | sí        |
| page          | int    | no        |
| pageSize      | int    | no        |

---

### Response — 200 OK

```json id="res-list-reclamos"
{
  "items": [
    {
      "numeroReclamo": "RC-00012345",
      "tipo": "Alumbrado",
      "estado": "En proceso",
      "fecha": "2026-03-02"
    }
  ],
  "page": 1,
  "pageSize": 10,
  "total": 1
}
```

---

## 7. Reglas de versionado de API

* Cambios compatibles → misma versión
* Cambios breaking → versionar endpoint
* Versionado vía URL si es necesario:

```text
/api/v1/reclamos
```

---

## 8. Criterios de aceptación

La API se considera correcta si:

* Cumple contratos JSON definidos
* Usa códigos HTTP correctos
* No expone excepciones internas
* Es consumible por MAUI sin adaptaciones
* Pasa pruebas de integración

---

## 9. Relación con otros documentos

Este documento se alinea con:

* especificacion-funcional_v1.1.md
* CU-01-registrar-reclamo
* CU-02-consultar-estado
* arquitectura-solucion_v1.0.md
* modelo-conceptual_v1.0.md

---

## 10. Historial de versiones

| Versión | Fecha      | Autor          | Cambios                         |
| ------- | ---------- | -------------- | ------------------------------- |
| 1.0     | 2026-03-02 | Equipo Técnico | Definición inicial de contratos |

---

```
```
