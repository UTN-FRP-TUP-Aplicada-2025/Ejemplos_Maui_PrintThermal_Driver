# 🏛️ Reclamos Ciudadanos

Sistema integral para la gestión digital de reclamos ciudadanos, construido con **.NET 10**, **ASP.NET Core REST API**, **.NET MAUI** y **SQL Server**.

El objetivo del proyecto es centralizar la recepción, clasificación automática y seguimiento de reclamos, proporcionando trazabilidad y eficiencia operativa para organismos municipales.

---

## 📌 Descripción

Reclamos Ciudadanos es una solución compuesta por:

- Backend REST para gestión de reclamos  
- Motor de reglas para asignación automática  
- Aplicación cliente en .NET MAUI  
- Base de datos relacional en SQL Server  
- Pipeline CI/CD automatizado  

El repositorio incluye documentación funcional, técnica y artefactos necesarios para construir y desplegar el sistema.

---

## 🎯 Objetivos del producto

- Digitalizar la recepción de reclamos  
- Automatizar la asignación de áreas  
- Permitir seguimiento del estado  
- Reducir tiempos de atención  
- Proveer trazabilidad completa  

---

## 🧱 Stack tecnológico

**Backend**

- .NET 10  
- ASP.NET Core Web API  
- Entity Framework Core  
- SQL Server  

**Cliente**

- .NET MAUI  

**DevOps**

- Git  
- Pipeline CI/CD  
- Containerización (opcional)

---

## 🏗️ Arquitectura (resumen)

El sistema adopta una arquitectura en capas:

- API REST desacoplada  
- Dominio con reglas de negocio  
- Persistencia mediante EF Core  
- Cliente móvil MAUI  
- Versionado semántico  

📄 Ver detalle:

```text
/docs/05_arquitectura_tecnica/arquitectura-solucion_v1.0.md
````

---

## 📂 Estructura del repositorio

```text
/docs
  00_contexto
  01_necesidades_negocio
  02_especificacion_funcional
  03_ux_ui
  04_prompts_ai
  05_arquitectura_tecnica
  06_backlog_tecnico
  07_plan_sprint
  08_calidad_y_pruebas
  09_devops

/src        (código fuente)
/tests      (pruebas automatizadas)
```

---

## 🚀 Cómo ejecutar el backend

### Prerrequisitos

* .NET 10 SDK
* SQL Server
* Visual Studio / VS Code

---

### 1. Clonar repositorio

```bash
git clone <repo-url>
cd Reclamos_Ciudadanos
```

---

### 2. Configurar variables de entorno

Ejemplo:

```bash
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=<cadena-sqlserver>
```

---

### 3. Ejecutar API

```bash
dotnet restore
dotnet build
dotnet run --project src/Reclamos.Api
```

API disponible en:

```text
http://localhost:5000
```

---

## 📱 Ejecutar cliente MAUI

```bash
dotnet build src/Reclamos.Maui
dotnet maui run
```

> Requiere workloads MAUI instalados.

---

## 🧪 Ejecutar pruebas

```bash
dotnet test
```

**Cobertura mínima requerida:** 70%

---

## 🔌 Endpoints principales

### Registrar reclamo

```http
POST /api/v1/reclamos
```

### Consultar estado

```http
GET /api/v1/reclamos/{id}
```

📄 Ver contratos completos:

```text
/docs/05_arquitectura_tecnica/contratos-api_v1.0.md
```

---

## 🔄 Flujo de desarrollo

1. Crear branch `feature/*`
2. Implementar
3. Agregar tests
4. Crear Pull Request
5. Pipeline CI en verde
6. Merge a main
7. Deploy automático a DEV

---

## ✅ Definition of Done

Una historia se considera completa cuando:

* Código compila
* Tests pasan
* Cobertura ≥ 70%
* PR aprobado
* Documentación actualizada

📄 Ver detalle:

```text
/docs/08_calidad_y_pruebas/definition-of-done_v1.0.md
```

---

## 🧭 Roadmap (alto nivel)

* v1.1 — mejoras de clasificación
* v1.2 — notificaciones
* v2.0 — funcionalidades avanzadas

---

## 🤝 Contribución

1. Fork del repositorio
2. Crear branch descriptivo
3. Seguir convenciones del proyecto
4. Asegurar tests en verde
5. Crear Pull Request

---

## 📜 Licencia

Uso interno / institucional.

---

## 📞 Contacto

Equipo de desarrollo — Proyecto Reclamos Ciudadanos.

---

```
