using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using SimulatorDialerDriver;

namespace Confirmit.CATI.Telephony.SimulatorDialerDriver
{
    public enum CallOutcomeGenerationMethod
    {
        Sequence,
        Random
    };

    [Serializable]
    public class CallOutcomeDistributionScenario
    {
        [XmlElement]
        public List<CallOutcomeDistributionData> OutcomeList { get; set; }
        
        [XmlElement]
        public List<CallOutcomeMetadata> OutcomeMetadataList { get; set; }

        [XmlElement]
        public CallOutcomeGenerationMethod GenerationMethod { get; set; }

        [XmlElement]
        public string OutcomePhonePrefix { get; set; }

        [XmlElement]
        public int StartIteration { get; set; }
    }
}
