namespace Confirmit.CATI.Common.Logging
{
    public interface IEventDetails
    {
        void AddTiming(string timingName);
        void AddTiming(string timingName, int minimumTimingToIgnore);
        void AddTiming(string format, params object[] args);
        void AddMessage(string format, params object[] args);
    }
}