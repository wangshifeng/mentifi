using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using Hub3c.Mentify.Core.Extensions;

namespace Hub3c.Mentify.Core.Serialization.Deserializers
{
    public class XmlDeserializer : IDeserializer
    {
        public XmlDeserializer()
        {
            Culture = CultureInfo.InvariantCulture;
        }

        public string RootElement { get; set; }
        public string Namespace { get; set; }
        public string DateFormat { get; set; }
        public CultureInfo Culture { get; set; }

        public T Deserialize<T>(string content)
        {
            if (string.IsNullOrEmpty(content)) return default(T);

            var doc = XDocument.Parse(content);
            var root = doc.Root;

            if (!string.IsNullOrEmpty(RootElement) && doc.Root != null)
                root = doc.Root.DescendantsAndSelf(RootElement.AsNamespaced(Namespace)).SingleOrDefault();

            if (!string.IsNullOrEmpty(Namespace))
                doc.RemoveNamespace();

            var result = Activator.CreateInstance<T>();
            var resultType = result.GetType();

            if (resultType.IsSubclassOfRawGeneric(typeof(List<>)))
                result = (T) HandleListDerivative(root, resultType);

            return result;
        }

        private XAttribute GetAttributeByName(XElement root, XName name)
        {
            var names = new List<XName>
            {
                name.LocalName,
                name.LocalName.ToLower().AsNamespaced(name.NamespaceName),
                name.LocalName.ToCamelCase(Culture).AsNamespaced(name.NamespaceName)
            };

            return root.DescendantsAndSelf()
                .OrderBy(d => d.Ancestors().Count())
                .Attributes()
                .FirstOrDefault(d => names.Contains(d.Name.LocalName.RemoveUnderscoresAndDashes()));
        }

        private XElement GetElementByName(XElement root, XName name)
        {
            var lowerCaseName = name.LocalName.ToLower().AsNamespaced(name.NamespaceName);
            var camelCaseName = name.LocalName.ToCamelCase(Culture).AsNamespaced(name.NamespaceName);

            if (root.Element(name) != null) return root.Element(name);
            if (root.Element(lowerCaseName) != null) return root.Element(lowerCaseName);
            if (root.Element(camelCaseName) != null) return root.Element(camelCaseName);
            if (name == "Value".AsNamespaced(name.NamespaceName) &&
                (!root.HasAttributes || root.Attributes().All(a => a.Name != name))) return root;

            var element = root.Descendants()
                .OrderBy(e => e.Ancestors().Count())
                .FirstOrDefault(e => e.Name.LocalName.RemoveUnderscoresAndDashes() == name.LocalName);
            if (element == null)
                element = root.Descendants()
                    .OrderBy(e => e.Ancestors().Count())
                    .FirstOrDefault(e => e.Name.LocalName.RemoveUnderscoresAndDashes() == name.LocalName.ToLower());

            return element;
        }

        private object GetValueFromXml(XElement root, XName name, PropertyInfo property)
        {
            if (root == null) return null;

            object value = null;
            var element = GetElementByName(root, name);
            if (element == null)
            {
                var attribute = GetAttributeByName(root, name);
                if (attribute != null) value = attribute.Value;
            }
            else if (!element.IsEmpty || element.HasElements || element.HasAttributes)
            {
                value = element.Value;
            }

            return value;
        }

        private bool TryGetFromString(string input, out object result, Type type)
        {
            var converter = TypeDescriptor.GetConverter(type);
            if (converter.CanConvertFrom(typeof(string)))
            {
                result = converter.ConvertFromInvariantString(input);
                return true;
            }

            result = null;
            return false;
        }

        private object Map(object obj, XElement root)
        {
            var objType = obj.GetType();
            var props = objType.GetProperties();

            foreach (var prop in props)
            {
                var propType = prop.GetType();
                if (propType.IsPublic || propType.IsNestedPublic || !prop.CanWrite) continue;

                XName name;
                var attributes = prop.GetCustomAttributes(typeof(XmlDeserializeAttribute), false);
                if (attributes.Length > 0)
                {
                    var attribute = (XmlDeserializeAttribute) attributes.First();
                    name = attribute.Name.AsNamespaced(Namespace);
                }
                else
                {
                    name = prop.Name.AsNamespaced(Namespace);
                }

                var value = GetValueFromXml(root, name, prop);
                if (value == null)
                {
                    if (propType.IsGenericType)
                    {
                        var genericType = propType.GetGenericArguments()[0];
                        var firstElement = GetElementByName(root, genericType.Name);
                        var list = (IList) Activator.CreateInstance(propType);
                        if (firstElement != null && root != null)
                        {
                            var elements = root.Elements(firstElement.Name);
                            PopulateListFromElements(genericType, elements, list);
                        }

                        prop.SetValue(obj, list, null);
                    }

                    continue;
                }

                if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    if (string.IsNullOrEmpty(value.ToString()))
                    {
                        prop.SetValue(obj, null, null);
                        continue;
                    }

                    propType = propType.GetGenericArguments()[0];
                }

