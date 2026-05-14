using System;
using System.Xml.Serialization;
using Confirmit.CATI.Common.Random;
using ConfirmitDialerInterface;

namespace SimulatorDialerDriver
{
    public class CallOutcomeDistributionData
    {
        [XmlAttribute]
        public CallOutcome CallOutcome { get; set; }

        [XmlAttribute("ProcessingTime")]
        public string ProcessingTimeFormattedString { get; set; }

        private TimeSpan? processingTime;
        [XmlIgnore]
        public TimeSpan ProcessingTime
        {
            get
            {
                if (processingTime.HasValue) return processingTime.Value;

                TimeSpan result;

                var parts = ProcessingTimeFormattedString.Split('-');
                if (parts.Length == 2)
                {
                    //range is specified
                    var leftBound = Int32.Parse(parts[0]);
                    var rightBound = Int32.Parse(parts[1]);
                    result = TimeSpan.FromSeconds(Randomizer.Next(leftBound, rightBound));
                }
                else
                {
                    var parsedTime = Int32.Parse(ProcessingTimeFormattedString);
                    result = TimeSpan.FromSeconds(parsedTime);
                }

                processingTime = result;
                return processingTime.Value;
            }
        }

        [XmlAttribute]
        public int DistributionWeight { get; set; }
    }
}
