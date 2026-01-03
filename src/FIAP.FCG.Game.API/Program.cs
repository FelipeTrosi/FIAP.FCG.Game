using Elastic.Clients.Elasticsearch;
using FIAP.FCG.Game.API.Extensions;
using FIAP.FCG.Game.Infrastructure.Repository;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

#region -- Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Fiap Cloud Games",
        Version = "v1",
        Description = "API criada para validação dos entregaveis do curso POS-TECH Arquitetura de Sistemas .NET com Azure"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Digite o token JWT sem 'Bearer '"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    var serviceXml = Path.Combine(AppContext.BaseDirectory, "FIAP.FCG.Game.Service.xml");
    if (File.Exists(serviceXml))
        c.IncludeXmlComments(serviceXml);
});

#endregion

#region -- Data

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
}, ServiceLifetime.Scoped);

#endregion

#region -- JWT

builder.Services.UseJwtAuthentication(builder.Configuration);

#endregion

#region -- DI

builder.Services.AddCorrelationIdGenerator();
builder.Services.AddRepositoryDI();
builder.Services.AddServiceDI();

#endregion

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

#region -- Elasticsearch
builder.Services.AddSingleton(sp =>
{
    var settings = new ElasticsearchClientSettings(new Uri(configuration["Elasticsearch:Url"]!))
        .DefaultIndex("games");

    return new ElasticsearchClient(settings);
});
#endregion

builder.Services.AddHealthChecks();

#region -- Datadog
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

builder.Host.UseSerilog();
#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#region -- Middleware

app.UseCorrelationMiddleware();
app.UseLogMiddleware();
app.UseGlobalExceptionHandler();

#endregion

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();
