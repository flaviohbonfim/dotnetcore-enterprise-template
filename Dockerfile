# Base image para runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Imagem para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os projetos para restaurar as dependências
COPY ["src/DotNetCore.EnterpriseTemplate.WebApi/DotNetCore.EnterpriseTemplate.WebApi.csproj", "src/DotNetCore.EnterpriseTemplate.WebApi/"]
COPY ["src/DotNetCore.EnterpriseTemplate.Application/DotNetCore.EnterpriseTemplate.Application.csproj", "src/DotNetCore.EnterpriseTemplate.Application/"]
COPY ["src/DotNetCore.EnterpriseTemplate.Common/DotNetCore.EnterpriseTemplate.Common.csproj", "src/DotNetCore.EnterpriseTemplate.Common/"]
COPY ["src/DotNetCore.EnterpriseTemplate.Domain/DotNetCore.EnterpriseTemplate.Domain.csproj", "src/DotNetCore.EnterpriseTemplate.Domain/"]
COPY ["src/DotNetCore.EnterpriseTemplate.IoC/DotNetCore.EnterpriseTemplate.IoC.csproj", "src/DotNetCore.EnterpriseTemplate.IoC/"]
COPY ["src/DotNetCore.EnterpriseTemplate.ORM/DotNetCore.EnterpriseTemplate.ORM.csproj", "src/DotNetCore.EnterpriseTemplate.ORM/"]
RUN dotnet restore "src/DotNetCore.EnterpriseTemplate.WebApi/DotNetCore.EnterpriseTemplate.WebApi.csproj"

# Copia todo o código-fonte
COPY . .

# Compila o projeto
WORKDIR "/src/src/DotNetCore.EnterpriseTemplate.WebApi"
RUN dotnet build "DotNetCore.EnterpriseTemplate.WebApi.csproj" --configuration Release --output /app/build

# Publica o projeto
FROM build AS publish
RUN dotnet publish "DotNetCore.EnterpriseTemplate.WebApi.csproj" --configuration Release --output /app/publish /p:UseAppHost=false

# Configuração final para rodar a aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Define variáveis do ASP.NET Core
ENV ASPNETCORE_URLS=http://+:8080

# Define o entrypoint padrão como a API
ENTRYPOINT ["dotnet", "DotNetCore.EnterpriseTemplate.WebApi.dll"]