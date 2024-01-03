using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class DbCleanupWorkerService : BackgroundService
{
    private readonly int _workerPeriod = 1 * 10 * 1000;
    private readonly ILogger<DbCleanupWorkerService> _logger;
    private readonly IServiceProvider _serviceProvider;


    public DbCleanupWorkerService(
        ILogger<DbCleanupWorkerService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
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

                    var itemRepository = scope.ServiceProvider.GetRequiredService<IItemRepository>();

                    int affectedRows = await itemRepository.DeleteOlderThan(DateTimeOffset.UtcNow);

                    _logger.Log(LogLevel.Information, "Succesfully deleted {rows} rows.", affectedRows);
                }

                await Task.Delay(_workerPeriod);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "DB cleanup error: {message}", ex.Message);
            }
        }

        _logger.Log(LogLevel.Information, "DB cleanup ended.");
    }
}
