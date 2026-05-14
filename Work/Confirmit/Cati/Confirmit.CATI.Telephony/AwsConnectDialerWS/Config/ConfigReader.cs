using System.IO;
using System.Xml.Serialization;

namespace Confirmit.CATI.Telephony.AwsConnectDialerWS.Config
{
    public static class ConfigReader
    {
        public static T Read<T>(string xml)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            using (TextReader reader = new StringReader(xml))
            {
                return (T)xmlSerializer.Deserialize(reader);
            }
        }
    }
}