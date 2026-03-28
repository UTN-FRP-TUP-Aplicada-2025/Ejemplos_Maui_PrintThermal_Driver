# Prototipo de Navegación — Sistema de Gestión de Reclamos  
**Versión:** 1.0  
**Fecha:** 2026-03-02  
**Autor:** Equipo de Análisis  
**Estado:** Borrador  

---

## 1. Objetivo

Este documento describe el prototipo de navegación del Sistema de Gestión de Reclamos. Su finalidad es visualizar la estructura de pantallas, los flujos principales de interacción del usuario y la organización general de la aplicación, sin definir aún detalles técnicos ni de diseño visual final.

El prototipo permite validar tempranamente la experiencia de usuario, detectar inconsistencias en el flujo y alinear a los stakeholders respecto del comportamiento esperado del sistema.

---

## 2. Alcance del Prototipo

Incluye:

- Pantallas principales del ciudadano
- Navegación básica entre vistas
- Flujo de registro de reclamo
- Flujo de consulta de estado

No incluye:

- Diseño gráfico definitivo  
- Validaciones técnicas detalladas  
- Integraciones externas  
- Reglas de negocio complejas  

---

## 3. Mapa de Navegación General

```text
[Inicio]
   ├── [Registrar Reclamo]
   │        └── [Confirmación de Registro]
   │
   ├── [Consultar Estado]
   │        └── [Detalle del Reclamo]
   │
   └── [Mis Reclamos]
````

---

## 4. Descripción de Pantallas

### 4.1 Pantalla: Inicio

**Objetivo:** Punto de entrada del ciudadano.

**Elementos principales:**

* Botón “Registrar Reclamo”
* Botón “Consultar Estado”
* Acceso a “Mis Reclamos”
* Información institucional

**Acciones:**

* Navegar a registro
* Navegar a consulta
* Navegar a listado personal

---

### 4.2 Pantalla: Registrar Reclamo

**Objetivo:** Permitir al ciudadano cargar un nuevo reclamo.

**Campos del formulario:**

* Tipo de reclamo (lista)
* Descripción (texto libre)
* Ubicación (opcional)
* Medio de contacto

**Acciones:**

* Confirmar registro
* Cancelar

**Resultado esperado:**

Al confirmar, el sistema:

* Genera número de reclamo
* Aplica reglas de negocio
* Asigna área responsable
* Muestra pantalla de confirmación

---

### 4.3 Pantalla: Confirmación de Registro

**Objetivo:** Informar que el reclamo fue creado correctamente.

**Elementos:**

* Número de reclamo generado
* Mensaje de éxito
* Botón “Ver estado”
* Botón “Volver al inicio”

---

### 4.4 Pantalla: Consultar Estado

**Objetivo:** Permitir al ciudadano consultar un reclamo existente.

**Campos:**

* Número de reclamo
* Documento del ciudadano (si aplica)

**Acciones:**

* Buscar
* Volver

**Resultado:**

* Navega al detalle del reclamo si existe
* Muestra mensaje de error si no existe

---

### 4.5 Pantalla: Detalle del Reclamo

**Objetivo:** Mostrar el estado actualizado del reclamo.

**Información mostrada:**

* Número de reclamo
* Tipo
* Estado actual
* Fecha de creación
* Área responsable
* Historial de cambios

**Acciones:**

* Volver
* Ir a inicio

---

### 4.6 Pantalla: Mis Reclamos

**Objetivo:** Listar reclamos del ciudadano autenticado (si aplica).

**Elementos:**

* Tabla/listado de reclamos
* Estado
* Fecha
* Acceso a detalle

---

## 5. Flujo Principal — Registrar Reclamo

```text
Inicio
  ↓
Registrar Reclamo
  ↓
Completar Formulario
  ↓
Confirmar
  ↓
Asignación automática
  ↓
Confirmación
```

---

## 6. Flujo Principal — Consultar Estado

```text
Inicio
  ↓
Consultar Estado
  ↓
Ingresar número
  ↓
Buscar
  ↓
Detalle del reclamo
```

---

## 7. Consideraciones de UX

* El flujo debe requerir la menor cantidad de pasos posible.
* El número de reclamo debe mostrarse claramente.
* Los mensajes de error deben ser comprensibles.
* La navegación debe permitir volver fácilmente al inicio.
* Debe ser usable desde dispositivos móviles.

---

## 8. Pendientes para próximas versiones

* Wireframes de baja fidelidad
* Prototipo visual navegable
* Estados intermedios del reclamo
* Manejo de usuario autenticado
* Notificaciones al ciudadano

---

## 9. Historial de Versiones

| Versión | Fecha      | Autor              | Cambios                                     |
| ------- | ---------- | ------------------ | ------------------------------------------- |
| 1.0     | 2026-03-02 | Equipo de Análisis | Versión inicial del prototipo de navegación |

---

```
