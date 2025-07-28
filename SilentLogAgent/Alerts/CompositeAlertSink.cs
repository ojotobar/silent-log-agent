using SilentLogAgent.Models;

namespace SilentLogAgent.Alerts
{
    public class CompositeAlertSink : IAlertSink
    {
        private readonly IEnumerable<IAlertSink> _sinks;

        public CompositeAlertSink(IEnumerable<IAlertSink> sinks)
        {
            _sinks = sinks;
        }

        public async Task SendAlertAsync(LogExpectation expectation, TimeSpan silenceDuration)
        {
            foreach (var sink in _sinks)
            {
                await sink.SendAlertAsync(expectation, silenceDuration);
            }
        }
    }
}
