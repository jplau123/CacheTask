using DbUp;
using Domain.Exceptions;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data;

namespace Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IPairRepository, PairRepository>();
        services.AddHostedService<DbCleanupWorkerService>();

        string dbConnectonString = config.GetConnectionString("postgres")
                ?? throw new ConfigException("Connection string cannot be found.");

        services.AddScoped<IDbConnection>((serviceProvider) => new NpgsqlConnection(dbConnectonString));

        // DbUp
        EnsureDatabase.For.PostgresqlDatabase(dbConnectonString);
        var upgrader = DeployChanges.To
                .PostgresqlDatabase(dbConnectonString)
                .WithScriptsEmbeddedInAssembly(typeof(PairRepository).Assembly)
                .LogToNowhere()
                .Build();

        var result = upgrader.PerformUpgrade();
    }
}
