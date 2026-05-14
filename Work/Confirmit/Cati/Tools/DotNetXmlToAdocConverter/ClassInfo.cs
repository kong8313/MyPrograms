using System.Collections.Generic;

namespace DotNetXmlToAdocConverter
{
    public class ClassInfo : PropertyInfo
    {
        public List<ConstructorInfo> Constructors { get; set; }

        public List<MethodInfo> Methods { get; set; }

        public List<PropertyInfo> Properties { get; set; }

        public string Namespace { get; set; }

        public ClassInfo()
        {
            Constructors = new List<ConstructorInfo>();
            Methods = new List<MethodInfo>();
            Properties = new List<PropertyInfo>();
        }
    }
}