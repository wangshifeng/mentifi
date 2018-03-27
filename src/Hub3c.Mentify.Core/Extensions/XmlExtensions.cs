using System.Linq;
using System.Xml.Linq;

namespace Hub3c.Mentify.Core.Extensions
{
    public static class XmlExtensions
    {
        public static void RemoveNamespace(this XDocument doc)
        {
            if (doc.Root == null) return;

            foreach (var element in doc.Root.DescendantsAndSelf())
            {
                if (element.Name.Namespace != XNamespace.None)
                    element.Name = XNamespace.None.GetName(element.Name.LocalName);

                if (element.Attributes().Any(a => a.IsNamespaceDeclaration || a.Name.Namespace != XNamespace.None))
                {
                    var newAttributes = element.Attributes()
                        .Select(a => a.IsNamespaceDeclaration
                            ? null
                            : a.Name.Namespace != XNamespace.None
                                ? new XAttribute(XNamespace.None.GetName(a.Name.LocalName), a.Value)
                                : a);

                    element.ReplaceAttributes(newAttributes);
                }
            }
        }

        public static XName AsNamespaced(this string name, string @namespace)
        {
            XName xName = name;
            if (!string.IsNullOrEmpty(@namespace)) xName = XName.Get(name, @namespace);

            return xName;
        }
    }
}