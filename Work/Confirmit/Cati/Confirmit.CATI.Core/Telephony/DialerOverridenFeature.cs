using System;
using System.Runtime.Serialization;

namespace Confirmit.CATI.Core.Telephony
{
    public class DialerOverridenFeature 
    {
        public string Name { get; set; }

        public bool? DefaultValue { get; set; }

        public bool? OverridenValue { get; set; }

        public DialerOverridenFeature(string name, bool? defaultValue, bool? overridenValue)
        {
            Name = name;
            DefaultValue = defaultValue;
            OverridenValue = overridenValue;
        }

        //For xml serialization
        protected DialerOverridenFeature()
        {
        }
    }
}