                if (propType == typeof(bool))
                {
                    var strVal = value.ToString().ToLower();
                    prop.SetValue(obj, XmlConvert.ToBoolean(strVal), null);
                }
                else if (propType.IsPrimitive)
                {
                    prop.SetValue(obj, Convert.ChangeType(value, propType, Culture));
                }
                else if (propType.IsEnum)
                {
                    var enumVal = propType.FindEnumValue(value.ToString(), Culture);
                    prop.SetValue(obj, enumVal, null);
                }
                else if (propType == typeof(Uri))
                {
                    var uri = new Uri(value.ToString(), UriKind.RelativeOrAbsolute);
                    prop.SetValue(obj, uri, null);
                }
                else if (propType == typeof(string))
                {
                    prop.SetValue(obj, value, null);
                }
                else if (propType == typeof(DateTime))
                {
                    value = !string.IsNullOrEmpty(DateFormat)
                        ? DateTime.ParseExact(value.ToString(), DateFormat, Culture)
                        : DateTime.Parse(value.ToString(), Culture);
                }
                else if (propType == typeof(DateTimeOffset))
                {
                    var strVal = value.ToString();
                    if (!string.IsNullOrEmpty(strVal))
                    {
                        DateTimeOffset deserializedValue;
                        try
                        {
                            deserializedValue = XmlConvert.ToDateTimeOffset(strVal);
                            prop.SetValue(obj, deserializedValue, null);
                        }
                        catch (Exception)
                        {
                            if (TryGetFromString(strVal, out var result, propType))
                            {
                                prop.SetValue(obj, result, null);
                            }
                            else
                            {
                                deserializedValue = DateTimeOffset.Parse(strVal);
                                prop.SetValue(obj, deserializedValue, null);
                            }
                        }
                    }
                }
                else if (propType == typeof(decimal))
                {
                    value = decimal.Parse(value.ToString(), Culture);
                    prop.SetValue(obj, value, null);
                }
                else if (propType == typeof(Guid))
                {
                    var strVal = value.ToString();
                    value = string.IsNullOrEmpty(strVal)
                        ? Guid.Empty
                        : new Guid(value.ToString());
                    prop.SetValue(obj, value, null);
                }
                else if (propType == typeof(TimeSpan))
                {
                    var timespan = XmlConvert.ToTimeSpan(value.ToString());
                    prop.SetValue(obj, timespan, null);
                }
                else if (propType.IsGenericType)
                {
                    var genericType = propType.GetGenericArguments()[0];
                    var list = (IList) Activator.CreateInstance(propType);
                    var container = GetElementByName(root, prop.Name.AsNamespaced(Namespace));
                    if (container.HasElements)
                    {
                        var firstElement = container.Elements().FirstOrDefault();
                        if (firstElement != null)
                        {
                            var elements = container.Elements(firstElement.Name);
                            PopulateListFromElements(genericType, elements, list);
                        }
                    }

                    prop.SetValue(obj, list, null);
                }
                else if (propType.IsSubclassOfRawGeneric(typeof(List<>)))
                {
                    var list = HandleListDerivative(root, propType);
                    prop.SetValue(obj, list, null);
                }
                else
                {
                    if (TryGetFromString(value.ToString(), out var result, propType))
                    {
                        prop.SetValue(obj, result, null);
                    }
                    else if (root != null)
                    {
                        var element = GetElementByName(root, name);
                        if (element != null)
                        {
                            var item = CreateAndMap(propType, element);
                            prop.SetValue(obj, item, null);
                        }
                    }
                }
            }

            return obj;
        }

        private object CreateAndMap(Type type, XElement element)
        {
            object item;
            if (type == typeof(string))
            {
                item = element.Value;
            }
            else if (type.IsPrimitive)
            {
                item = Convert.ChangeType(element.Value, type, Culture);
            }
            else
            {
                item = Activator.CreateInstance(type);
                Map(item, element);
            }

            return item;
        }

        private void PopulateListFromElements(Type type, IEnumerable<XElement> elements, IList list)
        {
            foreach (var item in elements.Select(e => CreateAndMap(type, e))) list.Add(item);
        }

        private object HandleListDerivative(XElement root, Type type)
        {
            var genericType = type.IsGenericType
                ? type.GetGenericArguments()[0]
                : type.BaseType.GetGenericArguments()[0];

            var genericTypeName = genericType.Name;
            if (Attribute.GetCustomAttribute(genericType, typeof(XmlDeserializeAttribute)) is XmlDeserializeAttribute
                attribute)
                genericTypeName = attribute.Name;

            var elements = root.Descendants(genericTypeName.AsNamespaced(Namespace)).ToList();
            var lowerCaseName = genericTypeName.ToLower().AsNamespaced(Namespace);
            var camelCaseName = genericTypeName.ToCamelCase(Culture).AsNamespaced(Namespace);

            if (!elements.Any())
                elements = root.Descendants(lowerCaseName).ToList();

            if (!elements.Any())
                elements = root.Descendants(camelCaseName).ToList();

            if (!elements.Any())
                elements = root.Descendants()
                    .Where(e => e.Name.LocalName.RemoveUnderscoresAndDashes() == genericTypeName)
                    .ToList();

            if (!elements.Any())
                elements = root.Descendants()
                    .Where(e => e.Name.LocalName.RemoveUnderscoresAndDashes() == lowerCaseName)
                    .ToList();

            var list = (IList) Activator.CreateInstance(type);
            PopulateListFromElements(genericType, elements, list);

            if (!type.IsGenericType)
                Map(list, root.Element(type.Name.AsNamespaced(Namespace)) ?? root);

            return list;
        }
    }
}