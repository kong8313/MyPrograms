using System.Xml.Serialization;

namespace Confirmit.CATI.Telephony.SimulatorDialerDriver
{
    public class CallOutcomeMetadata
    {
        [XmlAttribute]
        public string Key { get; set; }
        
        [XmlAttribute]
        public string Value { get; set; }
    }
}