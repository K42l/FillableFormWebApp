using Microsoft.IdentityModel.Tokens;

namespace FillableFormWebApp.Util
{
    public static class StringExtension
    {
        /// <summary>
        /// Converts the given string to camelCase
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string? ToCamelCase(this string value, string separator)
        {
            if (!value.Contains(separator))
                throw new Exception("The string doesn't contain the given separator");

            if (value.IsNullOrEmpty() || value.Length < 2) 
                return value.ToLowerInvariant();
            
            value = char.ToLowerInvariant(value[0]) + value.Substring(1);
            string[] valueSplit = value.Split(separator);
            string camelCaseValue = valueSplit[0];
            for (int i = 1; i < valueSplit.Length; i++) 
            {
                camelCaseValue += char.ToUpperInvariant(valueSplit[i][0]) + valueSplit[i].Substring(1);
            }

            return camelCaseValue;
        }
    }
}
