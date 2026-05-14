using System.Collections.Generic;

namespace SimulatorDialerDriver.Distribution
{
    public interface IGenerator
    {
        string Name { get; }
        string Type { get; }
        List<GeneratorBehavior> Behaviors { get; set; }
        string Check(string value);
    }
}
