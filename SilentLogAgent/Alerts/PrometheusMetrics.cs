using Prometheus;
using SilentLogAgent.Models;

namespace SilentLogAgent.Alerts
{
    public class PrometheusMetrics : IAlertSink
    {
        private static readonly Counter SilenceBreachCounter = Metrics.CreateCounter("log_silence_breaches_total", "Counts the number of log silence breaches",
            new CounterConfiguration
            {
                LabelNames = new[] { "expectation", "critical" }
            });

        public async Task SendAlertAsync(LogExpectation expectation, TimeSpan silenceDuration)
        {
            SilenceBreachCounter
                .WithLabels(expectation.Name, expectation.IsCritical.ToString().ToLower())
                .Inc();

            await Task.CompletedTask;
        }
    }
}
