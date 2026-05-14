using System;
using System.Runtime.Serialization;

namespace Confirmit.CATI.Monitoring.Common
{
    /// <summary>
    /// Used to map old event-class namespaces into their new home
    /// </summary>
    /// <remarks>    
    /// For more detail see bug 47277
    /// This classs can be deleted after deffered records expired (~ 1 month after Adama release).
    /// </remarks>
    public sealed class NamespaceMapperDeserializationBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {            
            if (typeName.StartsWith("CatiInterviewerConsole.Monitoring"))
            {
                typeName = typeName.Replace("CatiInterviewerConsole.Monitoring", "Confirmit.CATI.Monitoring.Common");                
            }

            return Type.GetType(typeName);
        }
    }
}
