using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class DbCleanupWorkerService : BackgroundService
{
    private readonly ILogger<DbCleanupWorkerService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public DbCleanupWorkerService(
        ILogger<DbCleanupWorkerService> logger,
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.Log(LogLevel.Information, "DB cleanup started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.Log(LogLevel.Information, "Worker running at: {time}", DateTimeOffset.UtcNow);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var itemRepository = scope.ServiceProvider.GetRequiredService<IItemRepository>()
                        ?? throw new Exception("Service IItemRepository not found.");

                    int affectedRows = await itemRepository.DeleteOlderThan(DateTimeOffset.UtcNow);

                    _logger.Log(LogLevel.Information, "Succesfully deleted {rows} rows.", affectedRows);
                }
            }
            catch (ConfigException ex)
            {
                await StopAsync(stoppingToken);

                _logger.Log(LogLevel.Error, "DB cleanup error: {message}", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "DB cleanup error: {message}", ex.Message);
            }

            await Task.Delay(GetWorkerPeriod(), stoppingToken);
        }

        _logger.Log(LogLevel.Information, "DB cleanup ended.");
    }

    private int GetWorkerPeriod()
    {
        string timeString = _configuration["DbCleanupPeriodInSeconds"] 
            ?? throw new ConfigException("DbCleanup period not found.");

        if (!int.TryParse(timeString, out int result))
            throw new ConfigException("DbCleanup period must contain only numbers.");

        return 1 * result * 1000; // in miliseconds
    }
}
