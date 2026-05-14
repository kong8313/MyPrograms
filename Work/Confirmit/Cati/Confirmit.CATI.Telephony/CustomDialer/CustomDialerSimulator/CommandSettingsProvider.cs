using System.IO;
using System.Xml.Serialization;

namespace CustomDialerSimulator
{
    internal  class CommandSettingsProvider
    {
        private readonly string ConfigFilePath;
        private Commands _commands;
        
        private Commands Commands
        {
            get { return _commands ?? (_commands = DeserializeDialCommandsFromXML()); }
        }

        public CommandSettingsProvider(string configFilePath)
        {
            ConfigFilePath = configFilePath;
        }

        public CommandSettings GetByIndexCyclically(long dialCommandIndex)
        {
            return Commands.DialCommands[dialCommandIndex % Commands.DialCommands.Length];
        }
        
        private Commands DeserializeDialCommandsFromXML()
        {
            Commands commands;

            var deserializer = new XmlSerializer(typeof(Commands));
            
            using (TextReader textReader = new StreamReader(ConfigFilePath))
            {
                commands = (Commands)deserializer.Deserialize(textReader);    
            }                                   

            return commands;
        }
    }
}