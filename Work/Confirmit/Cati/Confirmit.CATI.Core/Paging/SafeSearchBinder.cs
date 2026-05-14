using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Confirmit.CATI.Core.Paging
{
    public class SafeSearchBinder : ISerializationBinder
    {
        private static readonly HashSet<string> AllowedTypes = new HashSet<string>
        {
            typeof(int).FullName,
            typeof(long).FullName,
            typeof(double).FullName,
            typeof(decimal).FullName,
            typeof(DateTime).FullName,
            typeof(TimeSpan).FullName,
            typeof(string).FullName,
            typeof(SearchParameter).FullName,
            typeof(SearchParameterCollection).FullName,
            typeof(SearchPredefinedDate).FullName
        };

        public Type BindToType(string assemblyName, string typeName)
        {
            if (!AllowedTypes.Contains(typeName))
                throw new JsonSerializationException("Type not allowed: " + typeName);

            return Type.GetType($"{typeName}, {assemblyName}") ?? typeof(string);
        }

        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = serializedType.Assembly.GetName().Name;
            typeName = serializedType.FullName;
        }
    }
}