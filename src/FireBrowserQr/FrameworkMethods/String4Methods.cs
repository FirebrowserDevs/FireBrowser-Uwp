using System;

namespace FireBrowserQr.FrameworkMethods
{
    internal static class String40Methods
    {
        
        public static bool IsNullOrWhiteSpace(string? value) // Note the nullable reference type annotation
        {
           return value is null || value.Trim() == string.Empty;
        }


        public static string ReverseString(string str)
        {
           char[] chars = str.ToCharArray();
           return new string(chars[^1..^str.Length - 1]);
        }


        public static bool IsAllDigit(string str)
        {
           return str.All(char.IsDigit);
        }

    }
}
