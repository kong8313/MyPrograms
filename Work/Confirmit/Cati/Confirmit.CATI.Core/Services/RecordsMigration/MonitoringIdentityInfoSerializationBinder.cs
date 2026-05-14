using System;
using System.Runtime.Serialization;

namespace Confirmit.CATI.Core.Services.RecordsMigration
{
    public class MonitoringIdentityInfoSerializationBinder : SerializationBinder
    {
        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = "Confirmit.CATI.Common";
            typeName = "Confirmit.CATI.Common.Monitoring." + serializedType.Name;
        }

        public override Type BindToType(string assemblyName, string typeName)
        {
            return Type.GetType(typeName);
        }
    }
}