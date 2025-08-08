#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/DotNetCore.EnterpriseTemplate.WebApi/DotNetCore.EnterpriseTemplate.WebApi.csproj", "src/DotNetCore.EnterpriseTemplate.WebApi/"]
COPY ["src/DotNetCore.EnterpriseTemplate.IoC/DotNetCore.EnterpriseTemplate.IoC.csproj", "src/DotNetCore.EnterpriseTemplate.IoC/"]
COPY ["src/DotNetCore.EnterpriseTemplate.Domain/DotNetCore.EnterpriseTemplate.Domain.csproj", "src/DotNetCore.EnterpriseTemplate.Domain/"]
COPY ["src/DotNetCore.EnterpriseTemplate.Common/DotNetCore.EnterpriseTemplate.Common.csproj", "src/DotNetCore.EnterpriseTemplate.Common/"]
COPY ["src/DotNetCore.EnterpriseTemplate.Application/DotNetCore.EnterpriseTemplate.Application.csproj", "src/DotNetCore.EnterpriseTemplate.Application/"]
COPY ["src/DotNetCore.EnterpriseTemplate.ORM/DotNetCore.EnterpriseTemplate.ORM.csproj", "src/DotNetCore.EnterpriseTemplate.ORM/"]
RUN dotnet restore "./src/DotNetCore.EnterpriseTemplate.WebApi/DotNetCore.EnterpriseTemplate.WebApi.csproj"
COPY . .
WORKDIR "/src/src/DotNetCore.EnterpriseTemplate.WebApi"
RUN dotnet build "./DotNetCore.EnterpriseTemplate.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DotNetCore.EnterpriseTemplate.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DotNetCore.EnterpriseTemplate.WebApi.dll"]