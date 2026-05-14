using System;

namespace BuildServerRegistrator
{
    public class Program
    {
        private const string HelpMessage = @"
Usages: BuildServerRegistrator /Branch <master | release | ltu> /Action <register | unregister> /Config <path to ServerConfiguration.xml>
Check config file Configs/ServerConfiguration.xml. Information for both servers must be correct.";

        private readonly ConfigParser _configParser;
        private readonly RegistryRegistrator _registryRegistrator;
        private readonly SqlRegistrator _sqlRegistrator;
        private readonly CommandLineParser _commandLineParser;

        public Program()
        {
            _configParser = new ConfigParser();
            _registryRegistrator = new RegistryRegistrator();
            _sqlRegistrator = new SqlRegistrator();
            _commandLineParser = new CommandLineParser();
        }

        public static void Main(string[] args)
        {
            try
            {                
                new Program().Start(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                Console.WriteLine(HelpMessage);
            }
        }

        private void Start(string[] args)
        {
            Console.WriteLine($"Initialization...");

            CommandLineParameters parameters = _commandLineParser.Parse(args);

            var serverConfigurations = _configParser.Parse(parameters);

            var confirmQueryExecutor = new ConfirmQueryExecutor(serverConfigurations);

            var defaultCredentialsMaker = new DefaultCredentialsMaker(confirmQueryExecutor, serverConfigurations);
            defaultCredentialsMaker.SetDefaultCfgServerConfigValues();

            if (parameters.RegistrationAction == RegistrationAction.Register)
            {
                Console.WriteLine($"Start registering {Environment.MachineName}");

                _sqlRegistrator.Register(serverConfigurations, confirmQueryExecutor);
                _registryRegistrator.Register(serverConfigurations, defaultCredentialsMaker);
            }
            else
            {
                Console.WriteLine($"Start unregistering {Environment.MachineName}");

                _sqlRegistrator.Unregister(serverConfigurations, confirmQueryExecutor);
            }

            Console.WriteLine("Execution has successfully finished");
        }
    }
}
