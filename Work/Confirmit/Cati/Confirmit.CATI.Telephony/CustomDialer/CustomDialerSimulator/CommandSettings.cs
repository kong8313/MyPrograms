using System.Xml.Serialization;

namespace CustomDialerSimulator
{    
    public class Commands
    {
        [XmlArray("DialCommands")]
        [XmlArrayItem("Command")]
        public CommandSettings[] DialCommands;
    }

    public class CommandSettings
    { 
        /// <summary>
        /// Sleep timeout in milliseconds
        /// </summary>
        public int Timeout;

        public int Result;
    }
}