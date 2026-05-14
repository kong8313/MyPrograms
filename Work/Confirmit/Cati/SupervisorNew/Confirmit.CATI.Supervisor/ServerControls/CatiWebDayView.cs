using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Xml;

namespace Confirmit.CATI.Supervisor.ServerControls
{
    public class CatiWebDayView : Infragistics.WebUI.WebSchedule.WebDayView
    {
        protected override bool OnLoadPostData(string postDataKey, NameValueCollection values)
        {
            // For some reason sometimes ScrollPosition comes as float in form data and we get an exception on PostBack
            // We always set it to -1 to prevent such problems
            var data = values[$"{ClientID}_Data"];
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                string xml = HttpUtility.UrlDecode(data, Encoding.Default).Trim();
                var pattern =  "ScrollPosition=\".+?\"";
                var replacement = "ScrollPosition=\"-1\"";
                var rgx = new System.Text.RegularExpressions.Regex(pattern);
                data = rgx.Replace(xml, replacement);
                data = HttpUtility.UrlEncode(data, Encoding.Default);
            }
            catch 
            {
                data = "";
            }

            var collection = new NameValueCollection();
            collection.Add($"{ClientID}_Data", data);
            return base.OnLoadPostData(postDataKey, collection);
        }
    }
}