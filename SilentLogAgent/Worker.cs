using SilentLogAgent.Models;
using SilentLogAgent.Services;
using System.Threading;

namespace SilentLogAgent
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ISpecLoader _specLoader;
        private readonly LogTailService _logTailService;
        private readonly ExpectationMonitor _monitor;

        public Worker(ILogger<Worker> logger, ISpecLoader specLoader, LogTailService 
            logTailService, ExpectationMonitor monitor)
        {
            _logger = logger;
            _specLoader = specLoader;
            _logTailService = logTailService;
            _monitor = monitor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Silent Log Agent starting...");

            // Load expectations from spec file
            List<LogExpectation> expectations;
            try
            {
                expectations = _specLoader.GetExpectations().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Failed to load expectations from spec.json.");
                return;
            }

            if (expectations.Count == 0)
            {
                _logger.LogWarning("No expectations found. Agent will not run.");
                return;
            }

            _logger.LogInformation("Loaded {Count} expectation(s).", expectations.Count);

            // Start tailing all files concurrently
            var tailingTasks = expectations.Select(exp =>
                _logTailService.TailFileAsync(exp, stoppingToken)
            ).ToList();

            // Start monitoring silence in background
            var monitorTask = _monitor.MonitorAsync(expectations, stoppingToken);

            // Wait for all tailing tasks and monitor task
            await Task.WhenAll(tailingTasks.Concat(new[] { monitorTask }));

            // Keep the service alive
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }

            _logger.LogInformation("Silent Log Agent stopping...");
        }
    }
}
