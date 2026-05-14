using ConfirmitDialerInterface;

namespace SimulatorDialerDriver.Distribution
{
    public class AgentStateGenerator : BaseGenerator<AgentState>
    {
        public AgentStateGenerator(string name)
            : base(name)
        {
        }

        public override string Type => "AgentState";
        public override AgentState Parse(string value) => (AgentState)int.Parse(value);
    }
}