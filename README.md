# Developer Evaluation Project  

## Prerequisites  
- .NET 8.0 SDK  
- Docker Desktop  
- Visual Studio 2022 or VS Code  

## Running the Project  

### Using Docker Compose (Recommended)  
1. Open a terminal in the root directory and run:  
   ```bash
   docker-compose up -d
   ```
2. Wait for the services to start.  
3. Run migrations to ensure the database is up to date:  
   ```bash
   dotnet ef database update --project src/DotNetCore.EnterpriseTemplate.WebApi
   ```  

The API will be available at:  
- HTTP: [http://localhost:8080/swagger](http://localhost:8080/swagger)  
- HTTPS: [https://localhost:8081/swagger](https://localhost:8081/swagger)  

### Using Visual Studio 2022  
1. Open `DotNetCore.EnterpriseTemplate.sln`  
2. Set `DotNetCore.EnterpriseTemplate.WebApi` as the startup project  
3. Run migrations:  
   ```bash
   dotnet ef database update --project src/DotNetCore.EnterpriseTemplate.WebApi
   ```  
4. Press `F5` or click the "Run" button  

The API will be available at:  
- HTTP: [http://localhost:5119/swagger](http://localhost:5119/swagger)  
- HTTPS: [https://localhost:7181/swagger](https://localhost:7181/swagger)  

### Using .NET CLI  
1. Navigate to the WebAPI project directory:  
   ```bash
   cd src/DotNetCore.EnterpriseTemplate.WebApi
   ```
2. Run migrations:  
   ```bash
   dotnet ef database update --project ../DotNetCore.EnterpriseTemplate.WebApi
   ```  
3. Start the project:  
   ```bash
   dotnet run
   ```  

## Project Structure  
- `src/DotNetCore.EnterpriseTemplate.WebApi` : Main API project  
- `src/DotNetCore.EnterpriseTemplate.Application` : Application logic  
- `src/DotNetCore.EnterpriseTemplate.Domain` : Domain models and interfaces  
- `src/DotNetCore.EnterpriseTemplate.IoC` : Dependency injection configuration  
- `src/DotNetCore.EnterpriseTemplate.ORM` : Database access layer  

## Dependencies  
The project uses the following services (automatically configured in Docker):  

- **PostgreSQL Database**  
- **Kafka Broker**  

These services are configured in the `docker-compose.yml` file and will start automatically when using Docker Compose.  
