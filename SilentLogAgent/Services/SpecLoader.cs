using SilentLogAgent.Models;
using System.Text.Json;

namespace SilentLogAgent.Services
{
    public class SpecLoader : ISpecLoader
    {
        private readonly List<LogExpectation> _expectations;

        public SpecLoader(IConfiguration configuration, ILogger<SpecLoader> logger)
        {
            var specPath = configuration["SpecFilePath"] ?? "specs/spec.json";

            if (!File.Exists(specPath))
            {
                logger.LogError("Spec file not found at path: {Path}", specPath);
                _expectations = new List<LogExpectation>();
                return;
            }

            try
            {
                var json = File.ReadAllText(specPath);
                var expectations = JsonSerializer.Deserialize<List<LogExpectation>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                _expectations = expectations ?? new List<LogExpectation>();
                logger.LogInformation("Loaded {Count} log expectations from spec", _expectations.Count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to load or parse spec file at {Path}", specPath);
                _expectations = new List<LogExpectation>();
            }
        }

        public IReadOnlyList<LogExpectation> GetExpectations() 
            => _expectations;
    }
}