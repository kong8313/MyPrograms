using System.Xml;

namespace BuildServerRegistrator
{
    public class ConfigParser
    {        
        public ConfigParameters Parse(CommandLineParameters commandLineParameters)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(commandLineParameters.ConfigPath);

            var serverConfigurationNode = xmlDocument.SelectSingleNode($"//ServerConfigurations/ServerConfiguration[@name='{commandLineParameters.Branch.ToString().ToLowerInvariant()}']");

            return new ConfigParameters
            {
                SqlServerName = serverConfigurationNode.SelectSingleNode("SqlServerName").InnerText,
                SqlLoginName = serverConfigurationNode.SelectSingleNode("SqlLoginName").InnerText,
                SqlPassword = serverConfigurationNode.SelectSingleNode("SqlPassword").InnerText
            };
        }
    }
}
