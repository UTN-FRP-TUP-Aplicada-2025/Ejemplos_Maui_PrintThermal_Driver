
# criterios-validacion-motor_v1.0.md

## 1. Propósito

Definir los criterios que el motor DSL debe cumplir para validar la correcta ejecución de plantillas, datos, perfiles de dispositivo y reglas de negocio asociadas al proceso de generación de documentos.  

Este documento establece las condiciones que determinan si una ejecución es válida o debe ser rechazada, advertida o registrada como error.

---

## 2. Alcance

Aplica a:

- Validación de plantillas DSL.
- Validación de datos de entrada.
- Validación de perfiles de impresora/dispositivo.
- Validación de estructura del documento.
- Validación durante el procesamiento (runtime).
- Validaciones previas y posteriores a la renderización.

No incluye:
- Implementación interna del motor.
- Detalles de transporte (API, UI, etc.), salvo donde impacten en validaciones.

---

## 3. Principios de Validación

- **Fail-fast**: errores críticos deben detectarse lo antes posible.
- **Determinismo**: la validación debe producir siempre los mismos resultados para los mismos inputs.
- **Separación de responsabilidades**: validaciones de esquema, datos y ejecución deben estar desacopladas.
- **Extensibilidad**: nuevas reglas deben poder incorporarse sin modificar el núcleo del motor.
- **Trazabilidad**: toda validación debe poder registrarse y auditarse.

---

## 4. Tipos de Validación

### 4.1 Validación de Esquema

Verifica que la plantilla DSL cumple con:

- Sintaxis válida.
- Estructura jerárquica correcta.
- Tipos de nodos soportados.
- Atributos obligatorios presentes.
- Relaciones válidas entre elementos.

**Criterio:**
- Si el esquema es inválido → la ejecución se detiene.

---

### 4.2 Validación de Datos

Verifica que los datos de entrada:

- Cumplen con los tipos esperados.
- Contienen los campos requeridos.
- No presentan valores nulos en campos obligatorios.
- Respetan formatos definidos (fechas, números, strings, etc.).

**Criterio:**
- Datos inválidos pueden:
  - Detener ejecución (errores críticos).
  - Generar advertencias (según configuración).

---

### 4.3 Validación de Referencias

Verifica:

- Referencias a datos existentes en el contexto.
- Accesos a propiedades válidas.
- Expresiones evaluables correctamente.

**Criterio:**
- Referencias no resueltas deben tratarse como error o fallback configurable.

---

### 4.4 Validación de Estructura Jerárquica

Verifica que:

- El documento mantiene una estructura en árbol válida.
- No existen ciclos en la jerarquía.
- Los nodos hijos son compatibles con sus padres.

**Criterio:**
- Estructura inválida → rechazo de plantilla.

---

### 4.5 Validación de Reglas de Renderización

Verifica:

- Compatibilidad de elementos con el renderizador activo.
- Soporte de estilos, layout y componentes.
- Restricciones del perfil de dispositivo.

**Criterio:**
- Elementos no soportados pueden:
  - Ser omitidos.
  - Generar error.
  - Ser reemplazados por fallback (según configuración).

---

### 4.6 Validación de Condiciones

Verifica:

- Evaluación correcta de expresiones condicionales.
- Tipos de retorno booleanos.
- Expresiones seguras (sin errores en runtime).

**Criterio:**
- Condiciones inválidas → error de evaluación.

---

### 4.7 Validación de Iteraciones

Verifica:

- Que las colecciones existen.
- Que son iterables.
- Que no generan estructuras inconsistentes.
- Que no hay iteraciones infinitas.

**Criterio:**
- Colecciones inválidas → error o iteración vacía según configuración.

---

### 4.8 Validación de Perfil de Dispositivo

Verifica:

- Compatibilidad entre plantilla y dispositivo.
- Restricciones de tamaño, resolución, encoding.
- Capacidades del renderizador (ESC/POS, UI, etc.).

**Criterio:**
- Si el perfil no cumple → se rechaza o adapta la salida.

---

## 5. Niveles de Severidad

Las validaciones pueden clasificarse en:

| Nivel        | Descripción |
|-------------|------------|
| ERROR       | Impide la ejecución |
| WARNING     | No detiene ejecución, pero se registra |
| INFO        | Informativo, sin impacto en ejecución |

---

## 6. Reglas de Validación Globales

- Toda plantilla debe validarse antes de ejecutarse.
- Todo dataset debe validarse antes del mapeo.
- Toda ejecución debe validar compatibilidad con el perfil de dispositivo.
- Toda expresión debe validarse antes de evaluarse.
- Toda referencia debe resolverse en tiempo de ejecución.

---

## 7. Estrategia de Validación

- Validación en capas:
  1. Validación de esquema (compile-time).
  2. Validación de datos (pre-ejecución).
  3. Validación de ejecución (runtime).
  4. Validación de renderizado (post-mapeo).

- Validación incremental:
  - Se valida por etapas durante el pipeline del motor.

---

## 8. Manejo de Errores de Validación

- Registro de errores con:
  - Código de error
  - Mensaje descriptivo
  - Ubicación en la plantilla
  - Contexto de datos

- Opciones configurables:
  - Detener ejecución
  - Continuar con fallback
  - Omitir nodo
  - Reintentar resolución

---

## 9. Validaciones Obligatorias

El motor debe validar obligatoriamente:

- Existencia de plantilla válida.
- Estructura jerárquica.
- Tipos de nodos soportados.
- Datos requeridos.
- Referencias resueltas.
- Expresiones evaluables.
- Compatibilidad con perfil de dispositivo.

---

## 10. Validaciones Opcionales

Dependiendo de configuración:

- Validación estricta de tipos.
- Validación de campos adicionales.
- Validación de performance (tamaño de documento, profundidad).
- Validación de restricciones de layout.

---

## 11. Trazabilidad de Validación

Cada validación debe registrar:

- Timestamp
- Identificador de ejecución
- Tipo de validación
- Resultado (OK / ERROR / WARNING)
- Detalle del error (si aplica)

---

## 12. Versionado

Este documento corresponde a la versión:

- **criterios-validacion-motor_v1.0**

Cambios futuros deberán:

- Mantener compatibilidad hacia atrás cuando sea posible.
- Documentar nuevas reglas de validación.
- Indicar impacto en plantillas existentes.

---
