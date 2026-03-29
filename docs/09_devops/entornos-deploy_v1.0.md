# Entornos de Deploy  
**Archivo:** entornos-deploy_v1.0.md  
**Proyecto:** Motor DSL (Librería de Generación de Documentos)  
**Versión:** v1.0  
**Estado:** Aprobado  
**Fecha:** 2026-03-28  
**Owner:** Arquitectura / DevOps  

---

## 1. Propósito

Este documento define los entornos de despliegue del motor DSL, sus responsabilidades, configuraciones generales y reglas de promoción entre ambientes. El objetivo es asegurar consistencia, aislamiento, trazabilidad y calidad en el ciclo de entrega continua de la librería y sus componentes asociados.

---

## 2. Estrategia general

El sistema adopta una estrategia de despliegue progresivo por entornos, donde cada cambio atraviesa validaciones crecientes antes de llegar a producción.

Flujo de promoción:

```text
DEV → QA → STAGING (opcional) → PROD
````

Cada entorno cumple un rol específico dentro del ciclo de validación del motor, sus extensiones y sus artefactos.

---

## 3. Entorno de Desarrollo (DEV)

### 3.1 Propósito

Permitir a los desarrolladores del motor construir, probar y validar nuevas funcionalidades del DSL, renderizadores y extensiones de manera ágil.

### 3.2 Características

* Deploy frecuente (por commit o merge)
* Datos de prueba / ejemplos DSL
* Logging detallado
* Debug habilitado
* Configuración flexible
* Entorno no estable

### 3.3 Uso principal

* Desarrollo de nuevas capacidades del motor
* Validación de nodos DSL
* Pruebas de renderizado
* Integración de extensiones

### 3.4 Responsables

* Equipo de desarrollo del motor

---

## 4. Entorno de QA / Testing

### 4.1 Propósito

Validar el comportamiento funcional del motor DSL contra especificaciones, casos de uso y criterios de validación.

### 4.2 Características

* Deploy por versión o feature
* Datos controlados
* Logging moderado
* Configuración cercana a producción
* Acceso restringido

### 4.3 Uso principal

* Validación de casos de prueba
* Ejecución de pruebas funcionales del DSL
* Validación de renderizadores
* Verificación de extensiones

### 4.4 Responsables

* QA
* Desarrollo (soporte)

---

## 5. Entorno de Staging / Preproducción

> Estado opcional en v1.0 — recomendado para evolución futura

### 5.1 Propósito

Simular un entorno equivalente a producción para validar releases del motor antes de su publicación.

### 5.2 Características

* Configuración idéntica a producción
* Versiones específicas del motor
* Datos anonimizados o representativos
* Logging productivo
* Seguridad completa habilitada
* Alta estabilidad requerida

### 5.3 Uso principal

* Smoke tests de release del motor
* Validación de artefactos publicados
* Pruebas de integración completas
* Validación de compatibilidad de extensiones

### 5.4 Responsables

* DevOps
* QA
* Arquitectura

---

## 6. Entorno de Producción (PROD)

### 6.1 Propósito

Publicar versiones estables del motor DSL como librería consumible (paquete NuGet o equivalente) y/o servicios asociados.

### 6.2 Características

* Alta disponibilidad (si aplica como servicio)
* Logging controlado
* Monitoreo activo
* Seguridad reforzada
* Versiones inmutables

### 6.3 Reglas

* Solo versiones aprobadas pueden publicarse
* Requiere pipeline CI/CD exitoso
* Requiere versionado SemVer
* Cada release debe estar etiquetado

### 6.4 Responsables

* DevOps
* Arquitectura
* Mantenimiento de plataforma

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

* No hardcodear configuraciones en el motor
* Usar variables de entorno o archivos de configuración externos
* Separar configuración de código y de DSL

### Ejemplos de variables

* ENVIRONMENT
* LOG_LEVEL
* RENDER_MODE
* ENABLE_EXTENSIONS
* CACHE_SETTINGS

---

## 9. Estrategia de versionado

Formato:

```text
MAJOR.MINOR.PATCH
```

Ejemplos:

* 1.0.0 → release inicial del motor
* 1.1.0 → nuevas capacidades DSL o renderizado
* 1.1.1 → correcciones

### Reglas

* Cada deploy a QA debe estar asociado a una versión
* Cada release a PROD debe estar versionado y taggeado
* No se permiten versiones no trazables en producción

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
9. Publicación en PROD

### Criterios de promoción

* Build exitoso
* Tests en verde
* Validaciones de DSL correctas
* Casos de prueba aprobados
* Sin defectos críticos

---

## 11. Estrategia de despliegue

* Artefactos inmutables por versión
* Publicación versionada (ej: NuGet)
* Separación entre build y deploy
* Deploy automatizado vía CI/CD

---

## 12. Rollback

### Estrategia

* Mantener versiones anteriores del motor disponibles
* Posibilidad de volver a una versión previa
* Control de compatibilidad con extensiones

### Cuándo aplicar

* Fallos críticos en renderizado
* Incompatibilidad DSL
* Errores en nuevas versiones
* Problemas de integración con extensiones

---

## 13. Observabilidad por entorno

Se recomienda progresivamente:

* Logs estructurados
* Métricas del motor (tiempo de render, errores)
* Health checks
* Trazabilidad de ejecución DSL
* Alertas en producción

---

## 14. Seguridad por entorno

* DEV: accesos amplios controlados
* QA: accesos restringidos
* STAGING: accesos limitados
* PROD: accesos estrictamente controlados

Incluye:

* Gestión de secretos
* Control de dependencias externas
* Validación de inputs DSL
* Protección contra ejecución no segura

---

## 15. Riesgos

| Riesgo                     | Impacto | Mitigación                    |
| -------------------------- | ------- | ----------------------------- |
| Diferencias entre entornos | Alta    | Configuración por entorno     |
| Incompatibilidad DSL       | Alta    | versionado SemVer + tests     |
| Extensiones incompatibles  | Alta    | contratos y versionado        |
| Deploy manual              | Alta    | automatización CI/CD          |
| Falta de trazabilidad      | Alta    | tags + versionado obligatorio |

---

## 16. Control de cambios

| Versión | Fecha      | Autor  | Descripción                    |
| ------- | ---------- | ------ | ------------------------------ |
| v1.0    | 2026-03-28 | DevOps | Definición inicial de entornos |

---
