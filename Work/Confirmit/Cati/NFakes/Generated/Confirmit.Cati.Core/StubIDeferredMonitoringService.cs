using System;
using Confirmit.CATI.Core.Services;

namespace Confirmit.CATI.Core.Services.Fakes
{
    public class StubIDeferredMonitoringService : IDeferredMonitoringService 
    {
        private IDeferredMonitoringService _inner;

        public StubIDeferredMonitoringService()
        {
            _inner = null;
        }

        public IDeferredMonitoringService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GetStartFileInt32Delegate(int recordId);
        public GetStartFileInt32Delegate GetStartFileInt32;

        string IDeferredMonitoringService.GetStartFile(int recordId)
        {


            if (GetStartFileInt32 != null)
            {
                return GetStartFileInt32(recordId);
            } else if (_inner != null)
            {
                return ((IDeferredMonitoringService)_inner).GetStartFile(recordId);
            }

            return default(string);
        }

        public delegate void AppendToEventsFileInt32ArrayOfByteDelegate(int id, byte[] packet);
        public AppendToEventsFileInt32ArrayOfByteDelegate AppendToEventsFileInt32ArrayOfByte;

        void IDeferredMonitoringService.AppendToEventsFile(int id, byte[] packet)
        {

            if (AppendToEventsFileInt32ArrayOfByte != null)
            {
                AppendToEventsFileInt32ArrayOfByte(id, packet);
            } else if (_inner != null)
            {
                ((IDeferredMonitoringService)_inner).AppendToEventsFile(id, packet);
            }
        }

        public delegate void CompleteRecordInt32ArrayOfByteBooleanBooleanBooleanDelegate(int id, byte[] packet, bool hasAudio, bool requestAudio, bool updateDuration);
        public CompleteRecordInt32ArrayOfByteBooleanBooleanBooleanDelegate CompleteRecordInt32ArrayOfByteBooleanBooleanBoolean;

        void IDeferredMonitoringService.CompleteRecord(int id, byte[] packet, bool hasAudio, bool requestAudio, bool updateDuration)
        {

            if (CompleteRecordInt32ArrayOfByteBooleanBooleanBoolean != null)
            {
                CompleteRecordInt32ArrayOfByteBooleanBooleanBoolean(id, packet, hasAudio, requestAudio, updateDuration);
            } else if (_inner != null)
            {
                ((IDeferredMonitoringService)_inner).CompleteRecord(id, packet, hasAudio, requestAudio, updateDuration);
            }
        }

    }
}