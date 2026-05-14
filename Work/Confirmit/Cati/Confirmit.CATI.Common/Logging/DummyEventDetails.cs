namespace Confirmit.CATI.Common.Logging
{
    public class DummyEventDetails : IEventDetails
    {
        public void AddTiming(string timingName)
        {
        }

        public void AddTiming(string timingName, int minimumTimingToIgnore)
        {
        }

        public void AddTiming(string format, params object[] args)
        {
        }

        public void AddMessage(string format, params object[] args)
        {
        }
    }
}