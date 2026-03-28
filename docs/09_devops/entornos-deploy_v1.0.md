````markdown
# entornos-deploy_v1.0.md  
**Archivo:** entornos-deploy_v1.0.md  
**Proyecto:** Reclamos Ciudadanos  
**Versión:** v1.0  
**Estado:** Aprobado  
**Fecha:** 2026-03-28  
**Owner:** Arquitectura / DevOps  

---

## 1. Propósito

Este documento define los entornos de despliegue del sistema Reclamos Ciudadanos, sus responsabilidades, configuraciones generales y reglas de promoción entre ambientes. El objetivo es asegurar consistencia, aislamiento, trazabilidad y calidad en el ciclo de entrega continua.

---

## 2. Estrategia general

El sistema adopta una estrategia de despliegue progresivo por entornos, donde cada cambio atraviesa validaciones crecientes antes de llegar a producción.

Flujo de promoción:

```text
DEV → QA → STAGING (opcional) → PROD
````

Cada entorno cumple un rol específico dentro del ciclo de validación y liberación.

---

## 3. Entorno de Desarrollo (DEV)

### 3.1 Propósito

Permitir a los desarrolladores construir, integrar y validar funcionalidades de manera ágil.

### 3.2 Características

* Deploy frecuente (on commit o merge)
* Datos sintéticos o de prueba
* Logging detallado
* Debug habilitado
* Configuración flexible
* Baja criticidad de estabilidad

### 3.3 Uso principal

* Desarrollo de nuevas funcionalidades
* Validación técnica
* Integración temprana
* Pruebas unitarias e integración básica

### 3.4 Responsables

* Equipo de desarrollo

---

## 4. Entorno de QA / Testing

### 4.1 Propósito

Validar el comportamiento funcional del sistema contra los requisitos definidos.

### 4.2 Características

* Deploy por feature o por release
* Datos controlados y consistentes
* Logging moderado
* Configuración cercana a producción
* Acceso restringido

### 4.3 Uso principal

* Pruebas funcionales
* Pruebas de regresión
* Validación de criterios de aceptación
* Ejecución de casos de prueba

### 4.4 Responsables

* QA
* Desarrollo (soporte)

---

## 5. Entorno de Staging / Preproducción

> Estado opcional en v1.0 — recomendado para evolución futura

### 5.1 Propósito

Simular producción lo más fielmente posible antes de liberar cambios.

### 5.2 Características

* Configuración idéntica a producción
* Datos anonimizados o representativos
* Logging productivo
* Seguridad completa habilitada
* Alta estabilidad requerida

### 5.3 Uso principal

* Smoke tests finales
* Validación de despliegue
* Validación de integraciones externas
* Pruebas de aceptación finales

### 5.4 Responsables

* DevOps
* QA
* Arquitectura

---

## 6. Entorno de Producción (PROD)

### 6.1 Propósito

Proveer el servicio en vivo para usuarios finales.

### 6.2 Características

* Alta disponibilidad
* Escalabilidad horizontal
* Logging controlado
* Monitoreo activo
* Seguridad reforzada
* Cambios estrictamente controlados

### 6.3 Reglas

* Solo versiones aprobadas pueden desplegarse
* Requiere pipeline CI exitoso
* Requiere validación en QA
* Requiere versión etiquetada (tag)

### 6.4 Responsables

* DevOps
* Operaciones
* Arquitectura (gobernanza)

---

## 7. Configuración por entorno

| Configuración     | DEV    | QA    | STAGING | PROD       |
| ----------------- | ------ | ----- | ------- | ---------- |
| Debug             | Sí     | No    | No      | No         |
| Logging           | Alto   | Medio | Medio   | Bajo       |
| Datos reales      | No     | No    | Parcial | Sí         |
| Deploy automático | Sí     | Sí    | Parcial | Controlado |
| Monitoreo         | Básico | Medio | Alto    | Completo   |

---

## 8. Gestión de configuración

Cada entorno debe definir su configuración mediante variables externas.

### Reglas

* No hardcodear configuraciones
* Usar variables de entorno o servicios de configuración
* Separar configuración de código

### Ejemplos de variables

* DATABASE_URL
* API_KEYS
* ENVIRONMENT
* LOG_LEVEL

---

## 9. Estrategia de versionado

Formato:

```text
MAJOR.MINOR.PATCH
```

Ejemplos:

* 1.0.0 → release inicial
* 1.1.0 → nueva funcionalidad
* 1.1.1 → bugfix

### Reglas

* Cada deploy a QA debe estar asociado a una versión
* Cada deploy a PROD debe estar taggeado
* No se permiten despliegues sin versión trazable

---

## 10. Promoción entre entornos

### Flujo obligatorio

1. Merge a rama principal
2. Ejecución del pipeline CI/CD
3. Deploy automático a DEV
4. Validación técnica
5. Promoción a QA
6. Validación funcional
7. Promoción a STAGING (si aplica)
8. Aprobación final
9. Deploy a PROD

### Criterios de promoción

* Tests en verde
* Build exitoso
* Sin defectos críticos
* Validación QA aprobada

---

## 11. Estrategia de despliegue

Se recomienda:

* Deploy inmutable por versión
* Uso de artefactos versionados (containers o paquetes)
* Despliegues automatizados desde CI/CD
* Separación entre build y deploy

---

## 12. Rollback

### Estrategia

* Mantener versiones anteriores disponibles
* Deploy basado en versiones específicas
* Uso de migraciones reversibles cuando aplique

### Cuándo aplicar

* Fallos críticos en producción
* Problemas de performance severos
* Errores en migraciones
* Incidencias de disponibilidad

---

## 13. Observabilidad por entorno

Se recomienda progresivamente:

* Logs estructurados
* Métricas de aplicación
* Health checks
* Alertas básicas en DEV/QA
* Monitoreo completo en PROD

---

## 14. Seguridad por entorno

* DEV: accesos amplios controlados
* QA: accesos restringidos
* STAGING: accesos limitados
* PROD: accesos estrictamente controlados

Incluye:

* Gestión de secretos
* Autenticación y autorización
* Protección de endpoints sensibles

---

## 15. Riesgos

| Riesgo                     | Impacto | Mitigación                     |
| -------------------------- | ------- | ------------------------------ |
| Diferencias entre entornos | Alta    | Configuración por entorno      |
| Datos inconsistentes       | Media   | Uso de datos controlados       |
| Deploy manual              | Alta    | Automatización CI/CD           |
| Falta de trazabilidad      | Alta    | Versionado + tags obligatorios |

---

## 16. Control de cambios

| Versión | Fecha      | Autor  | Descripción                    |
| ------- | ---------- | ------ | ------------------------------ |
| v1.0    | 2026-03-28 | DevOps | Definición inicial de entornos |

---