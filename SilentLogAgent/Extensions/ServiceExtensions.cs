using SilentLogAgent.Alerts;
using SilentLogAgent.Models;
using SilentLogAgent.Services;
using System.Collections.Concurrent;

namespace SilentLogAgent.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<ExpectationMonitor>();
            services.AddSingleton<LogAnalyzer>();
            services.AddSingleton<LogTailService>();
            services.AddSingleton(new ConcurrentDictionary<string, LogExpectation>());
            services.AddSingleton<ISpecLoader, SpecLoader>();
            services.Configure<SlackOptions>(config.GetSection("Slack"));

            services.AddHttpClient<SlackAlertSink>();
            services.AddSingleton<IAlertSink>(sp =>
            {
                var sinks = new List<IAlertSink>
                {
                    sp.GetRequiredService<SlackAlertSink>(),
                    new PrometheusMetrics()
                };

                // Simple composite sink
                return new CompositeAlertSink(sinks);
            });
        }
    }
}
