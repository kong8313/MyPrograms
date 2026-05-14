using ConfirmitDialerInterface;

namespace SimulatorDialerDriver.Distribution
{
    public class TargetOutcomeGenerator : BaseGenerator<TargetOutcome>
    {
        public TargetOutcomeGenerator(string name)
            : base(name)
        {
        }

        public override string Type => "TargetOutcome";
        public override TargetOutcome Parse(string value) => (TargetOutcome)int.Parse(value);
    }
}