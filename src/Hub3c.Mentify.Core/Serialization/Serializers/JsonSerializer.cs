using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Hub3c.Mentify.Core.Serialization.Serializers
{
    public class JsonSerializer : ISerializer
    {
        public string Serialize(object obj)
        {
            if (obj == null) return null;

            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            settings.Converters.Add(new StringEnumConverter(camelCaseText: true));
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            return Serialize(obj, settings);
        }

        public string Serialize(object obj, JsonSerializerSettings settings)
        {
            return JsonConvert.SerializeObject(obj, settings);
        }
    }
}