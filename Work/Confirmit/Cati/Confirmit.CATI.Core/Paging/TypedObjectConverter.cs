using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Confirmit.CATI.Core.Paging
{
    public class TypedObjectConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => objectType == typeof(object);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartObject();

            writer.WritePropertyName("$type");
            writer.WriteValue(value.GetType().FullName);
            
            writer.WritePropertyName("$value");
            serializer.Serialize(writer, value);

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            JObject obj = JObject.Load(reader);

            var typeName = obj["$type"]?.ToString();
            var valueToken = obj["$value"];

            if (typeName == null || valueToken == null)
                throw new JsonSerializationException("Invalid typed value");

            var type = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .First(t => t.FullName == typeName);

            return valueToken.ToObject(type, serializer);
        }
    }
}