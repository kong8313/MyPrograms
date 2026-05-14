using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace Confirmit.CATI.Core.Logger
{
    public static class LoggingExtension
    {
        public static string ToYaml(this object obj)
        {
            var serializer = new SerializerBuilder().EmitDefaults().Build();
            var yaml = serializer.Serialize(obj);

            return yaml;
        }

        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}