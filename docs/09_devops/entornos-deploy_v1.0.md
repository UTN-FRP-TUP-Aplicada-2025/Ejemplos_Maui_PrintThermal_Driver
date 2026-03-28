# Entornos de Deploy  
**Proyecto:** Reclamos Ciudadanos  
**Versión:** v1.0  
**Estado:** Aprobado  
**Fecha:** 2026-03-02  
**Owner:** Arquitectura / DevOps  

---

## 1. Propósito

Este documento define los entornos de despliegue del sistema Reclamos Ciudadanos, sus responsabilidades, configuraciones generales y reglas de promoción entre ambientes. El objetivo es asegurar consistencia, trazabilidad y calidad en el ciclo de entrega continua.

---

## 2. Estrategia general

El proyecto adopta una estrategia de despliegue progresivo por entornos, donde cada cambio atraviesa validaciones crecientes antes de llegar a producción.

Flujo de promoción:

```text
Desarrollo → QA/Testing → Staging (opcional) → Producción
````

Cada entorno tiene datos, controles y niveles de estabilidad diferentes.

---

## 3. Entorno de Desarrollo (DEV)

### 3.1 Propósito

Permitir a los desarrolladores construir y validar funcionalidades de forma rápida e iterativa.

### 3.2 Características

* Deploy frecuente (varias veces por día)
* Datos sintéticos
* Logging verbose
* Debug habilitado
* Posible inestabilidad

### 3.3 Uso principal

* Pruebas unitarias
* Pruebas de integración temprana
* Validación técnica rápida

### 3.4 Responsables

* Equipo de desarrollo

---

## 4. Entorno de Testing / QA

### 4.1 Propósito

Validar funcionalmente el sistema contra la especificación funcional y los casos de uso.

### 4.2 Características

* Deploy por sprint o por feature
* Datos controlados
* Logging moderado
* Configuración cercana a producción
* Acceso controlado

### 4.3 Uso principal

* Pruebas funcionales
* Pruebas de regresión
* Validación de criterios de aceptación

### 4.4 Responsables

* QA
* Desarrollo (soporte)

---

## 5. Entorno de Staging (Preproducción)

> **Estado:** opcional en v1.0 — recomendado para madurez futura

### 5.1 Propósito

Simular producción lo más fielmente posible antes del release.

### 5.2 Características

* Configuración idéntica a producción
* Datos anonimizados o representativos
* Logging productivo
* Seguridad completa habilitada
* Alta estabilidad requerida

### 5.3 Uso principal

* Smoke tests finales
* Validación de despliegue
* Pruebas de humo
* Validación de performance básica

### 5.4 Responsables

* DevOps
* QA
* Arquitectura

---

## 6. Entorno de Producción (PROD)

### 6.1 Propósito

Proveer el servicio real a los usuarios finales.

### 6.2 Características

* Alta disponibilidad
* Logging controlado
* Monitoreo activo
* Seguridad completa
* Cambios estrictamente controlados

### 6.3 Reglas

* Solo se despliega código aprobado
* Requiere build CI en verde
* Requiere validación QA
* Requiere versión etiquetada

### 6.4 Responsables

* DevOps
* Operaciones
* Arquitectura (gobernanza)

---

## 7. Configuración por entorno

| Configuración     | DEV    | QA    | STAGING    | PROD       |
| ----------------- | ------ | ----- | ---------- | ---------- |
| Debug             | Sí     | No    | No         | No         |
| Logging           | Alto   | Medio | Medio      | Bajo       |
| Datos reales      | No     | No    | Parcial    | Sí         |
| Deploy automático | Sí     | Sí    | Controlado | Controlado |
| Monitoreo         | Básico | Medio | Alto       | Completo   |

---

## 8. Estrategia de versionado

Formato de versión:

```text
MAJOR.MINOR.PATCH
```

Ejemplos:

* 1.0.0 — primera release
* 1.1.0 — nueva funcionalidad
* 1.1.1 — bugfix

### Reglas

* Cada deploy a QA debe tener versión.
* Cada deploy a PROD debe estar taggeado.
* No se permite código sin versión en PROD.

---

## 9. Promoción entre entornos

### Flujo obligatorio

1. Merge a rama principal
2. Build CI exitoso
3. Deploy automático a DEV
4. Validación → promoción a QA
5. QA aprobado → promoción a PROD

### Criterios de promoción

* Definition of Done cumplida
* Tests en verde
* Sin defectos críticos abiertos

---

## 10. Rollback

### Estrategia

* Mantener versión previa deployable
* Deploy inmutable por versión
* Scripts reversibles de base de datos

### Cuándo aplicar

* Error crítico en producción
* Degradación severa
* Fallo de migración

---

## 11. Observabilidad mínima (recomendado)

Aunque básico en v1.0, se recomienda:

* Logs estructurados
* Métricas de API
* Health checks
* Alertas básicas

---

## 12. Riesgos

| Riesgo                     | Impacto | Mitigación         |
| -------------------------- | ------- | ------------------ |
| Diferencias entre entornos | Alta    | config por entorno |
| Datos inconsistentes       | Media   | datos sintéticos   |
| Deploy manual              | Alta    | automatizar CI/CD  |

---

## 13. Control de cambios

| Versión | Fecha      | Autor  | Descripción                    |
| ------- | ---------- | ------ | ------------------------------ |
| v1.0    | 2026-03-02 | DevOps | Definición inicial de entornos |

---

```
