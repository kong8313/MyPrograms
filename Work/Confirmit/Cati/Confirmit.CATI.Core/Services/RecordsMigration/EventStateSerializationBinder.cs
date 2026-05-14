using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Confirmit.CATI.Core.Services.RecordsMigration
{
    public class EventStateSerializationBinder : ISerializationBinder
    {
        private const string RootNamespace = "Confirmit.CATI.Monitoring.Common.StateData";

        public Type BindToType(string assemblyName, string typeName)
        {
            throw new NotSupportedException();
        }

        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            if (string.IsNullOrEmpty(serializedType.FullName) || !serializedType.FullName.StartsWith(RootNamespace))
                throw new JsonSerializationException("Invalid type");

            assemblyName = "CommonContract";
            typeName = serializedType.FullName;
        }
    }
}