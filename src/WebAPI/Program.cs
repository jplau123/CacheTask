using Application.Profiles;
using Npgsql;
using System.Data;
using WebAPI.Extensions;
using WebAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

string dbConnectonString = builder.Configuration.GetConnectionString("postgres")
    ?? throw new ArgumentException("Connection string cannot be found.");

// Dapper connection
builder.Services.AddScoped<IDbConnection>((serviceProvider) => new NpgsqlConnection(dbConnectonString));

builder.Services.AddTransient<ErrorMiddleware>();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseErrorMiddleware();

app.Run();
