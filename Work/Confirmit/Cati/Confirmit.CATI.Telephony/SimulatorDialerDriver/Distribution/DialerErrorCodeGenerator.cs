using ConfirmitDialerInterface;

namespace SimulatorDialerDriver.Distribution
{
    public class DialerErrorCodeGenerator : BaseGenerator<DialerErrorCode>
    {
        public DialerErrorCodeGenerator(string name) 
            : base(name)
        {
        }

        public override string Type => "DialerErrorCode";
        public override DialerErrorCode Parse(string value) => (DialerErrorCode) int.Parse(value);
    }
}