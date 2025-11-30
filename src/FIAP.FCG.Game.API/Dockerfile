# Etapa base: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 3000
ENV ASPNETCORE_HTTP_PORTS=3000

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
    && mkdir -p /opt/datadog /var/log/datadog \
    && TRACER_VERSION=$(curl -s https://api.github.com/repos/DataDog/dd-trace-dotnet/releases/latest | grep tag_name | cut -d '"' -f 4 | cut -c2-) \
    && curl -LO https://github.com/DataDog/dd-trace-dotnet/releases/download/v${TRACER_VERSION}/datadog-dotnet-apm_${TRACER_VERSION}_amd64.deb \
    && dpkg -i ./datadog-dotnet-apm_${TRACER_VERSION}_amd64.deb \
    && rm ./datadog-dotnet-apm_${TRACER_VERSION}_amd64.deb \
    && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Development

ENV CORECLR_ENABLE_PROFILING=1 \
    CORECLR_PROFILER="{846F5F1C-F9AE-4B07-969E-05C26BC060D8}" \
    CORECLR_PROFILER_PATH=/opt/datadog/Datadog.Trace.ClrProfiler.Native.so \
    DD_DOTNET_TRACER_HOME=/opt/datadog \
    DD_SERVICE=fiap-fcg-api \
    DD_ENV=staging \
    DD_VERSION=1.0.0 \
    DD_AGENT_HOST=dd-agent \
    DD_TRACE_DEBUG=false \
    DD_DATA_STREAMS_ENABLED=true \
    DD_TRACE_REMOVE_INTEGRATION_SERVICE_NAMES_ENABLED=true \
    DD_LOGS_ENABLED=true \
    DD_LOGS_CONFIG_CONTAINER_COLLECT_ALL=true \
    DD_CONTAINER_EXCLUDE="name:datadog-agent" \
    DD_LOGS_INJECTION=true \
    DD_APPSEC_ENABLED=true \
    DD_IAST_ENABLED=true \
    DD_APPSEC_SCA_ENABLED=true 

ENTRYPOINT ["dotnet", "FIAP.FCG.Game.API.dll"]
