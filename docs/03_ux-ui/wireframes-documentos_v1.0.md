# wireframes-documentos_v1.0.md

## 1. Introducción

Este documento describe los wireframes conceptuales de los documentos soportados por el motor de renderizado DSL. Su objetivo es representar visualmente (a nivel lógico) cómo se estructuran los elementos dentro de un documento antes de su renderización final en distintos formatos (ESC/POS, UI, texto plano, PDF futuro).

Los wireframes no representan estilos gráficos finales, sino la organización jerárquica y distribución de componentes.

---

## 2. Objetivo

Definir una representación simplificada de los documentos para:

- Visualizar la estructura jerárquica.
- Facilitar el diseño de plantillas DSL.
- Servir como guía para implementadores del motor.
- Asegurar consistencia entre distintos renderizadores.

---

## 3. Principios de diseño

- **Jerarquía clara**: cada documento se organiza en bloques anidados.
- **Independencia del renderizador**: el wireframe no depende del formato final.
- **Abstracción visual**: representa posiciones relativas, no píxeles exactos.
- **Modularidad**: cada sección del documento es reutilizable.
- **Adaptabilidad**: los bloques deben poder reordenarse según el perfil del dispositivo.

---

## 4. Estructura general de un documento

Un documento típico se compone de:

- Header (encabezado)
- Body (contenido principal)
- Footer (pie de página)

Wireframe lógico:
