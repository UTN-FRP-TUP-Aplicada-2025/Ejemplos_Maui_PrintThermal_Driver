# Definition of Done (DoD)  
**Proyecto:** Reclamos Ciudadanos  
**Versión:** v1.0  
**Estado:** Aprobado  
**Fecha:** 2026-03-02  
**Owner:** Equipo de Desarrollo  

---

## 1. Propósito

La Definition of Done establece los criterios mínimos que debe cumplir cualquier historia de usuario, tarea técnica o incremento del producto para considerarse **completado** dentro de un sprint. Su objetivo es garantizar calidad homogénea, reducir ambigüedad y asegurar que lo entregado sea potencialmente desplegable.

---

## 2. Alcance

Esta DoD aplica a:

- Historias de usuario
- Tareas técnicas
- Bugs
- Cambios en API
- Cambios en base de datos

No reemplaza los criterios de aceptación funcionales, sino que los complementa.

---

## 3. Criterios obligatorios de completitud

Una tarea se considera **Done** solo si cumple TODOS los siguientes puntos.

---

### 3.1 Código

- El código compila sin errores.
- No existen warnings críticos.
- Sigue las convenciones de codificación del proyecto.
- No contiene código comentado innecesario.
- No contiene secretos ni credenciales hardcodeadas.

**Ejemplo**

✔ Correcto: método limpio y nombrado según convención  
✘ Incorrecto: código con TODO pendientes o claves embebidas

---

### 3.2 Pruebas unitarias

- Se agregaron o actualizaron pruebas unitarias.
- La cobertura mínima del módulo afectado es ≥ 70%.
- Todas las pruebas pasan en CI.
- Se prueban casos felices y casos de error.

**Ejemplo**

Para “Registrar reclamo”:

- ✔ test_registro_valido  
- ✔ test_descripcion_vacia  
- ✔ test_tipo_invalido  

---

### 3.3 Validación funcional

- Se cumplen los criterios de aceptación del caso de uso.
- Se verificó el flujo principal.
- Se verificaron escenarios alternativos relevantes.
- No se detectan regresiones visibles.

**Ejemplo**

Para CU-01:

- ✔ se registra reclamo  
- ✔ se asigna área automáticamente  
- ✔ se devuelve número de reclamo  

---

### 3.4 API y contratos (si aplica)

- El endpoint respeta el contrato definido.
- Se actualizaron ejemplos de request/response si cambian.
- Se validaron códigos HTTP correctos.
- La API es backward compatible o está versionada.

**Ejemplo**

POST /api/reclamos devuelve:

- 201 Created en éxito  
- 400 Bad Request en validación  

---

### 3.5 Base de datos (si aplica)

- Migraciones creadas y versionadas.
- Scripts reproducibles.
- No se rompen datos existentes.
- Índices creados si corresponde.

**Ejemplo**

✔ migración `V3__agregar_tabla_reclamos.sql` incluida  
✘ cambio manual en base sin script

---

### 3.6 UX/UI (si aplica)

- La pantalla respeta wireframes aprobados.
- No hay desbordes visuales.
- Textos revisados.
- Estados de carga y error implementados.

**Ejemplo**

En pantalla de registro:

- ✔ loading visible  
- ✔ mensaje de error claro  
- ✔ botón deshabilitado durante envío  

---

### 3.7 Seguridad básica

- Validación de inputs implementada.
- No se exponen datos sensibles.
- Se respeta autenticación requerida.
- Se previenen inyecciones básicas.

**Ejemplo**

✔ sanitización de descripción  
✔ validación de longitud  

---

### 3.8 Revisión de código

- Pull Request creado.
- Revisado por al menos 1 miembro del equipo.
- Comentarios resueltos.
- Merge realizado a rama principal del sprint.

---

### 3.9 Documentación

- Se actualizó documentación técnica si aplica.
- Se actualizaron contratos si cambiaron.
- Se actualizó backlog si hubo ajustes.
- Se versionaron artefactos impactados.

**Ejemplo**

Si cambia la API:

- ✔ contratos-api actualizado  
- ✔ backlog ajustado  

---

### 3.10 Integración continua

- Build de CI exitoso.
- Tests automáticos en verde.
- Análisis estático sin issues críticos.
- Artefacto generado correctamente.

---

## 4. Checklist rápido (para el equipo)

Antes de mover a **Done**, verificar:

- [ ] Código limpio y compilando  
- [ ] Tests unitarios pasando  
- [ ] Criterios de aceptación cumplidos  
- [ ] PR revisado y aprobado  
- [ ] Documentación actualizada  
- [ ] Build CI en verde  
- [ ] Sin deuda técnica crítica introducida  

---

## 5. Ejemplo aplicado

**Historia:** Registrar reclamo

Se considera Done cuando:

- El usuario puede registrar un reclamo.
- El sistema asigna área automáticamente.
- Se devuelve número de reclamo.
- Existen tests unitarios.
- La API cumple contrato.
- El PR fue aprobado.
- El build CI pasó correctamente.

---

## 6. Control de cambios

| Versión | Fecha | Autor | Descripción |
|--------|-------|-------|-------------|
| v1.0 | 2026-03-02 | Equipo | Definición inicial de DoD |

---