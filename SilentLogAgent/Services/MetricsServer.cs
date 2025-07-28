using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Prometheus;

namespace SilentLogAgent.Services
{
    public class MetricsServer : IHostedService
    {
        private IHost? _metricsHost;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _metricsHost = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseKestrel(options =>
                {
                    options.ListenAnyIP(9091); // Expose metrics on port 9091
                });

                webBuilder.Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapMetrics(); // /metrics endpoint
                    });
                });
            })
            .Build();

            return _metricsHost.StartAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_metricsHost != null)
            {
                await _metricsHost.StopAsync(cancellationToken);
                _metricsHost.Dispose();
            }
        }
    }
}
