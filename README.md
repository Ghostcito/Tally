**Proy empresa work and cleaning SAC**


Dotnet version9.0


1. antes de correr el programa actualizar tu herramienta de dotnet .net, tambien: ^ _ ^


```dotnet
dotnet tool update --global dotnet-ef --version 7.0.3
```


Comandos extra utilizados
- Comando para crear crud de acuerdo a entidad; -m entidad

```dotnet
dotnet aspnet-codegenerator controller -name DireccionesController -m Direccion -dc ApplicationDbContext --relativeFolderPath Controllers --useDefaultLayout --referenceScriptLibraries
```

- Creacion del proyecto:
```dotnet
App con Autentificacion y Autorizacion (Login)
dotnet new mvc --auth Individual
```

- Creacion de Migraciones y carpeta utilizada

```dotnet
dotnet ef migrations add InitialMigration --context appfunko.Data.ApplicationDbContext -o "C:\Users\Inteligo\Code\netcore\usmp\2023\appfunko\Data\Migrations"

//version corta: 
dotnet ef migrations add InitialCreate -o  Data/Migrations

dotnet ef database update -> crear bd actualmente local y en sqlite
```

- Generacion del Codigo Login y Registrar Cuenta de Identity para modificar vistas de aspidentity:
```dotnet
dotnet aspnet-codegenerator identity -dc appName.Data.ApplicationDbContext --files "Account.Register;Account.Login"

Generacion de CRUD
dotnet aspnet-codegenerator controller -name ClienteController -m Cliente -dc ApplicationDbContext --relativeFolderPath Controllers --useDefaultLayout --referenceScriptLibraries
```