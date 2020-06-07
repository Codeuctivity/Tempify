using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Codeuctivity.Pages;

namespace Codeuctivity
{
  public class TimedHostedService : IHostedService, IDisposable
  {
    private readonly ILogger<TimedHostedService> _logger;
    private Timer _timer;

    public TimedHostedService(ILogger<TimedHostedService> logger)
    {
      _logger = logger;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
      _logger.LogInformation("Timed Hosted Service running.");

      _timer = new Timer(DoWork, null, TimeSpan.Zero,
          TimeSpan.FromMinutes(2));

      return Task.CompletedTask;
    }

    private void DoWork(object state)
    {
      var temperature = TemperReader.GetTemperature();
      if (temperature.Temperature.HasValue)
      {
        using (var db = new TemperDbContext())
        {
          db.MesureValues.Add(new MesureValue() { MesuareDateTime = DateTime.Now, Temperature = temperature.Temperature.Value });
          db.SaveChanges();
          _logger.LogInformation("Timed Hosted Service is working. Temperature: {temperature}", temperature.StringifiedResult);
        }
      }
      else
      {
        _logger.LogInformation("Timed Hosted Service failed.Error: {temperature}", temperature.StringifiedResult);
      }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
      _logger.LogInformation("Timed Hosted Service is stopping.");

      _timer?.Change(Timeout.Infinite, 0);

      return Task.CompletedTask;
    }

    public void Dispose() => _timer?.Dispose();
  }
}