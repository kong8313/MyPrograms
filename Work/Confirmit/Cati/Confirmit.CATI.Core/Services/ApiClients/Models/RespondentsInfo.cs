using System.Collections.Generic;
using Newtonsoft.Json;

namespace Confirmit.CATI.Core.Services.ApiClients.Models
{
    public class RespondentsInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("values")]
        public Dictionary<string, object> Values { get; set; }
        [JsonProperty("links")]
        public Dictionary<string, string> Links { get; set; }
    }
}
