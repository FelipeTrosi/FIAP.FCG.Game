using Serilog;
using Serilog.Events;

namespace FIAP.FCG.Game.API.Extensions;

public static class DatadogLoggerExtension
{
    public static void UseDatadogLoggerExtension(IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.DatadogLogs(
                apiKey: configuration["Datadog:ApiKey"],
                source: "dotnet-application",
                service: "fiap-fcg-api",
                host: "fiap-fcg-api",
                tags: new[] { "env:homolog", "version:1.0.0" }
            )
            .CreateLogger();
    }
}
