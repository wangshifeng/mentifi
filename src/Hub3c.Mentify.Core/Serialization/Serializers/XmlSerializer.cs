using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Hub3c.Mentify.Core.Extensions;

namespace Hub3c.Mentify.Core.Serialization.Serializers
{
    public class XmlSerializer : ISerializer
    {
        public XmlSerializer()
        {
        }

        public XmlSerializer(string @namespace)
        {
            Namespace = @namespace;
        }

        public string RootElement { get; set; }
        public string Namespace { get; set; }
        public string DateFormat { get; set; }

        public string Serialize(object obj)
        {
            var doc = new XDocument();
            var t = obj.GetType();
            var name = t.Name;
            var options = t.GetAttribute<XmlSerializeAttribute>();
            if (options != null) name = options.TransformName(options.Name ?? name);

            var root = new XElement(name.AsNamespaced(Namespace));
            if (obj is IList list)
            {
                var itemTypeName = "";

                foreach (var item in list)
                {
                    var type = item.GetType();
                    var opts = type.GetAttribute<XmlSerializeAttribute>();

                    if (opts != null)
                        itemTypeName = opts.TransformName(opts.Name ?? name);

                    if (itemTypeName == "")
                        itemTypeName = type.Name;

                    var instance = new XElement(itemTypeName.AsNamespaced(Namespace));

                    Map(instance, item);
                    root.Add(instance);
                }
            }
            else
            {
                Map(root, obj);
            }

            if (!string.IsNullOrEmpty(RootElement))
            {
                var wrapper = new XElement(RootElement.AsNamespaced(Namespace), root);
                doc.Add(wrapper);
            }
            else
            {
                doc.Add(root);
            }

            return doc.ToString();
        }

        private void Map(XContainer root, object obj)
        {
            var objType = obj.GetType();
            var props = from p in objType.GetTypeInfo().GetProperties()
                let indexAttribute = p.GetAttribute<XmlSerializeAttribute>()
                where p.CanRead && p.CanWrite
                orderby indexAttribute?.Index ?? int.MaxValue
                select p;
            var globalOptions = objType.GetAttribute<XmlSerializeAttribute>();

            foreach (var prop in props)
            {
                var name = prop.Name;
                var rawValue = prop.GetValue(obj, null);

                if (rawValue == null)
                    continue;

                var value = GetSerializedValue(rawValue);
                var propType = prop.PropertyType;
                var useAttribute = false;
                var settings = prop.GetAttribute<XmlSerializeAttribute>();

                if (settings != null)
                {
                    name = !string.IsNullOrEmpty(settings.Name)
                        ? settings.Name
                        : name;
                    useAttribute = settings.Attribute;
                }

                var options = prop.GetAttribute<XmlSerializeAttribute>();

                if (options != null)
                    name = options.TransformName(name);
                else if (globalOptions != null)
                    name = globalOptions.TransformName(name);

                var nsName = name.AsNamespaced(Namespace);
                var element = new XElement(nsName);
                if (propType.GetTypeInfo().IsPrimitive || propType.GetTypeInfo().IsValueType ||
                    propType == typeof(string))
                {
                    if (useAttribute)
                    {
                        root.Add(new XAttribute(name, value));
                        continue;
                    }

                    element.Value = value;
                }
                else if (rawValue is IList)
                {
                    var itemTypeName = "";

                    foreach (var item in (IList) rawValue)
                    {
                        if (itemTypeName == "")
                        {
                            var type = item.GetType();
                            var setting = type.GetAttribute<XmlSerializeAttribute>();

                            itemTypeName = setting != null && !string.IsNullOrEmpty(setting.Name)
                                ? setting.Name
                                : type.Name;
                        }

                        var instance = new XElement(itemTypeName.AsNamespaced(Namespace));

                        Map(instance, item);
                        element.Add(instance);
                    }
                }
                else
                {
                    Map(element, rawValue);
                }

                root.Add(element);
            }
        }

        private string GetSerializedValue(object obj)
        {
            var output = obj;

            switch (obj)
            {
                case DateTime time when !string.IsNullOrEmpty(DateFormat):
                    output = time.ToString(DateFormat, CultureInfo.InvariantCulture);
                    break;
                case bool _:
                    output = ((bool) obj).ToString().ToLowerInvariant();
                    break;
            }

            return IsNumeric(obj) ? SerializeNumber(obj) : output.ToString();
        }

        private bool IsNumeric(object value)
        {
            switch (value)
            {
                case sbyte _:
                    return true;
                case byte _:
                    return true;
                case short _:
                    return true;
                case ushort _:
                    return true;
                case int _:
                    return true;
                case uint _:
                    return true;
                case long _:
                    return true;
                case ulong _:
                    return true;
                case float _:
                    return true;
                case double _:
                    return true;
                case decimal _:
                    return true;
            }

            return false;
        }

        private static string SerializeNumber(object number)
        {
            switch (number)
            {
                case long l:
                    return l.ToString(CultureInfo.InvariantCulture);
                case ulong @ulong:
                    return @ulong.ToString(CultureInfo.InvariantCulture);
                case int i:
                    return i.ToString(CultureInfo.InvariantCulture);
                case uint u:
                    return u.ToString(CultureInfo.InvariantCulture);
                case decimal @decimal:
                    return @decimal.ToString(CultureInfo.InvariantCulture);
                case float f:
                    return f.ToString(CultureInfo.InvariantCulture);
            }

            return Convert.ToDouble(number, CultureInfo.InvariantCulture).ToString("r", CultureInfo.InvariantCulture);
        }
    }
}