using Application.Interfaces;
using Application.Profiles;
using Application.Services;
using DbUp;
using Domain.Exceptions;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.OpenApi.Models;
using Npgsql;
using System.Data;
using WebAPI.Extensions;
using WebAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

string dbConnectonString = builder.Configuration.GetConnectionString("postgres")
    ?? throw new ConfigException("Connection string cannot be found.");

// Dapper connection
builder.Services.AddScoped<IDbConnection>((serviceProvider) => new NpgsqlConnection(dbConnectonString));

// DB injection
builder.Services.AddTransient<ErrorMiddleware>();
builder.Services.AddTransient<AuthMiddleware>();

builder.Services.AddScoped<IPairService, PairService>();
builder.Services.AddScoped<IPairRepository, PairRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHostedService<DbCleanupWorkerService>();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "CacheTask", Version = "v1" });

    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Name = "X-Api-Key",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Description = "API Key header",
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "ApiKey",
            },
        },
        new string[] { }
    },
    });
});

// Automapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// DbUp
var upgrader = DeployChanges.To
        .PostgresqlDatabase(builder.Configuration.GetConnectionString("postgres"))
        .WithScriptsEmbeddedInAssembly(typeof(PairRepository).Assembly)
        .LogToNowhere()
        .Build();

var result = upgrader.PerformUpgrade();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.UseErrorMiddleware();

// Uncomment when using API KEY authentication
//app.UseAuthMiddleware();

app.Run();
