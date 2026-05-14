using System.Collections.Generic;
using System.Linq;

namespace Confirmit.CATI.Telephony.SimulatorDialerDriver
{
    internal class IvrSimulatedResponse
    {
        public IEnumerable<KeyValuePair<string, string>> Variables { get; set; }
        public KeyValuePair<string, string>? SimulatedUserInput { get; set; }

        public KeyValuePair<string, string>[] ToSubmitVariables()
        {
            return (SimulatedUserInput == null)
                ? Variables.ToArray()
                : Variables.Union(new[] { (KeyValuePair<string, string>)SimulatedUserInput }).ToArray();
        }
    }
}