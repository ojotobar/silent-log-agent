using SilentLogAgent.Models;

namespace SilentLogAgent.Alerts
{
    public interface IAlertSink
    {
        Task SendAlertAsync(LogExpectation expectation, TimeSpan silenceDuration);
    }
}
