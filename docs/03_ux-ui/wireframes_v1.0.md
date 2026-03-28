# Wireframes — Sistema de Gestión de Reclamos  
**Versión:** 1.0  
**Fecha:** 2026-03-02  
**Autor:** Equipo de Análisis  
**Estado:** Borrador  

---

## 1. Objetivo

Este documento presenta wireframes de baja fidelidad para las principales pantallas del Sistema de Gestión de Reclamos. Su propósito es visualizar la disposición de los elementos de interfaz y validar la experiencia de usuario antes de avanzar al diseño visual definitivo.

Los wireframes se centran en estructura y flujo de interacción, por lo que no representan estilos gráficos finales, colores ni tipografías.

---

## 2. Alcance

Incluye wireframes de:

- Pantalla de inicio  
- Registro de reclamo  
- Confirmación de registro  
- Consulta de estado  
- Detalle del reclamo  
- Mis reclamos  

---

## 3. Convenciones

- `[Botón]` → acción del usuario  
- `(Campo)` → campo de entrada  
- `{Texto}` → información mostrada  
- `---` → separación visual  

---

## 4. Wireframe — Pantalla de Inicio

```text
+--------------------------------------------------+
|        SISTEMA DE RECLAMOS MUNICIPALES           |
+--------------------------------------------------+

   {Bienvenido}

   [Registrar Reclamo]

   [Consultar Estado]

   [Mis Reclamos]

----------------------------------------------------
   {Información institucional}
----------------------------------------------------
````

**Notas:**

* Pantalla simple y clara.
* Botones principales centrados.
* Debe ser responsive.

---

## 5. Wireframe — Registrar Reclamo

```text
+--------------------------------------------------+
|              Registrar Reclamo                   |
+--------------------------------------------------+

Tipo de reclamo:
(Seleccionar tipo v)

Descripción:
(Texto multilinea.........................)

Ubicación (opcional):
(Texto)

Medio de contacto:
(Texto)

--------------------------------------------------

            [Confirmar]   [Cancelar]
```

**Notas:**

* La descripción debe permitir texto largo.
* El tipo de reclamo es obligatorio.
* Validaciones se muestran inline.

---

## 6. Wireframe — Confirmación de Registro

```text
+--------------------------------------------------+
|                Reclamo Registrado                |
+--------------------------------------------------+

   ✔ {Su reclamo fue registrado correctamente}

   Número de reclamo:
   {RC-00012345}

--------------------------------------------------

      [Ver Estado]     [Volver al Inicio]
```

**Notas:**

* El número debe destacarse visualmente.
* Debe poder copiarse fácilmente.

---

## 7. Wireframe — Consultar Estado

```text
+--------------------------------------------------+
|               Consultar Estado                   |
+--------------------------------------------------+

Número de reclamo:
(Texto)

Documento (opcional):
(Texto)

--------------------------------------------------

             [Buscar]   [Volver]
```

**Notas:**

* Validar formato del número.
* Mostrar error si no existe.

---

## 8. Wireframe — Detalle del Reclamo

```text
+--------------------------------------------------+
|               Detalle del Reclamo                |
+--------------------------------------------------+

Número: {RC-00012345}
Tipo: {Alumbrado}
Estado: {En proceso}
Fecha: {01/03/2026}
Área: {Servicios Públicos}

--------------------------------------------------

Historial:

- 01/03/2026 — Reclamo registrado
- 02/03/2026 — Asignado a área
- 04/03/2026 — En atención

--------------------------------------------------

                 [Volver]
```

**Notas:**

* El estado debe ser visualmente destacado.
* Historial ordenado cronológicamente.

---

## 9. Wireframe — Mis Reclamos

```text
+--------------------------------------------------+
|                 Mis Reclamos                     |
+--------------------------------------------------+

| Número     | Tipo        | Estado      | Fecha |
|------------|-------------|-------------|-------|
| RC-000123  | Alumbrado   | En proceso  | 01/03 |
| RC-000122  | Baches      | Cerrado     | 28/02 |

--------------------------------------------------

Seleccionar fila → ver detalle

                [Volver]
```

**Notas:**

* Tabla responsive.
* Permitir ordenamiento futuro.
* Posible paginación en versiones siguientes.

---

## 10. Pendientes para próximas versiones

* Wireframes de alta fidelidad
* Estados de error detallados
* Versiones mobile-first
* Accesibilidad (WCAG)
* Prototipo navegable en herramienta UX

---

## 11. Historial de Versiones

| Versión | Fecha      | Autor              | Cambios                       |
| ------- | ---------- | ------------------ | ----------------------------- |
| 1.0     | 2026-03-02 | Equipo de Análisis | Versión inicial de wireframes |

---

```
