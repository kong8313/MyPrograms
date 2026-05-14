using System;
using System.Collections.Generic;

namespace SimulatorDialerDriver.Distribution
{
    public class TimeSpanGenerator : BaseGenerator<TimeSpan>
    {
        public TimeSpanGenerator(string name)
            : base(name)
        {
        }

        public override string Type => "TimeSpan";
        public override TimeSpan Parse(string value) => TimeSpan.Parse(value);
    }
}