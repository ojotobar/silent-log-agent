using SilentLogAgent.Models;
using System.Text;

namespace SilentLogAgent.Services
{
    public class LogTailService
    {
        private readonly ILogger<LogTailService> _logger;
        private readonly LogAnalyzer _logAnalyzer;
        private readonly IEnumerable<LogExpectation> _expectations;

        public LogTailService(ILogger<LogTailService> logger, LogAnalyzer logAnalyzer, IEnumerable<LogExpectation> expectations)
        {
            _logger = logger;
            _logAnalyzer = logAnalyzer;
            _expectations = expectations;
        }

        public async Task TailFileAsync(LogExpectation expectation, CancellationToken token)
        {
            var path = expectation.LogFilePath;
            if (!File.Exists(path))
            {
                _logger.LogWarning("Log file not found: {Path}", path);
                return;
            }

            try
            {
                using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var sr = new StreamReader(fs, Encoding.UTF8);

                // Jump to end of file
                _ = await sr.ReadToEndAsync();

                while (!token.IsCancellationRequested)
                {
                    var line = await sr.ReadLineAsync();
                    if (line != null)
                    {
                        _logAnalyzer.ProcessLine(expectation, line);
                    }
                    else
                    {
                        await Task.Delay(500, token);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tailing file: {Path}", path);
            }
        }
    }
}
