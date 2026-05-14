using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Confirmit.CATI.Monitoring.Common;
using Confirmit.CATI.Monitoring.Common.Contracts;
using Confirmit.CATI.Monitoring.Common.StateData;
using Newtonsoft.Json;

namespace Confirmit.CATI.Core.Services.RecordsMigration
{
    public class StateEventInfoPacker
    {
        private readonly List<MonitoringEvent> _stateEvents;
        
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Auto,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            SerializationBinder = new EventStateSerializationBinder()
        };
        
        public StateEventInfoPacker(List<MonitoringEvent> stateEvents)
        {
            _stateEvents = stateEvents;
        }
        
        public byte[] SerializeAllEvents()
        {
            var stateEvents = _stateEvents.Select(x => new StateEventInfo
                {
                    MessageType = (MonitoringMessageTypes)x.MessageType,
                    TimeStamp = x.TimeStamp,
                    State = SerializeState((MonitoringMessageTypes)x.MessageType, x.Data)
                })
                .ToArray();

            return CreatePackage(stateEvents);
        }
        
        private byte[] CreatePackage(IEnumerable<StateEventInfo> events)
        {
            using (var packetStream = new MemoryStream(16 * 1024))
            {
                using (var deflateStream = new DeflateStream(packetStream, CompressionMode.Compress, true))
                using (var streamWriter = new StreamWriter(deflateStream, new UTF8Encoding(false)))
                using (var jsonWriter = new JsonTextWriter(streamWriter))
                {
                    var serializer = JsonSerializer.Create(JsonSettings);
                    serializer.Serialize(jsonWriter, events);
                }

                return packetStream.ToArray();
            }
        }
        
        private static byte[] SerializeState(MonitoringMessageTypes type, BaseStateData obj)
        {
            if (obj == null)
                return Array.Empty<byte>();

            // Serialize to JSON
            string json = JsonConvert.SerializeObject(obj, typeof(BaseStateData), JsonSettings);

            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

            // Apply compression only for selected message types
            if (type == MonitoringMessageTypes.MonitoringInitialMessage ||
                type == MonitoringMessageTypes.InterviewInitialMessage ||
                type == MonitoringMessageTypes.InterviewPageBrowserPageCompletedMessage)
            {
                using (var tmp = new MemoryStream())
                {
                    using (var compressed = new DeflateStream(tmp, CompressionMode.Compress, true))
                    {
                        compressed.Write(jsonBytes, 0, jsonBytes.Length);
                    }

                    return tmp.ToArray();
                }
            }

            return jsonBytes;
        }
    }
}