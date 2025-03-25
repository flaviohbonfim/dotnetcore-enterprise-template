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
   dotnet ef database update --project src/Ambev.DeveloperEvaluation.WebApi
   ```  

The API will be available at:  
- HTTP: [http://localhost:8080/swagger](http://localhost:8080/swagger)  
- HTTPS: [https://localhost:8081/swagger](https://localhost:8081/swagger)  

### Using Visual Studio 2022  
1. Open `Ambev.DeveloperEvaluation.sln`  
2. Set `Ambev.DeveloperEvaluation.WebApi` as the startup project  
3. Run migrations:  
   ```bash
   dotnet ef database update --project src/Ambev.DeveloperEvaluation.WebApi
   ```  
4. Press `F5` or click the "Run" button  

The API will be available at:  
- HTTP: [http://localhost:5119/swagger](http://localhost:5119/swagger)  
- HTTPS: [https://localhost:7181/swagger](https://localhost:7181/swagger)  

### Using .NET CLI  
1. Navigate to the WebAPI project directory:  
   ```bash
   cd src/Ambev.DeveloperEvaluation.WebApi
   ```
2. Run migrations:  
   ```bash
   dotnet ef database update --project ../Ambev.DeveloperEvaluation.WebApi
   ```  
3. Start the project:  
   ```bash
   dotnet run
   ```  

## Project Structure  
- `src/Ambev.DeveloperEvaluation.WebApi` : Main API project  
- `src/Ambev.DeveloperEvaluation.Application` : Application logic  
- `src/Ambev.DeveloperEvaluation.Domain` : Domain models and interfaces  
- `src/Ambev.DeveloperEvaluation.IoC` : Dependency injection configuration  
- `src/Ambev.DeveloperEvaluation.ORM` : Database access layer  

## Dependencies  
The project uses the following services (automatically configured in Docker):  

- **PostgreSQL Database**  
- **Kafka Broker**  

These services are configured in the `docker-compose.yml` file and will start automatically when using Docker Compose.  
