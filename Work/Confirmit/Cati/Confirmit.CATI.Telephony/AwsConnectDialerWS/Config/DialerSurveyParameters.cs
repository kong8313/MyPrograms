using System;
using System.Linq;
using System.Xml.Serialization;

namespace Confirmit.CATI.Telephony.AwsConnectDialerWS.Config
{
    public class DialerSurveyParameters
    {
        [XmlElement("DialerParameter")]
        public DialerParameter[] DialerParameters { get; set; }

        public string GetValue(string key)
        {
            var dialerParam = DialerParameters.FirstOrDefault(x => x.Id == key);
            if (dialerParam != null)
                return dialerParam.Value;

            return null;
        }

        public bool GetBoolValue(string key)
        {
            var strValue = GetValue(key);
            if (strValue != null)
                return strValue.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase);

            return false;
        }
    }

    public class DialerParameter
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
    }

    public static class DialerParameterKnownNames
    {
        public static string SourcePhoneNumber => nameof(SourcePhoneNumber);
        public static string CallerID => nameof(CallerID);
        public static string AnsMachineDetect => nameof(AnsMachineDetect);
    }
}
