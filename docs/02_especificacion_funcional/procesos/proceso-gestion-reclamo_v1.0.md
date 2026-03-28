# Proceso de Negocio: Gestión de Reclamo

**Proyecto:** Sistema de Gestión de Reclamos Ciudadanos
**Documento:** proceso-gestion-reclamo_v1.0.md
**Versión:** 1.0
**Estado:** Aprobado
**Fecha:** 2026-03-02
**Autor:** Equipo Funcional

---

# 1. Propósito

Este documento describe el proceso de negocio completo para la gestión de un reclamo ciudadano, desde su registro hasta su cierre. Su objetivo es proporcionar una visión de extremo a extremo del flujo operativo que el sistema debe soportar.

El proceso permite alinear a las áreas de negocio y técnicas respecto de cómo circula un reclamo dentro de la organización, identificando responsabilidades, decisiones y puntos de control.

---

# 2. Alcance

El proceso inicia cuando un ciudadano registra un reclamo y finaliza cuando el reclamo es resuelto y notificado al ciudadano. Incluye la asignación automática, la gestión por parte del área responsable y la comunicación del resultado.

Quedan fuera del alcance los procesos de mantenimiento de catálogos maestros y las integraciones con sistemas externos.

---

# 3. Actores del Proceso

* **Ciudadano** — Origina el reclamo.
* **Sistema** — Ejecuta validaciones, asignaciones y notificaciones.
* **Operador Municipal** — Gestiona y resuelve el reclamo.
* **Área Responsable** — Unidad organizativa que atiende el reclamo.

**Ejemplo:**
Un vecino reporta un bache y el área de Obras Públicas lo repara.

---

# 4. Disparador del Proceso

El proceso se inicia cuando el ciudadano confirma el envío del formulario de registro de reclamo.

**Evento de inicio:** Reclamo registrado.

---

# 5. Flujo Principal del Proceso

1. El ciudadano registra un reclamo.
2. El sistema valida los datos obligatorios.
3. El sistema genera el identificador del reclamo.
4. El sistema asigna automáticamente el área responsable.
5. El reclamo queda en estado “Registrado”.
6. El área responsable visualiza el reclamo en su bandeja.
7. Un operador toma el reclamo para gestión.
8. El operador cambia el estado a “En proceso”.
9. El área ejecuta la acción correctiva correspondiente.
10. El operador registra la resolución.
11. El sistema cambia el estado a “Resuelto”.
12. El sistema notifica al ciudadano.
13. El proceso finaliza.

---

# 6. Flujos Alternativos Relevantes

## FA-01 Datos inválidos en el registro

Si en la validación inicial faltan datos obligatorios:

* El sistema rechaza el registro.
* Se informa al ciudadano.
* El proceso no se inicia.

---

## FA-02 Reclamo sin correspondencia de área

Si el tipo de reclamo no tiene área configurada:

* El sistema asigna “Mesa de Entradas”.
* Se registra advertencia.
* El proceso continúa normalmente.

---

## FA-03 Reclamo no resoluble

Si el área determina que el reclamo no corresponde:

* El operador cambia el estado a “Rechazado”.
* Se registra el motivo.
* El sistema notifica al ciudadano.
* El proceso finaliza.

---

# 7. Estados del Reclamo

Secuencia típica:

Registrado → En proceso → Resuelto

Estados alternativos:

Registrado → Rechazado

**Regla:** Un reclamo en estado final no puede volver a estados anteriores.

---

# 8. Puntos de Control

* Validación de datos obligatorios.
* Correcta asignación de área.
* Cambio de estado con trazabilidad.
* Notificación efectiva al ciudadano.

Estos puntos son críticos para asegurar calidad operativa y auditabilidad.

---

# 9. Indicadores del Proceso (KPIs)

* Tiempo medio de resolución.
* Cantidad de reclamos por área.
* Porcentaje de reclamos rechazados.
* Tiempo de primera atención.

**Ejemplo:**
Tiempo medio objetivo: ≤ 72 horas.

---

# 10. Trazabilidad

| Elemento                    | Relación           |
| --------------------------- | ------------------ |
| CU-01 Registrar Reclamo     | Inicio del proceso |
| RN-01 Asignación automática | Paso 4             |
| Modelo conceptual           | Soporte de datos   |
| Notificaciones              | Paso 12            |

---

# 11. Consideraciones Operativas

El proceso debe soportar alta concurrencia y permitir auditoría completa de cada transición de estado. Todas las acciones relevantes deben quedar registradas con usuario, fecha y hora.

En futuras versiones podrá incorporarse priorización automática y SLA por tipo de reclamo.

---

# 12. Control de Cambios

| Versión | Fecha      | Descripción                 |
| ------- | ---------- | --------------------------- |
| 1.0     | 2026-03-02 | Versión inicial del proceso |

---

**Fin del documento**
