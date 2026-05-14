using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Confirmit.CATI.Backend.WebApiServices
{
    /// <summary>
    /// A link to a node
    /// </summary>
    public class Link
    {
        /// <summary>
        /// Description of relation
        /// </summary>
        [XmlAttribute("rel")]
        [JsonProperty("rel", NullValueHandling = NullValueHandling.Ignore)]
        public string Rel { get; set; }

        /// <summary>
        /// Defines the http method to use
        /// </summary>
        [XmlAttribute("method")]
        [JsonProperty("method", NullValueHandling = NullValueHandling.Ignore)]
        public string Method { get; set; }

        /// <summary>
        /// Relative url to node
        /// </summary>
        [XmlAttribute("href")]
        [JsonProperty("href", NullValueHandling = NullValueHandling.Ignore)]
        public string HRef { get; set; }
    }
}