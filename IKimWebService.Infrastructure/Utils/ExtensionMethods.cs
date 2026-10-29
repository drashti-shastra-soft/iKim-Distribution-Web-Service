using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IKimWebService.Infrastructure.Utils
{
    public static class ExtensionMethods
    {
        public static bool IsNotNullOrEmpty(this string inputtedString)
        {
            bool hasValue = false;
            if (!string.IsNullOrEmpty(inputtedString) && !string.IsNullOrWhiteSpace(inputtedString))
                hasValue = true;
            return hasValue;
        }

        public static bool IsNullOrEmpty(this string inputtedString)
        {
            bool hasValue = false;
            if (string.IsNullOrEmpty(inputtedString) || string.IsNullOrWhiteSpace(inputtedString))
                hasValue = true;
            return hasValue;
        }

        public static string StrToUpper(this string inputtedString)
        {
            if (!string.IsNullOrEmpty(inputtedString) && !string.IsNullOrWhiteSpace(inputtedString))
                inputtedString = inputtedString.Trim().ToUpper();
            return inputtedString;
        }

        public static string StrToLower(this string inputtedString)
        {
            if (!string.IsNullOrEmpty(inputtedString) && !string.IsNullOrWhiteSpace(inputtedString))
                inputtedString = inputtedString.Trim().ToLower();
            return inputtedString;
        }

        public static string ToEnumString(this Enum enumValue)
        {
            string value = string.Empty;
            if (enumValue != null && enumValue is Enum)
            {
                value = Convert.ToString(enumValue).Replace("_", " ");
            }
            return value;
        }

        public static string RemoveHindiChars(this string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            // Regex removes all Devanagari characters
            return Regex.Replace(input, @"[\u0900-\u097F]+", "");
        }
    }
}
