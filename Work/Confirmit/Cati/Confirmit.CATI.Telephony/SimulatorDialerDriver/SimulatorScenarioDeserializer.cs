using System.IO;
using System.Xml.Serialization;

namespace Confirmit.CATI.Telephony.SimulatorDialerDriver
{
    public class SimulatorScenarioDeserializer
    {
        public SimulatorScenario Deserialize(string scenarioXmlFileName)
        {
            var xmlSerializer = new XmlSerializer(typeof(SimulatorScenario));

            using (var streamReader = File.OpenText(scenarioXmlFileName))
            {
                var deserializedObject = (SimulatorScenario) xmlSerializer.Deserialize(streamReader);
                deserializedObject.OnDeserialized();

                if (string.IsNullOrEmpty(deserializedObject.CallOutcomeDistributionScenario.OutcomePhonePrefix))
                {
                    deserializedObject.CallOutcomeDistributionScenario.OutcomePhonePrefix = "5554321";
                }

                return deserializedObject;
            }
        }
    }
}