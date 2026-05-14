using System.Diagnostics;

namespace SimulatorDialerDriver.Models
{
    public class SectionalRecordingInfo
    {
        public SectionalRecordingInfo(string label)
        {
            Label = label;
            Timer = Stopwatch.StartNew();
        }

        public string Label { get; }
        public Stopwatch Timer { get; }
    }
}