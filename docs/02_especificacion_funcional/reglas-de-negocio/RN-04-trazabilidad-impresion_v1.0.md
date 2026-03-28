# Regla de Negocio: Trazabilidad de Impresión

**Código:** RN-04
**Archivo:** RN-04-trazabilidad-impresion_v1.0.md
**Versión:** 1.0
**Estado:** Aprobada
**Fecha:** 2026-03-28
**Autor:** Equipo Funcional / Arquitectura

---

# 1. Descripción

Esta regla de negocio establece que toda operación de impresión realizada por el sistema debe registrarse de forma trazable, permitiendo reconstruir el evento completo desde la generación del documento hasta su envío al dispositivo.

El objetivo es garantizar auditabilidad, diagnóstico de errores y control operativo del proceso de impresión.

---

# 2. Motivación de Negocio

En entornos donde se imprimen tickets, comprobantes o documentos operativos, es fundamental poder responder preguntas como:

* ¿Qué se imprimió?
* ¿Cuándo se imprimió?
* ¿En qué dispositivo?
* ¿Con qué datos?
* ¿El proceso fue exitoso o falló?

La trazabilidad permite:

* auditar operaciones
* detectar fallos
* reimprimir documentos
* mejorar la confiabilidad del sistema

---

# 3. Definición de la Regla

El sistema debe registrar cada operación de impresión incluyendo información suficiente para su seguimiento, auditoría y posible reejecución.

El registro debe contemplar:

* identificación del documento
* plantilla utilizada
* datos procesados
* perfil de dispositivo
* resultado de la impresión
* fecha y hora
* errores (si existieron)

---

# 4. Modelo de Trazabilidad (Ejemplo)

| Campo               | Descripción                      |
| ------------------- | -------------------------------- |
| IdOperacion         | Identificador único              |
| DocumentoId         | Identificador del documento      |
| PlantillaId         | Plantilla DSL utilizada          |
| PerfilDispositivoId | Perfil de impresora              |
| FechaHora           | Momento de ejecución             |
| Resultado           | Éxito / Error                    |
| DetalleError        | Información de error (si aplica) |
| HashDocumento       | Huella del contenido generado    |

---

# 5. Condiciones de Aplicación

La regla se ejecuta en los siguientes momentos:

* Al enviar un documento a impresión (CU-10)
* Durante la ejecución de impresión
* Al finalizar el proceso (éxito o fallo)

Debe aplicarse a todas las operaciones de impresión.

---

# 6. Resultados Esperados

* Toda impresión queda registrada.
* Se puede auditar cualquier operación.
* Se facilita el diagnóstico de errores.
* Se habilita la posibilidad de reimpresión.

---

# 7. Excepciones

## EX-01 Modo sin persistencia

Si el sistema opera en modo debug o liviano:

* Puede no persistir la trazabilidad.
* Debe al menos registrarse en logs temporales.

---

## EX-02 Error en registro

Si falla el almacenamiento de trazabilidad:

* El sistema debe continuar la operación de impresión.
* Debe registrar el error técnico.

---

# 8. Criterios de Validación

## CV-01 Registro de impresión

**Dado** una impresión realizada
**Cuando** finaliza
**Entonces** existe un registro asociado.

---

## CV-02 Registro de error

**Dado** una impresión fallida
**Cuando** ocurre el error
**Entonces** se registra con detalle.

---

## CV-03 Integridad de datos

**Dado** un registro de trazabilidad
**Cuando** se consulta
**Entonces** contiene todos los datos relevantes.

---

## CV-04 Identificación única

**Dado** múltiples impresiones
**Cuando** se registran
**Entonces** cada una tiene un identificador único.

---

# 9. Impacto

Esta regla impacta directamente en:

* CU-10 Enviar documento a impresora Bluetooth
* CU-32 Manejar errores de impresión
* Procesos de auditoría
* Sistemas de monitoreo

La trazabilidad es clave para la operación en producción del sistema.

---

# 10. Control de Cambios

| Versión | Fecha      | Descripción     |
| ------- | ---------- | --------------- |
| 1.0     | 2026-03-28 | Versión inicial |

---

**Fin del documento**
