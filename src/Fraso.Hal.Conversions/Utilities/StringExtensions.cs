using System.Text.RegularExpressions;

namespace Fraso.Hal.Conversions
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string str)
        {
            str = str.Replace("_", "");

            if (str.Length == 0)
                return string.Empty;

            str =
                Regex.Replace(
                    str,
                    "([A-Z])([A-Z]+)($|[A-Z])",
                    m => m.Groups[1].Value
                        + m.Groups[2].Value.ToLower()
                        + m.Groups[3].Value);

            return
                char.ToLower(str[0])
                    + str.Substring(1);
        }
    }
}
