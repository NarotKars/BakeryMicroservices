using Microsoft.VisualBasic;

public class TimedHostedService : IHostedService, IDisposable
{
    private int executionCount = 0;
    private readonly ILogger<TimedHostedService> _logger;
    private Timer UpdateTimer;
    private Timer PreCacheTimer;
    private readonly ProductsCacheWrapper cacheService;

    public TimedHostedService(ProductsCacheWrapper cacheService)
    {
        this.cacheService = cacheService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        PreCacheTimer = new Timer(async _ => await this.cacheService.PreCacheAsync(), null, TimeSpan.Zero, TimeSpan.FromMinutes(30));
        UpdateTimer = new Timer(async _ => await this.cacheService.UpdateCacheAsync(), null, TimeSpan.Zero, TimeSpan.FromMinutes(10));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        UpdateTimer?.Change(Timeout.Infinite, 0);
        PreCacheTimer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        var count = Interlocked.Increment(ref executionCount);

        _logger.LogInformation(
            "Timed Hosted Service is working. Count: {Count}", count);
    }

    public void Dispose()
    {
        UpdateTimer?.Dispose();
        PreCacheTimer?.Dispose();
    }
}