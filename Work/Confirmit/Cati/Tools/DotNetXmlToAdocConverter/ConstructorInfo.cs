using System.Collections.Generic;

namespace DotNetXmlToAdocConverter
{
    public class ConstructorInfo : PropertyInfo
    {
        public List<PropertyInfo> Parameters { get; set; }

        public ConstructorInfo()
        {
            Parameters = new List<PropertyInfo>();
        }
    }
}