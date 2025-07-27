using SilentLogAgent.Services;

namespace SilentLogAgent
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ISpecLoader specLoader;

        public Worker(ILogger<Worker> logger, ISpecLoader specLoader)
        {
            _logger = logger;
            this.specLoader = specLoader;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var specs = specLoader.GetExpectations();
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
