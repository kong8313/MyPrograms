using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace DialerCommon.DialerParameters
{
    /// <summary>
    /// Class contains general methods working with dialer parameters.
    /// </summary>
    public static class DialerParametersSerializer
    {
        /// <summary>
        /// The name of xml section contains dialer survey parameters.
        /// </summary>
        public const string DialerSurveyParametersSectionName = "DialerSurveyParameters";

        public static XmlSerializer XmlSerializer = new XmlSerializer(typeof(List<DialerParameter>), new XmlRootAttribute(DialerSurveyParametersSectionName));

        /// <summary>
        /// Gets collection of dialer parameters from the parameters xml string.
        /// </summary>
        /// <param name="xmlParametersString"></param>
        /// <returns></returns>
        public static IEnumerable<DialerParameter> DeserializeDialerParameters(string xmlParametersString)
        {
            if (string.IsNullOrWhiteSpace(xmlParametersString))
            {
                return new List<DialerParameter>();    
            }

            using (var stringReader = new StringReader(xmlParametersString))
            using (var xmlReader = XmlReader.Create(stringReader))
                {
                    return (IEnumerable<DialerParameter>) XmlSerializer.Deserialize(xmlReader);
                }
        }

        /// <summary>
        /// Gets parameters xml string from the collection of dialer parameters .
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>

        public static string SerializeDialerParameters(IEnumerable<DialerParameter> parameters)
        {
            using (var stringWriter = new StringWriter())
            {
                XmlSerializer.Serialize(stringWriter, parameters.ToList());
                return stringWriter.ToString();
            }
        }
    }
}
