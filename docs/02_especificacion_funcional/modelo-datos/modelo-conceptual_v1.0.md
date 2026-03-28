# Modelo de Datos Conceptual

**Proyecto:** Motor DSL de Generación de Documentos
**Documento:** modelo-conceptual_v1.0.md
**Versión:** 1.0
**Estado:** Aprobado
**Fecha:** 2026-03-28
**Autor:** Equipo Funcional / Arquitectura

---

# 1. Propósito

Este documento describe el modelo de datos conceptual del motor DSL para generación de documentos, identificando las entidades principales del dominio, sus atributos relevantes y las relaciones entre ellas.

El objetivo es establecer un entendimiento claro del sistema basado en tres pilares fundamentales:

* Datos del documento
* Plantilla (DSL)
* Perfil del dispositivo

Este modelo permite validar que toda la información necesaria para la generación, renderizado e impresión de documentos esté correctamente representada.

---

# 2. Alcance del Modelo

El modelo cubre las entidades necesarias para:

* representar datos de documentos (tickets, comprobantes, etc.)
* definir estructuras de plantillas DSL
* modelar perfiles de dispositivos de impresión
* soportar el proceso completo de renderizado

No incluye detalles físicos de almacenamiento ni optimizaciones técnicas.

---

# 3. Entidades Principales

## 3.1 DocumentoDatos

Representa el conjunto de datos en formato JSON que alimenta la generación del documento.

**Atributos principales**

* DocumentoId
* TipoDocumento
* ContenidoJSON
* FechaCreacion

**Ejemplo:**
Ticket con ítems, precios y total.

---

## 3.2 PlantillaDSL

Define la estructura del documento mediante un lenguaje declarativo (DSL).

**Atributos principales**

* PlantillaId
* Nombre
* Version
* EstructuraDSL
* Activa

**Ejemplo:**
Plantilla de ticket con header, body y footer.

---

## 3.3 NodoDSL

Representa cada elemento dentro de la plantilla DSL.

**Atributos principales**

* NodoId
* Tipo (text, image, qr, table, etc.)
* Propiedades
* NodoPadreId (opcional)

**Ejemplo:**
Elemento tipo "text" con valor dinámico.

---

## 3.4 PerfilDispositivo

Define las capacidades y restricciones del dispositivo de salida.

**Atributos principales**

* PerfilId
* Nombre
* TipoDispositivo
* AnchoMaximo
* SoportaImagen
* SoportaQR
* Configuracion

**Ejemplo:**
Impresora térmica ESC/POS de 58mm.

---

## 3.5 DocumentoAbstracto

Representa el documento procesado por el motor, independiente del formato final.

**Atributos principales**

* DocumentoAbstractoId
* EstructuraRenderizada
* Metadatos

**Ejemplo:**
Documento listo para ser interpretado por cualquier renderizador.

---

## 3.6 Renderizador

Define el mecanismo de transformación del documento abstracto a un formato final.

**Atributos principales**

* RenderizadorId
* TipoSalida (ESC/POS, PDF, UI, texto)
* Configuracion

**Ejemplo:**
Renderizador ESC/POS para impresora térmica.

---

## 3.7 OperacionImpresion

Representa la ejecución de una impresión.

**Atributos principales**

* OperacionId
* DocumentoId
* PlantillaId
* PerfilId
* FechaHora
* Resultado
* DetalleError

**Ejemplo:**
Impresión exitosa de ticket en dispositivo Bluetooth.

---

# 4. Relaciones

## 4.1 DocumentoDatos — PlantillaDSL

* Un documento se procesa con una plantilla.
* Una plantilla puede utilizarse para múltiples documentos.

**Cardinalidad:**
DocumentoDatos (N) —— (1) PlantillaDSL

---

## 4.2 PlantillaDSL — NodoDSL

* Una plantilla está compuesta por múltiples nodos.
* Cada nodo pertenece a una única plantilla.

**Cardinalidad:**
PlantillaDSL (1) —— (N) NodoDSL

---

## 4.3 NodoDSL — NodoDSL

* Un nodo puede tener hijos.
* Representa la estructura jerárquica.

**Cardinalidad:**
NodoDSL (1) —— (N) NodoDSL

---

## 4.4 DocumentoDatos — DocumentoAbstracto

* Un documento genera un documento abstracto.

**Cardinalidad:**
DocumentoDatos (1) —— (1) DocumentoAbstracto

---

## 4.5 DocumentoAbstracto — Renderizador

* Un documento abstracto puede ser procesado por múltiples renderizadores.

**Cardinalidad:**
DocumentoAbstracto (1) —— (N) Renderizador

---

## 4.6 PerfilDispositivo — Renderizador

* Un renderizador puede adaptarse a un perfil.

**Cardinalidad:**
PerfilDispositivo (1) —— (N) Renderizador

---

## 4.7 DocumentoAbstracto — OperacionImpresion

* Un documento puede imprimirse múltiples veces.

**Cardinalidad:**
DocumentoAbstracto (1) —— (N) OperacionImpresion

---

# 5. Reglas Conceptuales del Modelo

* RC-01: Todo documento debe tener datos válidos.
* RC-02: Toda plantilla debe cumplir el esquema DSL.
* RC-03: La estructura del documento debe ser jerárquica.
* RC-04: Los elementos deben ser soportados por el motor.
* RC-05: El documento abstracto es independiente del renderizador.
* RC-06: El perfil de dispositivo condiciona la salida final.

---

# 6. Consideraciones de Evolución

El modelo está diseñado para soportar:

* nuevos tipos de elementos DSL
* nuevos renderizadores (PDF, imagen, etc.)
* nuevos dispositivos

En futuras versiones se podrá incorporar:

* versionado de plantillas
* historial de impresiones
* caché de documentos renderizados

---

# 7. Trazabilidad

| Elemento del modelo | Soporta                              |
| ------------------- | ------------------------------------ |
| DocumentoDatos      | CU-18 Cargar datos                   |
| PlantillaDSL        | CU-13 Cargar plantilla               |
| NodoDSL             | CU-04 Motor de layout                |
| DocumentoAbstracto  | CU-05 Representación abstracta       |
| Renderizador        | CU-29 Extensión del motor            |
| PerfilDispositivo   | CU-11 Selección de perfil            |
| OperacionImpresion  | CU-10 Impresión + RN-04 Trazabilidad |

---

# 8. Control de Cambios

| Versión | Fecha      | Descripción                           |
| ------- | ---------- | ------------------------------------- |
| 1.0     | 2026-03-28 | Versión inicial del modelo conceptual |

---

**Fin del documento**
