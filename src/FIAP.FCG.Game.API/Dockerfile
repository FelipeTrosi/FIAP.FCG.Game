# Etapa base: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 3002
ENV ASPNETCORE_HTTP_PORTS=3002

# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar os .csproj primeiro
COPY FIAP.FCG.Game.API/FIAP.FCG.Game.API.csproj FIAP.FCG.Game.API/
COPY FIAP.FCG.Game.Domain/FIAP.FCG.Game.Domain.csproj FIAP.FCG.Game.Domain/
COPY FIAP.FCG.Game.Infrastructure/FIAP.FCG.Game.Infrastructure.csproj FIAP.FCG.Game.Infrastructure/
COPY FIAP.FCG.Game.Service/FIAP.FCG.Game.Service.csproj FIAP.FCG.Game.Service/

# Restaurar dependências
RUN dotnet restore FIAP.FCG.Game.API/FIAP.FCG.Game.API.csproj
RUN dotnet restore FIAP.FCG.Game.Infrastructure/FIAP.FCG.Game.Infrastructure.csproj 

# Copiar o restante dos arquivos
COPY FIAP.FCG.Game.API/ FIAP.FCG.Game.API/
COPY FIAP.FCG.Game.Domain/ FIAP.FCG.Game.Domain/
COPY FIAP.FCG.Game.Infrastructure/ FIAP.FCG.Game.Infrastructure/
COPY FIAP.FCG.Game.Service/ FIAP.FCG.Game.Service/

# Build
WORKDIR /src/FIAP.FCG.Game.API
RUN dotnet build FIAP.FCG.Game.API.csproj -c Release -o /app/build

# Publish
FROM build AS publish
WORKDIR /src/FIAP.FCG.Game.API
RUN dotnet publish FIAP.FCG.Game.API.csproj -c Release -o /app/publish
FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "FIAP.FCG.Game.API.dll"]
