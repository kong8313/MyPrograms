using System;
using System.IO;

namespace BuildServerRegistrator
{
    public class CommandLineParser
    {
        public CommandLineParameters Parse(string[] args)
        {
            try
            {
                RegistrationAction? registrationAction = null;
                Branch? branch = null;
                string configPath = null;

                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i].ToLowerInvariant())
                    {
                        case "/branch":
                            branch = (Branch)Enum.Parse(typeof(Branch), args[++i], true);
                            break;
                        case "/action":
                            registrationAction = (RegistrationAction)Enum.Parse(typeof(RegistrationAction), args[++i], true);
                            break;
                        case "/config":
                            configPath = args[++i];
                            break;
                    }
                }

                if (!registrationAction.HasValue || !branch.HasValue || string.IsNullOrEmpty(configPath))
                {
                    throw new Exception("Not all parameters are specified");
                }

                if (!File.Exists(configPath))
                {
                    throw new Exception($"Config file path '{configPath}' is not found");
                }

                return new CommandLineParameters
                {
                    RegistrationAction = registrationAction.Value,
                    Branch = branch.Value,
                    ConfigPath = configPath
                };
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Wrong command line format", ex);
            }
        }
    }
}
