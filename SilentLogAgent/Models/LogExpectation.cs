namespace SilentLogAgent.Models
{
    public class LogExpectation
    {
        public string Name { get; set; } = string.Empty;
        public string LogFilePath { get; set; } = string.Empty;
        public string Pattern { get; set; } = string.Empty;  // regex to match meaningful log entries
        public int SilenceThresholdMinutes { get; set; }  // threshold for silence
        public bool IsCritical { get; set; }
        public SpecLabel? Labels { get; set; }
    }
}
