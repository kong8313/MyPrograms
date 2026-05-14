using System.IO;
using System.Xml.Serialization;

namespace Confirmit.CATI.Core.Misc.Extensions
{
    public static class XmlSerializerExtenstions
    {
        public static string SerializeToString(this XmlSerializer serializer, object o)
        {
            using (var stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, o);
                
                return stringWriter.ToString();
            }
        }
    }
}
