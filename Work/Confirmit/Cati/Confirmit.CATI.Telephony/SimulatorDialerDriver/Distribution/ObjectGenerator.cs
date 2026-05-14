using Newtonsoft.Json;

namespace SimulatorDialerDriver.Distribution
{
    public class ObjectGenerator<T> : BaseGenerator<T>
    {
        public ObjectGenerator(string name)
            : base(name)
        {
        }

        public override string Type => "Object";
        public override T Parse(string value) => JsonConvert.DeserializeObject<T>(value);
    }
}
