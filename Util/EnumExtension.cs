using System.Reflection;

namespace FillableFormWebApp.Util
{
    public static class EnumExtension
    {
        /// <summary>
        /// Gets the value of the StringValueAttribute
        /// </summary>
        /// <param name="value"></param>
        /// <returns>String</returns>
        public static string? GetStringValue(this Enum value)
        {
            FieldInfo? fieldInfo = value.GetType().GetField(value.ToString());

            StringValueAttribute[]? attributes = fieldInfo.GetCustomAttributes(
                typeof(StringValueAttribute), false) as StringValueAttribute[];

            return attributes.Length > 0 ? attributes[0].StringValue : null;
        }
    }
}
