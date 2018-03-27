using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Hub3c.Mentify.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static IDictionary<string, string> ToKeyValue(this object metaToken, CultureInfo culture)
        {
            if (metaToken == null) return null;

            var token = metaToken as JToken;
            if (token == null) return ToKeyValue(JObject.FromObject(metaToken), culture);

            if (token.HasValues)
            {
                var contentData = new Dictionary<string, string>();
                foreach (var child in token.Children().ToList())
                {
                    var childContent = child.ToKeyValue(culture);
                    if (childContent != null)
                        contentData = contentData.Concat(childContent).ToDictionary(k => k.Key, v => v.Value);
                }

                return contentData;
            }

            var jValue = token as JValue;
            if (jValue?.Value == null) return null;

            var value = jValue?.Type == JTokenType.Date
                ? jValue?.ToString("o", culture)
                : jValue?.ToString(culture);

            return new Dictionary<string, string> {{token.Path, value}};
        }
    }
}