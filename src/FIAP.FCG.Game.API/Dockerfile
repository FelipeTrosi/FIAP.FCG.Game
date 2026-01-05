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

# Instalar Datadog .NET Tracer 
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl ca-certificates \
    && curl -LO https://github.com/DataDog/dd-trace-dotnet/releases/download/v3.6.0/datadog-dotnet-apm_3.6.0_amd64.deb \
    && dpkg -i ./datadog-dotnet-apm_3.6.0_amd64.deb \
    && rm ./datadog-dotnet-apm_3.6.0_amd64.deb \
    && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

ENV CORECLR_ENABLE_PROFILING=1 \
    CORECLR_PROFILER="{846F5F1C-F9AE-4B07-969E-05C26BC060D8}" \
    CORECLR_PROFILER_PATH=/opt/datadog/Datadog.Trace.ClrProfiler.Native.so \
    DD_DOTNET_TRACER_HOME=/opt/datadog \
    DD_LOGS_INJECTION=true

ENTRYPOINT ["dotnet", "FIAP.FCG.Game.API.dll"]
