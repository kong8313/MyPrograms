namespace BuildServerRegistrator
{
    public class CommandLineParameters
    {
        public RegistrationAction RegistrationAction { get; set; }

        public Branch Branch { get; set; }

        public string ConfigPath { get; set; }
    }
}
