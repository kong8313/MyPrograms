using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Confirmit.CATI.Core.Services.RecordsMigration
{
    public class EventStateDeserializationBinder : ISerializationBinder
    {
        private const string RootNamespace = "Confirmit.CATI.Monitoring.Common.StateData";

        private static readonly Dictionary<string, Type> Cache = new Dictionary<string, Type>();

        public Type BindToType(string assemblyName, string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName) || !typeName.StartsWith(RootNamespace))
                throw new JsonSerializationException($"Invalid type: {typeName}");

            // Check cache
            if (Cache.TryGetValue(typeName, out var cachedType))
                return cachedType;

            Type foundType = null;
            
            if (!string.IsNullOrWhiteSpace(assemblyName))
            {
                var assembly = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == assemblyName);

                if (assembly != null)
                {
                    try
                    {
                        foundType = assembly.GetType(typeName, false);
                    }
                    catch
                    {
                        // ignore the problem assembly
                    }
                }
            }
            
            if (foundType == null)
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        var type = assembly.GetType(typeName, false);
                        if (type != null)
                        {
                            foundType = type;
                            break;
                        }
                    }
                    catch
                    {
                        // ignore problem dependencies
                    }
                }
            }

            Cache[typeName] = foundType ?? throw new JsonSerializationException($"Type not found: {typeName}, assembly: {assemblyName}");

            return foundType;
        }

        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            throw new NotSupportedException();
        }
    }
}