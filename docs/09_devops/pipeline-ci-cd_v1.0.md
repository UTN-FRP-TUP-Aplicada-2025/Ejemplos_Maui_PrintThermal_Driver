# Pipeline CI/CD  
**Proyecto:** Reclamos Ciudadanos  
**Versión:** v1.0  
**Estado:** Aprobado  
**Fecha:** 2026-03-02  
**Owner:** DevOps  

---

## 1. Propósito

Este documento define el pipeline de Integración Continua y Entrega Continua (CI/CD) del proyecto Reclamos Ciudadanos. Su objetivo es automatizar la construcción, validación y despliegue del sistema para asegurar calidad, trazabilidad y rapidez en la entrega de valor.

---

## 2. Alcance

El pipeline cubre:

- Build del backend  
- Ejecución de tests  
- Análisis de calidad  
- Empaquetado de artefactos  
- Despliegue a entornos (DEV y QA en v1.0)  

Fuera de alcance en v1.0:

- Despliegue automático a producción  
- Pruebas de performance automatizadas  
- Escaneo de seguridad avanzado  

---

## 3. Estrategia general

El pipeline se ejecuta ante:

- Push a ramas principales  
- Pull Requests  
- Tags de release  

Flujo de alto nivel:

```text
Commit → Build → Tests → Quality → Package → Deploy
````

---

## 4. Disparadores (triggers)

### 4.1 Pull Request

Se ejecuta:

* Build
* Tests
* Análisis estático

**Objetivo:** prevenir errores antes del merge.

---

### 4.2 Push a develop/main

Se ejecuta:

* Build completo
* Tests
* Empaquetado
* Deploy a DEV

---

### 4.3 Tag de versión

Se ejecuta:

* Build release
* Tests completos
* Empaquetado versionado
* Deploy a QA
* (Producción manual en v1.0)

---

## 5. Etapas del pipeline

---

### 5.1 Stage: Checkout

**Objetivo:** obtener el código fuente.

**Pasos**

* Clonar repositorio
* Restaurar submódulos (si aplica)
* Validar versión

---

### 5.2 Stage: Build

**Objetivo:** compilar la solución.

**Pasos**

* Restaurar dependencias
* Compilar proyecto
* Fallar ante error de compilación

**Criterio de éxito**

* Build sin errores

---

### 5.3 Stage: Tests

**Objetivo:** validar calidad funcional básica.

**Incluye**

* Tests unitarios
* Tests de integración (si aplica)

**Reglas**

* Cobertura mínima: 70%
* Cualquier test fallido → pipeline falla

---

### 5.4 Stage: Análisis de calidad

**Objetivo:** detectar problemas de código.

**Incluye**

* Análisis estático
* Detección de code smells
* Validación de convenciones

**Regla**

* Issues críticos → pipeline falla

---

### 5.5 Stage: Empaquetado

**Objetivo:** generar artefacto desplegable.

**Salida**

* Imagen de contenedor o paquete
* Versionado con SemVer
* Publicación en registry

**Ejemplo de nombre**

```text
reclamos-api:1.1.0
```

---

### 5.6 Stage: Deploy DEV

**Objetivo:** validación continua temprana.

**Condición**

* Solo desde rama develop/main

**Acciones**

* Desplegar servicio
* Ejecutar smoke tests
* Verificar health endpoint

---

### 5.7 Stage: Deploy QA

**Objetivo:** validación funcional formal.

**Condición**

* Solo en tags de release

**Acciones**

* Deploy controlado
* Notificación a QA
* Registro de versión

---

## 6. Gates de calidad

El pipeline falla si ocurre cualquiera de los siguientes:

* Error de compilación
* Tests fallidos
* Cobertura < 70%
* Issues críticos de análisis
* Fallo en smoke tests

---

## 7. Versionado en el pipeline

La versión se determina por:

1. Tag git (prioritario)
2. Número de build (fallback)

**Ejemplo**

* Tag: v1.2.0 → versión 1.2.0
* Sin tag → 1.2.0-build.45

---

## 8. Variables de entorno

Variables mínimas:

* DATABASE_CONNECTION
* API_KEYS (seguras)
* ENVIRONMENT
* LOG_LEVEL

**Regla**

Nunca hardcodear secretos en el repositorio.

---

## 9. Estrategia de rollback

En caso de fallo en producción (manual en v1.0):

* Mantener última imagen estable
* Permitir redeploy inmediato
* Migraciones reversibles cuando sea posible

---

## 10. Observabilidad del pipeline

Se debe registrar:

* Duración de build
* Tasa de fallos
* Cobertura de tests
* Frecuencia de deploy

---

## 11. Ejemplo simplificado (pseudo YAML)

```yaml
stages:
  - build
  - test
  - quality
  - package
  - deploy_dev

build:
  script: dotnet build

test:
  script: dotnet test

package:
  script: docker build -t reclamos-api .

deploy_dev:
  script: ./deploy-dev.sh
  only:
    - develop
```

---

## 12. Riesgos

| Riesgo           | Impacto | Mitigación                  |
| ---------------- | ------- | --------------------------- |
| Builds lentos    | Media   | cache dependencias          |
| Tests inestables | Alta    | aislar pruebas              |
| Deploy manual    | Media   | automatizar progresivamente |

---

## 13. Evolución futura

Planeado para v2.x:

* Deploy automático a producción
* Pruebas de performance
* Escaneo de seguridad
* Canary releases

---

## 14. Control de cambios

| Versión | Fecha      | Autor  | Descripción                     |
| ------- | ---------- | ------ | ------------------------------- |
| v1.0    | 2026-03-02 | DevOps | Definición inicial del pipeline |

---

```