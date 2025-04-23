using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace FN.Utilities
{
    public class StringHelper
    {
        public static bool IsNullOrEmpty(params string[] values)
        {
            foreach (var value in values)
            {
                if (string.IsNullOrEmpty(value))
                {
                    return true;
                }
            }
            return false;
        }
        public static string GenerateSeoAlias(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            input = input.Trim();

            input = input.Replace("–", "-").Replace("—", "-");

            input = Regex.Replace(input, @"[^\w\s-]", "");
            input = Regex.Replace(input, @"\s+", "-"); // mọi khoảng trắng thành 1 dấu -

            string normalized = input.Normalize(NormalizationForm.FormD);
            Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
            string result = regex.Replace(normalized, string.Empty)
                                 .Replace('đ', 'd')
                                 .Replace('Đ', 'D');

            result = Regex.Replace(result, @"-+", "-");

            result = result.Trim('-');

            return result.ToLower();
        }


        public static string GenerateProductCode(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            string codePrefix = GeneratePrefix(input);
            string codeNumbers = ExtractNumbers(input);
            string randomSuffix = Guid.NewGuid().ToString().Substring(0, 6);
            return $"{codePrefix}{codeNumbers}-{randomSuffix}".ToUpperInvariant();
        }
        public static string NormalizeString(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            string normalizedString = input.ToLowerInvariant();
            normalizedString = RemoveDiacritics(normalizedString);
            normalizedString = Regex.Replace(normalizedString, @"\s+", " ");

            return normalizedString;
        }
        static string GeneratePrefix(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            string normalized = RemoveDiacritics(input);
            string[] words = normalized.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder prefix = new StringBuilder();
            foreach (string word in words)
            {
                if (char.IsLetter(word[0]))
                    prefix.Append(word[0]);
            }
            return prefix.Length > 3 ? prefix.ToString().Substring(0, 3) : prefix.ToString();
        }
        static string ExtractNumbers(string input)
        {
            Match match = Regex.Match(input, @"\d+");
            return match.Success ? match.Value : "00";
        }

        static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
