using System.IO;
using System.Xml.Serialization;

namespace Confirmit.CATI.Core.Misc
{
    public class XmlSerialization
    {
        public static string Serialize<T>(T input) where T : class
        {
            var serializer = new XmlSerializer(typeof(T));

            using (var stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, input);

                return stringWriter.ToString();
            }
        }
        public static T Deserialize<T>(string input) where T : class
        {
            var serializer = new XmlSerializer(typeof(T));

            using (StringReader sr = new StringReader(input))
                return (T)serializer.Deserialize(sr);
        }
    }
}