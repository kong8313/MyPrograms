using System.Collections.Generic;

namespace NFakes
{
    public class InterfaceProps
    {
        public string InterfaceName { get; set; }
        public string GenericArguments { get; set; }
        public string Constraints { get; set; }
        public string Namespace { get; set; }
        public List<MethodProps> MethodsProps { get; set; }
        public List<PropertyProps> PropertyProps { get; set; }
        public List<EventProps> EventProps { get; set; }
        public HashSet<string> UsedNamespaces { get; set; }
        public bool Ignore { get; set; }
    }
}