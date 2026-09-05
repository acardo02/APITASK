# APITask

API REST de tareas desarrollada con ASP.NET Core, Entity Framework Core y SQL Server.

## Ejecución local

### Requisitos

- .NET SDK 10
- SQL Server

### Configuración

1. Restaura las dependencias:

   ```powershell
   dotnet restore
   ```

2. Configura la conexión a SQL Server mediante User Secrets:

   ```powershell
   dotnet user-secrets set `
     "ConnectionStrings:DefaultConnection" `
     "Server=localhost;Database=APITaskDb;Trusted_Connection=True;TrustServerCertificate=True" `
     --project .\APITask\APITask.csproj
   ```

   Para autenticación con usuario y contraseña utiliza:

   ```powershell
   dotnet user-secrets set `
     "ConnectionStrings:DefaultConnection" `
     "Server=localhost,1433;Database=APITaskDb;User Id=sa;Password=TU_PASSWORD;TrustServerCertificate=True" `
     --project .\APITask\APITask.csproj
   ```

3. Instala la herramienta de Entity Framework Core si aún no está disponible:

   ```powershell
   dotnet tool install --global dotnet-ef --version 10.*
   ```

4. Crea la base de datos y aplica las migraciones:

   ```powershell
   dotnet ef database update `
     --project .\APITask\APITask.csproj `
     --startup-project .\APITask\APITask.csproj
   ```

5. Ejecuta la API:

   ```powershell
   dotnet run --project .\APITask\APITask.csproj --launch-profile http
   ```

6. Abre Swagger:

   ```text
   http://localhost:5158/swagger
   ```

## Pruebas

Ejecuta todas las pruebas:

```powershell
dotnet test .\APITask.Tests\APITask.Tests.csproj
```

Ejecuta solamente las pruebas del controlador:

```powershell
dotnet test .\APITask.Tests\APITask.Tests.csproj `
  --filter "FullyQualifiedName~AssignmentControllerTests"
```

## Ejecución con Docker

### Requisitos

- Docker Desktop

### Configuración

1. Crea un archivo `.env` en la raíz del repositorio:

   ```env
   SQL_SA_PASSWORD=TU_PASSWORD_SEGURO
   ```

   La contraseña debe contener mayúsculas, minúsculas, números y símbolos.

2. Construye e inicia la API y SQL Server:

   ```powershell
   docker compose up --build -d
   ```

   Las migraciones se aplican automáticamente cuando inicia la API.

3. Comprueba el estado de los contenedores:

   ```powershell
   docker compose ps
   ```

4. Consulta los registros de la API:

   ```powershell
   docker compose logs api
   ```

5. Abre Swagger:

   ```text
   http://localhost:8080/swagger
   ```

6. Detén los contenedores:

   ```powershell
   docker compose down
   ```

Para eliminar también la base de datos almacenada en el volumen:

```powershell
docker compose down --volumes
```

## Nuevas migraciones

Después de modificar las entidades o su configuración, crea una migración:

```powershell
dotnet ef migrations add NombreDeLaMigracion `
  --project .\APITask\APITask.csproj `
  --startup-project .\APITask\APITask.csproj
```

En ejecución local, aplícala con:

```powershell
dotnet ef database update `
  --project .\APITask\APITask.csproj `
  --startup-project .\APITask\APITask.csproj
```

Con Docker, reconstruye la API para incluirla:

```powershell
docker compose up --build -d
```
