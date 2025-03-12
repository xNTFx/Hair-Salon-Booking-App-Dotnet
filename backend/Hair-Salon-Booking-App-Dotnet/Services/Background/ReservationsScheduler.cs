using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

public class ReservationsScheduler : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ReservationsScheduler(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var reservationsService = scope.ServiceProvider.GetRequiredService<IReservationsService>();

                await reservationsService.MarkReservationsAsCompletedAsync();
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // 1 Hour
        }
    }
}
