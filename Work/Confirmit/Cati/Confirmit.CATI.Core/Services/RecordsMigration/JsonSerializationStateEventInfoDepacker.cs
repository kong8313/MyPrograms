using System.Collections.Generic;
using System.IO;
using System.Text;
using Confirmit.CATI.Monitoring.Common;
using Confirmit.CATI.Monitoring.Common.Contracts;
using Confirmit.CATI.Monitoring.Common.StateData;
using Ionic.Zlib;
using Newtonsoft.Json;

namespace Confirmit.CATI.Core.Services.RecordsMigration
{
    public class JsonSerializationStateEventInfoDepacker
    {
        private readonly MemoryStream _source;
        
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Auto,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            SerializationBinder = new EventStateDeserializationBinder()
        };
        
        public JsonSerializationStateEventInfoDepacker(byte[] buffer)
        {
            _source = new MemoryStream(buffer);
        }
        
        public List<MonitoringEvent> GetAllEvents()
        {
            var deserializedEvents = Deserialize();
            return ConvertToMonitoringEvents(deserializedEvents);
        }
        
        private List<MonitoringEvent> ConvertToMonitoringEvents(List<StateEventInfo> deserializedEvents)
        {
            var result = new List<MonitoringEvent>();
            if (deserializedEvents.Count > 0)
            {
                foreach (var stateEvent in deserializedEvents)
                {
                    result.Add(new MonitoringEvent
                    {
                        TimeStamp = stateEvent.TimeStamp,
                        MessageType = (int)stateEvent.MessageType,
                        Data = DeserializeState(stateEvent.MessageType, stateEvent.State)
                    });
                }
            }
            return result;
        }
        
        private static BaseStateData DeserializeState(MonitoringMessageTypes type, byte[] binary)
        {
            if (binary == null || binary.Length == 0)
            {
                return null;
            }
            
            byte[] jsonBytes = binary;

            if (type == MonitoringMessageTypes.MonitoringInitialMessage ||
                type == MonitoringMessageTypes.InterviewInitialMessage ||
                type == MonitoringMessageTypes.InterviewPageBrowserPageCompletedMessage)
            {
                using (var input = new MemoryStream(binary))
                using (var deflate = new DeflateStream(input, CompressionMode.Decompress))
                using (var output = new MemoryStream())
                {
                    deflate.CopyTo(output);
                    jsonBytes = output.ToArray();
                }
            }

            string json = Encoding.UTF8.GetString(jsonBytes);
            json = RemoveBom(json);
            
            return JsonConvert.DeserializeObject<BaseStateData>(json, JsonSettings);
        }
        
        private static string RemoveBom(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return text.TrimStart('\uFEFF', '\u200B');
        }
        
        private List<StateEventInfo> Deserialize()
        {
            var deserializedEvents = new List<StateEventInfo>();

            long curPos = _source.Position;

            while (curPos < _source.Length)
            {
                // Reset stream to logical block start
                _source.Seek(curPos, SeekOrigin.Begin);

                using (var ds = new DeflateStream(_source, CompressionMode.Decompress, leaveOpen: true))
                using (var ms = new MemoryStream())
                {
                    ds.CopyTo(ms);

                    var jsonBytes = ms.ToArray();

                    curPos += ds.Position;
                    string json = Encoding.UTF8.GetString(jsonBytes);
                    json = RemoveBom(json);

                    // Try to deserialize as collection
                    try
                    {
                        var collection = JsonConvert.DeserializeObject<List<StateEventInfo>>(json, JsonSettings);
                        if (collection != null)
                        {
                            deserializedEvents.AddRange(collection);
                            continue;
                        }
                    }
                    catch
                    {
                    }

                    // Try to deserialize as single object
                    try
                    {
                        var single = JsonConvert.DeserializeObject<StateEventInfo>(json, JsonSettings);
                        if (single != null)
                        {
                            deserializedEvents.Add(single);
                            continue;
                        }
                    }
                    catch
                    {
                    }

                    throw new InvalidDataException("Cannot deserialize JSON block. A possible reason is that data was serialized using a binary formatter.");
                }
            }

            return deserializedEvents;
        }
    }
}