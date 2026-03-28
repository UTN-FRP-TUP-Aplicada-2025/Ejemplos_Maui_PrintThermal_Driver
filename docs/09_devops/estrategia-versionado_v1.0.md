# Estrategia de Versionado  
**Proyecto:** Reclamos Ciudadanos  
**Versión:** v1.0  
**Estado:** Aprobado  
**Fecha:** 2026-03-02  
**Owner:** Arquitectura / DevOps  

---

## 1. Propósito

Este documento define la estrategia de versionado del sistema Reclamos Ciudadanos, incluyendo reglas para numeración de versiones, versionado de artefactos, control de cambios y etiquetado de releases. El objetivo es asegurar trazabilidad, reproducibilidad de builds y control de evolución del producto.

---

## 2. Alcance

Aplica a:

- Código fuente  
- APIs  
- Base de datos  
- Artefactos de documentación  
- Releases del producto  

No aplica a:

- Versiones internas de librerías externas  
- Versiones locales de desarrollo no publicadas  

---

## 3. Esquema de versionado

El proyecto adopta **Semantic Versioning (SemVer)**.

Formato:

```text
MAJOR.MINOR.PATCH
````

### Significado

* **MAJOR** → cambios incompatibles
* **MINOR** → nuevas funcionalidades compatibles
* **PATCH** → correcciones de errores

---

## 4. Reglas de incremento

### 4.1 Incremento MAJOR

Se incrementa cuando:

* Cambia el contrato de API de forma incompatible
* Se rompe compatibilidad de datos
* Cambia comportamiento funcional crítico

**Ejemplo**

* 1.0.0 → 2.0.0
* Eliminación de un campo obligatorio en la API

---

### 4.2 Incremento MINOR

Se incrementa cuando:

* Se agrega funcionalidad nueva compatible
* Se agregan endpoints nuevos
* Se agregan campos opcionales

**Ejemplo**

* 1.0.0 → 1.1.0
* Nuevo endpoint de consulta avanzada

---

### 4.3 Incremento PATCH

Se incrementa cuando:

* Se corrigen bugs
* Se mejora performance sin cambiar contrato
* Se corrigen validaciones

**Ejemplo**

* 1.1.0 → 1.1.1
* Corrección en asignación automática

---

## 5. Versionado de documentación

Los documentos Markdown del repositorio se versionan en el nombre del archivo.

Formato:

```text
<nombre>_vX.Y.md
```

**Ejemplos**

* especificacion-funcional_v1.0.md
* CU-01-registrar-reclamo_v1.1.md
* prompt-clasificacion-reclamo_v1.1.md

### Reglas

* Cambios menores → mismo major/minor
* Cambios funcionales → subir MINOR
* Cambios estructurales grandes → subir MAJOR

---

## 6. Versionado de API

### 6.1 Estrategia

La API se versiona por URL.

Formato:

```text
/api/v{version}/reclamos
```

**Ejemplos**

* /api/v1/reclamos
* /api/v2/reclamos

### 6.2 Reglas

* Cambios incompatibles → nueva versión de API
* Cambios compatibles → misma versión
* Nunca romper clientes existentes

---

## 7. Versionado de base de datos

### 7.1 Estrategia

Se utilizan migraciones versionadas e inmutables.

Formato sugerido:

```text
V{numero}__descripcion.sql
```

**Ejemplos**

* V1__crear_tabla_reclamos.sql
* V2__agregar_indice_tipo.sql
* V3__agregar_campo_estado.sql

### 7.2 Reglas

* Nunca modificar migraciones aplicadas
* Siempre agregar nuevas migraciones
* Scripts reversibles cuando sea posible

---

## 8. Versionado de releases

Cada release productiva debe:

* Tener tag en repositorio
* Tener build reproducible
* Tener changelog asociado
* Estar desplegada en producción

### Formato de tag

```text
vMAJOR.MINOR.PATCH
```

**Ejemplos**

* v1.0.0
* v1.1.0
* v1.1.1

---

## 9. Branching strategy (simplificada)

Estrategia recomendada:

```text
main
 └── develop
      ├── feature/*
      ├── bugfix/*
      └── hotfix/*
```

### Reglas

* `feature/*` → nuevas funcionalidades
* `bugfix/*` → correcciones normales
* `hotfix/*` → correcciones urgentes en PROD
* `main` siempre deployable

---

## 10. Control de cambios

Cada versión debe registrar:

* funcionalidades nuevas
* bugs corregidos
* cambios breaking
* migraciones asociadas

Se recomienda mantener un **CHANGELOG.md**.

---

## 11. Ejemplo completo de evolución

```text
v1.0.0  → release inicial
v1.1.0  → agrega consulta de estado
v1.1.1  → corrige validación de descripción
v2.0.0  → cambia contrato de API
```

---

## 12. Riesgos y mitigaciones

| Riesgo                   | Impacto | Mitigación          |
| ------------------------ | ------- | ------------------- |
| Versionado inconsistente | Alta    | reglas SemVer       |
| Cambios breaking ocultos | Alta    | revisión de API     |
| Migraciones manuales     | Alta    | scripts versionados |

---

## 13. Control de cambios del documento

| Versión | Fecha      | Autor        | Descripción        |
| ------- | ---------- | ------------ | ------------------ |
| v1.0    | 2026-03-02 | Arquitectura | Definición inicial |

---

```
