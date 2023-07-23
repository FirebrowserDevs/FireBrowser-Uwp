using System;

namespace FireBrowserQr.Extensions
{
// trying something
    public record StringValueAttribute(string StringValue);

    public static class CustomExtensions
    {
    /// <summary>
    /// Get the string value for a given enum's value.
    /// </summary>
    /// <param name="value">The enum value.</param>
    /// <returns>The string value of the enum.</returns>
        public static string? GetStringValue(this Enum value)
        {
           var fieldInfo = value.GetType().GetField(value.ToString());

           if (fieldInfo?.GetCustomAttribute<StringValueAttribute>() is { } attr)
           {
             return attr.StringValue;
           }

           return null;
        }
    }
}
