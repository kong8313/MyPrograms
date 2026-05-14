namespace Confirmit.CATI.Core.Services
{
    public interface IDeferredMonitoringService
    {
        /// <summary>
        /// Returns starting file for given deferred record. It is xml string which contains data
        /// needed to start deferred monitoring at client side.
        /// </summary>
        /// <returns>XML string.</returns>
        string GetStartFile(int recordId);

        void AppendToEventsFile(int id, byte[] packet);

        void CompleteRecord(int id, byte[] packet, bool hasAudio, bool requestAudio, bool updateDuration);
    }
}