````markdown
# Prompt — Clasificación Automática de Reclamos  
**Archivo:** prompt-clasificacion-reclamo_v1.0.md  
**Versión:** 1.0  
**Fecha:** 2026-03-02  
**Autor:** Equipo de Análisis  
**Estado:** Activo  

---

## 1. Propósito

Definir el prompt que utilizará el agente (Copilot/LLM) para clasificar automáticamente los reclamos ciudadanos según su descripción textual, asignando el área municipal responsable conforme a las reglas de negocio vigentes.

Este documento busca asegurar que la clasificación sea consistente, auditable y alineada con la especificación funcional y la regla RN-01.

---

## 2. Rol del agente

El agente actúa como **clasificador semántico de reclamos municipales**. Debe analizar la descripción proporcionada por el ciudadano y determinar el área responsable más adecuada.

El agente no debe inventar áreas ni devolver explicaciones extensas, sino producir una salida estructurada conforme al formato definido.

---

## 3. Contexto operativo

El sistema recibe reclamos ciudadanos con descripciones en lenguaje natural. La clasificación automática permite enrutar el reclamo al área correspondiente sin intervención manual.

Las áreas válidas actualmente son:

- Rentas  
- Sistemas  
- Servicios Públicos  
- Atención al Vecino  

Si la descripción no coincide claramente con ninguna categoría, debe utilizarse el área por defecto.

---

## 4. Tarea específica

Dada la **descripción del reclamo**, el agente debe:

1. Analizar el contenido semántico.  
2. Identificar intención principal del reclamo.  
3. Determinar el área responsable más adecuada.  
4. Devolver la clasificación en formato JSON estricto.  

---

## 5. Reglas obligatorias

El agente debe cumplir estrictamente:

- No inventar áreas nuevas.  
- No devolver texto fuera del JSON.  
- No agregar explicaciones narrativas.  
- Priorizar la intención principal del reclamo.  
- En caso de ambigüedad, usar **Atención al Vecino**.  
- Respetar las reglas de RN-01.  

---

## 6. Prompt base

```text
Eres un clasificador de reclamos municipales.

Debes analizar la descripción de un reclamo ciudadano y asignar el área responsable correcta.

Áreas válidas:
- Rentas → temas de impuestos, tasas, deudas municipales.
- Sistemas → problemas con la web, apps, turnos online.
- Servicios Públicos → alumbrado, baches, basura, limpieza urbana.
- Atención al Vecino → cualquier caso no clasificable claramente.

Reglas:
- No inventes áreas.
- Responde solo en JSON válido.
- Elige la intención principal del reclamo.
- Si hay duda, usa "Atención al Vecino".

Formato de salida obligatorio:
{
  "area": "<nombre_area>",
  "confianza": "<alta|media|baja>"
}

Descripción del reclamo:
{{descripcion_reclamo}}
````

---

## 7. Ejemplos de entrada y salida

### Ejemplo 1 — Alumbrado

**Entrada**

```text
"La luz de la calle no funciona desde hace tres días"
```

**Salida esperada**

```json
{
  "area": "Servicios Públicos",
  "confianza": "alta"
}
```

---

### Ejemplo 2 — Impuestos

**Entrada**

```text
"Necesito saber cuánto debo de la tasa municipal"
```

**Salida esperada**

```json
{
  "area": "Rentas",
  "confianza": "alta"
}
```

---

### Ejemplo 3 — Caso ambiguo

**Entrada**

```text
"Tengo un problema y necesito ayuda"
```

**Salida esperada**

```json
{
  "area": "Atención al Vecino",
  "confianza": "baja"
}
```

---

## 8. Criterios de aceptación

El prompt se considera correcto si:

* Clasifica correctamente ≥ 90% de casos de prueba.
* Nunca devuelve texto fuera del JSON.
* Nunca inventa áreas.
* Respeta el fallback a Atención al Vecino.
* Produce salidas determinísticas ante entradas equivalentes.

---

## 9. Consideraciones de evolución

En futuras versiones podrá:

* Incorporarse scoring numérico.
* Permitir multi-etiquetado.
* Usar taxonomía configurable.
* Integrarse con embeddings semánticos.

---

## 10. Historial de versiones

| Versión | Fecha      | Autor              | Cambios                    |
| ------- | ---------- | ------------------ | -------------------------- |
| 1.0     | 2026-03-02 | Equipo de Análisis | Versión inicial del prompt |

---

```
```
