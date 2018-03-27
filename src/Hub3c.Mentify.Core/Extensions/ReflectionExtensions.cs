using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Hub3c.Mentify.Core.Extensions
{
    public static class ReflectionExtensions
    {
        public static bool IsSubclassOfRawGeneric(this Type typeToCheck, Type genericType)
        {
            while (typeToCheck != null && typeToCheck != typeof(object))
            {
                var currentType = typeToCheck.IsGenericType
                    ? typeToCheck.GetGenericTypeDefinition()
                    : typeToCheck;

                if (genericType == currentType) return true;

                typeToCheck = currentType.BaseType;
            }

            return false;
        }

        public static object FindEnumValue(this Type type, string value, CultureInfo culture)
        {
            var result = Enum.GetValues(type).Cast<Enum>()
                .FirstOrDefault(v =>
                    v.ToString().GetNameVariants(culture).Contains(value, StringComparer.Create(culture, true)));

            if (result == null)
            {
                var enumValue = Convert.ChangeType(value, Enum.GetUnderlyingType(type), culture);
                if (enumValue != null && Enum.IsDefined(type, enumValue))
                    result = (Enum) Enum.ToObject(type, enumValue);
            }

            return result;
        }

        public static T GetAttribute<T>(this Type type) where T : Attribute
        {
            return Attribute.GetCustomAttribute(type, typeof(T)) as T;
        }

        public static T GetAttribute<T>(this MemberInfo prop) where T : Attribute
        {
            return Attribute.GetCustomAttribute(prop, typeof(T)) as T;
        }
    }
}