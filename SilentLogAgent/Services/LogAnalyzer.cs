using SilentLogAgent.Models;
using System.Text.RegularExpressions;
using System.Threading;

namespace SilentLogAgent.Services
{
    public class LogAnalyzer
    {
        private readonly ExpectationMonitor _expectationMonitor;
        private readonly ILogger<LogAnalyzer> _logger;

        public LogAnalyzer(ExpectationMonitor expectationMonitor, ILogger<LogAnalyzer> logger)
        {
            _expectationMonitor = expectationMonitor;
            _logger = logger;
        }

        public void ProcessLine(LogExpectation expectation, string line)
        {
            var match = Regex.Match(line, expectation.Pattern);
            if (!match.Success)
                return;

            if (!match.Groups.TryGetValue(expectation.TimestampGroupName, out var tsGroup))
            {
                _logger.LogWarning("Timestamp group '{Group}' not found in line: {Line}", expectation.TimestampGroupName, line);
                return;
            }

            if (!DateTimeOffset.TryParse(tsGroup.Value, out var timestamp))
            {
                _logger.LogWarning("Failed to parse timestamp '{Timestamp}' in line: {Line}", tsGroup.Value, line);
                return;
            }

            _logger.LogInformation("Matched log for '{ExpectationName}' at {Timestamp}", expectation.Name, timestamp);
            _expectationMonitor.UpdateLastSeen(expectation, timestamp);  // ✅ Track latest match
        }
    }
}
