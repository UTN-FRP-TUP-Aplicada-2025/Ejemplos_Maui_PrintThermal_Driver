# Plan de Pruebas  
**Proyecto:** Reclamos Ciudadanos  
**Versión:** v1.0  
**Estado:** Aprobado  
**Fecha:** 2026-03-02  
**Owner:** QA / Equipo de Desarrollo  

---

## 1. Propósito

Este plan define la estrategia, alcance, tipos de pruebas, responsabilidades y criterios para verificar que el sistema de Reclamos Ciudadanos cumple los requisitos funcionales y técnicos definidos. El objetivo es detectar defectos tempranamente y asegurar que cada incremento sea potencialmente liberable.

---

## 2. Alcance

Incluye pruebas sobre:

- Registro de reclamos  
- Consulta de estado  
- Asignación automática de área  
- API REST de reclamos  
- Validaciones de entrada  
- Persistencia de datos  

Fuera de alcance (v1.0):

- Pruebas de performance masiva  
- Pruebas de seguridad avanzada (pentesting)  
- Pruebas multi-idioma  

---

## 3. Estrategia de pruebas

La estrategia combina distintos niveles para reducir riesgo progresivamente.

### Niveles

1. Pruebas unitarias (automáticas)  
2. Pruebas de integración  
3. Pruebas funcionales  
4. Pruebas exploratorias  
5. Pruebas de regresión  

---

## 4. Tipos de pruebas

### 4.1 Pruebas unitarias

**Objetivo:** validar lógica aislada.

**Responsable:** Desarrollo  
**Automatización:** obligatoria  
**Cobertura mínima:** 70%

**Componentes a cubrir**

- Servicio de reclamos  
- Clasificador de tipo  
- Validaciones de entrada  

**Ejemplos**

- test_registro_valido  
- test_descripcion_vacia  
- test_tipo_no_reconocido  

---

### 4.2 Pruebas de integración

**Objetivo:** verificar interacción entre componentes.

**Responsable:** Desarrollo / QA  
**Automatización:** recomendada  

**Casos clave**

- API + base de datos  
- API + clasificador  
- Persistencia de reclamos  

**Ejemplo**

Registrar reclamo → verificar que se guarda y se asigna área.

---

### 4.3 Pruebas funcionales

**Objetivo:** validar comportamiento contra casos de uso.

**Responsable:** QA  
**Base:** especificación funcional

**Casos cubiertos**

- CU-01 Registrar reclamo  
- CU-02 Consultar estado  

**Ejemplo CU-01**

Flujo:

1. Usuario envía reclamo  
2. Sistema valida  
3. Sistema clasifica  
4. Sistema asigna área  
5. Sistema devuelve número  

Resultado esperado: éxito completo.

---

### 4.4 Pruebas de regresión

**Objetivo:** asegurar que cambios no rompen funcionalidad existente.

**Ejecución**

- En cada sprint  
- Antes de release  
- Automática cuando sea posible  

**Suite mínima**

- Registro válido  
- Consulta existente  
- Validaciones críticas  

---

### 4.5 Pruebas exploratorias

**Objetivo:** detectar comportamientos no previstos.

**Responsable:** QA  
**Momento:** durante el sprint

**Focos**

- Inputs extremos  
- Combinaciones inusuales  
- Experiencia de usuario  

---

## 5. Entornos de prueba

### 5.1 Entorno de desarrollo

- Uso: pruebas unitarias e integración temprana  
- Datos: sintéticos  
- Frecuencia: continua  

---

### 5.2 Entorno de testing (QA)

- Uso: pruebas funcionales  
- Datos: controlados  
- Deploy: por sprint  

---

### 5.3 Entorno de staging (opcional futuro)

- Uso: validación previa a producción  
- Datos: similares a producción  
- Automatización: recomendada  

---

## 6. Datos de prueba

### Datos válidos

- Descripción con texto realista  
- Tipos de reclamo conocidos  
- Usuarios autenticados  

### Datos inválidos

- Descripción vacía  
- Texto excesivamente largo  
- Tipo inexistente  
- Caracteres especiales  

**Ejemplo**

Entrada inválida:

```json
{
  "descripcion": "",
  "tipoReclamo": "XXX"
}
````

Resultado esperado: 400 Bad Request.

---

## 7. Criterios de entrada

Se ejecutan pruebas cuando:

* La historia está en estado Ready for QA
* El build CI está en verde
* El entorno de pruebas está disponible
* Los datos de prueba están cargados

---

## 8. Criterios de salida

Una funcionalidad se considera validada cuando:

* No hay defectos críticos abiertos
* Los criterios de aceptación se cumplen
* La regresión está en verde
* QA aprueba la historia

---

## 9. Gestión de defectos

### Severidad

* Crítica — bloquea funcionalidad principal
* Alta — afecta funcionalidad importante
* Media — impacto moderado
* Baja — cosmético

### Flujo

Nuevo → En análisis → En corrección → En validación → Cerrado

---

## 10. Automatización

### Obligatoria

* Pruebas unitarias
* Parte de integración

### Recomendada

* Smoke tests API
* Regresión crítica

### Futura (no v1.0)

* UI automation
* Performance tests

---

## 11. Métricas de calidad

Se monitorean:

* Cobertura de tests
* Defectos por sprint
* Defectos escapados a producción
* Tiempo medio de corrección

---

## 12. Riesgos y mitigaciones

| Riesgo                   | Impacto | Mitigación           |
| ------------------------ | ------- | -------------------- |
| Clasificación incorrecta | Alta    | tests de reglas      |
| Datos inválidos          | Media   | validaciones fuertes |
| Cambios de contrato API  | Alta    | tests de integración |

---

## 13. Control de cambios

| Versión | Fecha      | Autor | Descripción              |
| ------- | ---------- | ----- | ------------------------ |
| v1.0    | 2026-03-02 | QA    | Versión inicial del plan |

---

```