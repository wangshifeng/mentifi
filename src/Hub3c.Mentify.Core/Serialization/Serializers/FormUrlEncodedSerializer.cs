using System.Globalization;
using System.Net.Http;
using Hub3c.Mentify.Core.Extensions;

namespace Hub3c.Mentify.Core.Serialization.Serializers
{
    public class FormUrlEncodedSerializer : ISerializer
    {
        public FormUrlEncodedSerializer()
        {
            Culture = CultureInfo.InvariantCulture;
        }

        public CultureInfo Culture { get; set; }

        public string Serialize(object obj)
        {
            var keyValueContent = obj.ToKeyValue(Culture);
            var formUrlEncodedContent = new FormUrlEncodedContent(keyValueContent);
            return formUrlEncodedContent.ReadAsStringAsync().Result;
        }
    }
}