using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using Confirmit.CATI.Monitoring.Common;
using Confirmit.CATI.Monitoring.Common.Contracts;
using Confirmit.CATI.Monitoring.Common.StateData;
using Ionic.Zlib;

namespace Confirmit.CATI.Core.Services.RecordsMigration
{
    public class BinaryFormatterStateEventInfoDepacker
    {
        private readonly MemoryStream _source;
        
        public BinaryFormatterStateEventInfoDepacker(byte[] buffer)
        {
            _source = new MemoryStream(buffer);
        }
        
        public List<MonitoringEvent> GetAllEvents()
        {
            var deserializedEvents = Deserialize();
            return ConvertToMonitoringEvents(deserializedEvents);
        }
        
        private List<StateEventInfo> Deserialize()
        {
            BinaryFormatter formatter = new BinaryFormatter
            {
                AssemblyFormat = FormatterAssemblyStyle.Simple,
                Binder = new MonitoringIdentityInfoSerializationBinder()
            };
        
            var list = new List<StateEventInfo>();
            //TODO: Remove the compression check in one release.
            // 2011.05.17 Note that this implementation can be simplified. After 30 days after the release is installed
            // all mixed and old formated (without compression) data will be deleted from the database. So, only 
            // compressed data will be presented in the stream. We will not need to check if a part of the stream is compressed.
            long curPos = _source.Position;
            while (curPos < _source.Length)
            {
                // Returns to the logical end position. The source stream can be read further 
                // than the logical end of a stram part because of buffering in the compression stream.
                _source.Seek(curPos, SeekOrigin.Begin);

                // Check if a part of the stream is compressed.
                bool compressed = true;
                const int signatureSize = 4;
                var buff = new byte[signatureSize];
                if (_source.Length - curPos > signatureSize)
                {
                    if (_source.Read(buff, 0, signatureSize) == signatureSize)
                    {
                        // This is signature of a binary serialization (not compressed)
                        compressed = !buff.SequenceEqual(new byte[] { 0, 1, 0, 0 });
                    }
                    _source.Seek(curPos, SeekOrigin.Begin);
                }

                // Deserialize objects
                object o;
                if (compressed)
                {
                    using (var ds = new Ionic.Zlib.DeflateStream(_source, Ionic.Zlib.CompressionMode.Decompress, true))
                    {
                        o = formatter.Deserialize(ds);
                        curPos += ds.Position;
                    }
                }
                else
                {
                    o = formatter.Deserialize(_source);
                    curPos = _source.Position;
                }

                // Add to the list depending on a type.
                // An event, an array of events and a list of events are supported.
                if (o is StateEventInfo)
                {
                    list.Add((StateEventInfo)o);
                }
                else
                    if (o is IEnumerable<StateEventInfo>)
                    {
                        list.AddRange((IEnumerable<StateEventInfo>)o);
                    }
                    else
                    {
                        throw new InvalidDataException(String.Format("Invalid stream content. Cannot process the '{0}' class",
                                                          o.GetType()));
                    }
            }
            list.Sort((a, b) => DateTime.Compare(a.TimeStamp, b.TimeStamp));
            return list;
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
            
            return DeserializeStateByBinaryFormatter(type, binary);
        }

        private static BaseStateData DeserializeStateByBinaryFormatter(MonitoringMessageTypes type, byte[] binary)
        {
            var formatter = new BinaryFormatter
            {
                AssemblyFormat = FormatterAssemblyStyle.Simple,
                Binder = new MonitoringIdentityInfoSerializationBinder()
            };

            using (Stream stream = new MemoryStream())
            {
                stream.Write(binary, 0, binary.Length);
                stream.Seek(0, SeekOrigin.Begin);
                if (type is MonitoringMessageTypes.MonitoringInitialMessage ||
                    type is  MonitoringMessageTypes.InterviewInitialMessage ||
                    type is  MonitoringMessageTypes.InterviewPageBrowserPageCompletedMessage)
                {
                    // we are going to decompress document complete message because 
                    // we compressed it during serialization
                    var decompressedStream = new DeflateStream(stream, CompressionMode.Decompress);
                    return (BaseStateData)formatter.Deserialize(decompressedStream);
                }

                return (BaseStateData)formatter.Deserialize(stream);
            }
        }
    }
}