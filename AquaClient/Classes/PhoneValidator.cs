using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AquaClient.Classes
{
    public static class PhoneValidator
    {
        // Строгий паттерн: +7 и ровно 10 цифр после
        private static readonly Regex _strictPhoneRegex = new Regex(
            @"^\+7\d{10}$",
            RegexOptions.Compiled);

        // Паттерн для проверки цифр (без +7 в начале)
        private static readonly Regex _digitsOnlyRegex = new Regex(
            @"^\d{10}$",
            RegexOptions.Compiled);

        /// <summary>
        /// Проверяет строгий формат: +7XXXXXXXXXX
        /// </summary>
        public static bool IsValidStrictPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            return _strictPhoneRegex.IsMatch(phone);
        }

        /// <summary>
        /// Конвертирует разные форматы в строгий формат +7XXXXXXXXXX
        /// </summary>
        public static string ConvertToStrictFormat(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return string.Empty;

            // Удаляем всё, кроме цифр
            string digitsOnly = Regex.Replace(phone, @"[^\d]", "");

            // Если 11 цифр и начинается с 7 или 8
            if (digitsOnly.Length == 11)
            {
                if (digitsOnly[0] == '8')
                    return "+7" + digitsOnly.Substring(1);
                else if (digitsOnly[0] == '7')
                    return "+" + digitsOnly;
            }
            // Если 10 цифр (без кода страны)
            else if (digitsOnly.Length == 10)
            {
                return "+7" + digitsOnly;
            }

            return phone; // Не удалось конвертировать
        }

        /// <summary>
        /// Проверяет и конвертирует в строгий формат
        /// </summary>
        public static (bool IsValid, string StrictFormat) ValidateAndConvert(string phone)
        {
            // Если уже в строгом формате
            if (IsValidStrictPhone(phone))
                return (true, phone);

            // Пробуем конвертировать
            string strictFormat = ConvertToStrictFormat(phone);

            // Проверяем результат
            bool isValid = IsValidStrictPhone(strictFormat);

            return (isValid, isValid ? strictFormat : phone);
        }
    }
}
