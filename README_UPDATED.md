# Tally (SoftWC)

Aplicación web ASP.NET Core MVC para gestión (login, usuarios, turnos, asistencia, exportes) utilizada por la empresa. Proyecto basado en .NET 9 y PostgreSQL por defecto (configurable).

## Contenido
- `Program.cs` — arranque y configuración (Identity, EF Core, servicios, Swagger).
- `Data/` — `ApplicationDbContext` y migraciones EF Core.
- `Controllers/`, `Views/`, `Models/`, `Service/`, `Utils/` — lógica de la aplicación.

## Requisitos
- .NET SDK 9.0 (dotnet 9)
- dotnet-ef (si vas a usar migraciones desde CLI). Puedes instalar/actualizar la herramienta globalmente:

```powershell
dotnet tool update --global dotnet-ef --version 7.0.3
```

Nota: No dejes credenciales sensibles (ej. passwords en `appsettings.json`) en repositorios públicos. Usa variables de entorno para producción.

## Configuración rápida
1. Clonar el repositorio y abrir la carpeta del proyecto (`SoftWC.csproj`).
2. Revisar la cadena de conexión en `appsettings.json` bajo `ConnectionStrings` — por defecto este proyecto apunta a `PostgreSQLConnection`.

Opciones comunes:
- Usar PostgreSQL (valor por defecto): configurar `ConnectionStrings:PostgreSQLConnection` con tus credenciales.
- Usar SQLite/local: el código contiene una alternativa comentada para SQLite en `Program.cs`; si prefieres SQLite, adapta la cadena de conexión y el código de inicialización.

Puedes proporcionar la cadena de conexión mediante variable de entorno (Windows PowerShell):

```powershell
# $env:ASPNETCORE_ENVIRONMENT = "Development"  # opcional
# $env:ConnectionStrings__PostgreSQLConnection = "Host=...;Database=...;Username=...;Password=..."

# O exportar directamente antes de ejecutar
setx ConnectionStrings__PostgreSQLConnection "Host=...;Database=...;Username=...;Password=..."
```

## Migraciones y seed (base de datos)
Si deseas aplicar migraciones desde la CLI:

```powershell
# Restaurar herramientas y paquetes
dotnet restore

# Crear/Aplicar migraciones (ej: desde la raíz del proyecto que contiene SoftWC.csproj)
dotnet ef migrations add NombreDeLaMigracion -o Data/Migrations --context ApplicationDbContext

dotnet ef database update --context ApplicationDbContext
```

El proyecto incluye inicializadores (`IdentityDataInitializer`, `UserDataInitializer`, `newRolesDataInitializer`) que pueden ejecutarse desde `Program.cs` (están preparados y comentados en el scope de creación de roles/usuarios). Activa las llamadas si quieres crear datos iniciales automáticamente.

## Ejecutar local (desarrollo)
Desde PowerShell, en la raíz del proyecto:

```powershell
# Restaurar dependencias
dotnet restore

# Build
dotnet build

# Ejecutar
dotnet run
```

Por defecto el proyecto usará la configuración de Kestrel y las URLs por defecto; si ejecutas con Docker (ve la sección Docker) el puerto expuesto es 8080.

## Ejecutar con Docker
El repositorio incluye un `Dockerfile` basado en .NET 9 y expone el puerto 8080.

```powershell
# Construir la imagen
docker build -t tally:latest .

# Ejecutar el contenedor (mapea puerto 8080)
docker run -d --name tally -p 8080:8080 --env ConnectionStrings__PostgreSQLConnection="Host=...;Database=...;Username=...;Password=..." tally:latest
```

Accede a la aplicación en http://localhost:8080 cuando ejecutes con Docker.

## Endpoints útiles
- Swagger UI: `/swagger` (generado por SwaggerGen en `Program.cs`).
- Página principal: `/` (controlador `HomeController`).

## Comandos útiles y generadores
- Generar CRUD a partir de una entidad (ejemplo):

```powershell
dotnet aspnet-codegenerator controller -name ClienteController -m Cliente -dc ApplicationDbContext --relativeFolderPath Controllers --useDefaultLayout --referenceScriptLibraries
```

- Generar vistas/Identity (ejemplo):

```powershell
dotnet aspnet-codegenerator identity -dc SoftWC.Data.ApplicationDbContext --files "Account.Register;Account.Login"
```

## Dependencias nativas
El proyecto referencia `DinkToPdf` que suele requerir librerías nativas de `libwkhtmltox` en algunas plataformas. Si usas funciones de PDF, revisa la documentación de DinkToPdf y añade la librería nativa apropiada para tu sistema.

## Estructura de carpetas (resumen)
- `Controllers/` — controladores MVC.
- `Views/` — vistas Razor.
- `Models/` — entidades y DTOs.
- `Data/` — `ApplicationDbContext`, migraciones.
- `Service/` — lógica de servicio y exportes (Excel/PDF).

## Notas finales
- Swagger está habilitado para explorar la API.
- Ajusta las opciones de Identity (cookies, expiración) en `Program.cs` según necesites.
- Recuerda no subir credenciales al repositorio; usa variables de entorno o secretos de tu CI/CD.

Si quieres, puedo:
- Añadir un script de PowerShell `run-local.ps1` que prepare variables de entorno y arranque la app.
- Añadir instrucciones para ejecutar con SQLite exactamente (modificando `Program.cs`) y crear un `appsettings.Local.json` de ejemplo.
