using Microsoft.Extensions.Options;
using SilentLogAgent.Models;
using System.Text.Json;
using System.Text;

namespace SilentLogAgent.Alerts
{
    public class SlackAlertSink : IAlertSink
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SlackAlertSink> _logger;
        private readonly string _webhookUrl;

        public SlackAlertSink(HttpClient httpClient, ILogger<SlackAlertSink> logger, IOptions<SlackOptions> options)
        {
            _httpClient = httpClient;
            _logger = logger;
            _webhookUrl = options.Value.WebhookUrl;
        }

        public async Task SendAlertAsync(LogExpectation expectation, TimeSpan silenceDuration)
        {
            var message = $"🚨 *Log Silence Alert*\n" +
                          $"*Name:* `{expectation.Name}`\n" +
                          $"*File:* `{expectation.LogFilePath}`\n" +
                          $"*Threshold:* `{expectation.SilenceThresholdMinutes}` minutes\n" +
                          $"*Silence Duration:* `{silenceDuration.TotalMinutes:F1}` minutes\n" +
                          $"*Critical:* `{expectation.IsCritical}`";

            var payload = new
            {
                text = message
            };
            
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(_webhookUrl, content);
                response.EnsureSuccessStatusCode();
                _logger.LogInformation("Slack alert sent for {Name}", expectation.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send Slack alert for {Name}", expectation.Name);
            }
        }
    }
}
