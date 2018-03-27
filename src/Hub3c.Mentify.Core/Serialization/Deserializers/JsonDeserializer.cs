using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Hub3c.Mentify.Core.Serialization.Deserializers
{
    public class JsonDeserializer : IDeserializer
    {
        public T Deserialize<T>(string content)
        {
            if (string.IsNullOrEmpty(content)) return default(T);

            var settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
            settings.Converters.Add(new StringEnumConverter());

            return Deserialize<T>(content, settings);
        }

        public T Deserialize<T>(string content, JsonSerializerSettings settings)
        {
            return JsonConvert.DeserializeObject<T>(content, settings);
        }
    }
}