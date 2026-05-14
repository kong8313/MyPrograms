using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

using ConfirmitDialerInterface;

namespace Confirmit.CATI.Telephony.SimulatorDialerDriver
{
    public class SimulatorScenario
    {
        [XmlElement]
        public List<CallOutcome> CallOutcomeList { get; set; }
        [XmlElement]
        public CallOutcomeGenerationMethod GenerationMethod { get; set; }

        public static SimulatorScenario Deserialize(string scenarioXmlFileName)
        {
            var xmlSerializer = new XmlSerializer(typeof(SimulatorScenario));
            using (var streamReader = File.OpenText(scenarioXmlFileName))
            {
                return (SimulatorScenario)xmlSerializer.Deserialize(streamReader);
            }
        }

        public void Serialize(string scenarioXmlFileName)
        {
            var xmlSerializer = new XmlSerializer(typeof(SimulatorScenario));
            using (var streamWriter = File.CreateText(scenarioXmlFileName))
            {
                xmlSerializer.Serialize(streamWriter, this);
            }
        }

    }
}