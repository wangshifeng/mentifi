using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Hub3c.Mentify.Core.Extensions
{
    public static class StringExtensions
    {
        public static bool IsUpperCase(this string input)
        {
            return Regex.IsMatch(input, @"^[A-Z]+$");
        }

        public static string MakeInitialLowerCase(this string input)
        {
            return string.Concat(input.Substring(0, 1).ToLower(), input.Substring(1));
        }

        public static string ToPascalCase(this string input, bool removeUnderscores, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(input)) return input;

            input = input.Replace("_", " ");
            var joinString = removeUnderscores ? string.Empty : "_";
            var words = input.Split(' ');
            if (words.Length > 1 || words[0].IsUpperCase())
                for (var i = 0; i < words.Length; i++)
                {
                    var word = words[i];
                    if (word.Length <= 0) continue;

                    var restOfWord = word.Substring(1);
                    if (restOfWord.IsUpperCase()) restOfWord = restOfWord.ToLower(culture);

                    var firstChar = char.ToUpper(word[0], culture);

                    words[i] = string.Concat(firstChar, restOfWord);
                }

            return string.Join(joinString, words);
        }

        public static string ToPascalCase(this string input, CultureInfo culture)
        {
            return ToPascalCase(input, true, culture);
        }

        public static string ToCamelCase(this string input, CultureInfo culture)
        {
            return MakeInitialLowerCase(ToPascalCase(input, culture));
        }

        public static string RemoveUnderscoresAndDashes(this string input)
        {
            return input.Replace("_", "").Replace("-", "");
        }

        public static string AddUnderscores(this string input)
        {
            return Regex.Replace(
                Regex.Replace(
                    Regex.Replace(input, @"([A-Z]+)([A-Z][a-z])", "$1_$2"),
                    @"([a-z\d])([A-Z])",
                    "$1_$2"),
                @"[-\s]",
                "_");
        }

        public static string AddDashes(this string input)
        {
            return Regex.Replace(
                Regex.Replace(
                    Regex.Replace(input, @"([A-Z]+)([A-Z][a-z])", "$1-$2"),
                    @"([a-z\d])([A-Z])",
                    "$1-$2"),
                @"[\s]",
                "-");
        }

        public static string AddSpaces(this string input)
        {
            return Regex.Replace(
                Regex.Replace(
                    Regex.Replace(input, @"([A-Z]+)([A-Z][a-z])", "$1 $2"),
                    @"([a-z\d])([A-Z])",
                    "$1 $2"),
                @"[-\s]",
                " ");
        }

        public static string AddUnderscorePrefix(this string input)
        {
            return $"_{input}";
        }

        public static IEnumerable<string> GetNameVariants(this string name, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(name)) yield break;

            yield return name;
            yield return name.ToCamelCase(culture);
            yield return name.ToLower(culture);
            yield return name.AddUnderscores();
            yield return name.AddUnderscores().ToLower(culture);
            yield return name.AddDashes();
            yield return name.AddDashes().ToLower(culture);
            yield return name.AddUnderscorePrefix();
            yield return name.ToCamelCase(culture).AddUnderscorePrefix();
            yield return name.AddSpaces();
            yield return name.AddSpaces().ToLower(culture);
        }

        public static string UrlEncode(this string input)
        {
            const int maxLength = 32766;

            if (input == null) throw new ArgumentNullException(nameof(input));
            if (input.Length <= maxLength) return Uri.EscapeDataString(input);

            var sb = new StringBuilder(input.Length * 2);
            var index = 0;
            while (index < input.Length)
            {
                var length = Math.Min(input.Length - index, maxLength);
                var subString = input.Substring(index, length);
                sb.Append(Uri.EscapeDataString(subString));
                index += subString.Length;
            }

            return sb.ToString();
        }

        public static string UrlDecode(this string input)
        {
            return HttpUtility.UrlDecode(input);
        }
    }
}