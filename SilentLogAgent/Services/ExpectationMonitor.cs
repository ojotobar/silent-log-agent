
using Microsoft.Extensions.Logging;
using SilentLogAgent.Alerts;
using SilentLogAgent.Models;
using System.Collections.Concurrent;

namespace SilentLogAgent.Services
{
    public class ExpectationMonitor
    {
        private readonly ILogger<ExpectationMonitor> _logger;
        private readonly IAlertSink _alertSink;
        private readonly ConcurrentDictionary<string, LogExpectation> _expectations;
        private readonly ConcurrentDictionary<string, DateTimeOffset> _lastSeenTimestamps = new();
        private readonly ConcurrentDictionary<string, DateTime> _lastAlertTimes;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

        public ExpectationMonitor(ILogger<ExpectationMonitor> logger, IAlertSink alertSink, ConcurrentDictionary<string, LogExpectation> expectations)
        {
            _logger = logger;
            _alertSink = alertSink;
            _expectations = expectations;
            _lastAlertTimes = new ConcurrentDictionary<string, DateTime>();
        }

        public async Task MonitorAsync(IEnumerable<LogExpectation> expectations, CancellationToken token)
        {
            _logger.LogInformation($"Silence monitoring started with {expectations.Count()} expectations");

            while (!token.IsCancellationRequested)
            {
                foreach (var exp in expectations)
                {
                    await CheckForSilenceAsync(exp);
                }

                await Task.Delay(TimeSpan.FromSeconds(30), token);
            }
        }

        public void UpdateLastSeen(LogExpectation expectation, DateTimeOffset timestamp)
        {
            _lastSeenTimestamps[expectation.Id] = timestamp;
            _logger.LogDebug("Updated last seen for '{ExpectationName}' to {Timestamp}", expectation.Name, timestamp);
        }

        public async Task CheckForSilenceAsync(LogExpectation expectation)
        {
            if (!_lastSeenTimestamps.TryGetValue(expectation.Id, out var lastSeen))
            {
                _logger.LogWarning("No log entry recorded yet for '{ExpectationName}'", expectation.Name);
                return;
            }

            var silenceDuration = DateTimeOffset.UtcNow - lastSeen;
            if (silenceDuration.TotalMinutes >= expectation.SilenceThresholdMinutes)
            {
                var message = $"🚨 Silence detected in log: **{expectation.Name}**\n" +
                              $"No matching log entry since {lastSeen:u} ({(int)silenceDuration.TotalMinutes} mins ago).";

                _logger.LogWarning(message);
                await _alertSink.SendAlertAsync(expectation, silenceDuration);
            }
        }
    }
}
