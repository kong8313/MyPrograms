using System;
using System.Collections.Generic;
using ConfirmitDialerInterface;

namespace SimulatorDialerDriver.Distribution
{
    public class CallOutcomeGenerator : BaseGenerator<CallOutcome>
    {
        public CallOutcomeGenerator(string name)
            : base(name)
        {
        }

        public override string Type => "CallOutcome";
        public override CallOutcome Parse(string value) => (CallOutcome) int.Parse(value);
    }
}