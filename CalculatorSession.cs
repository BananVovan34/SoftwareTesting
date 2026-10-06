using System.Globalization;
using System.Text.RegularExpressions;

namespace SoftwareTesting
{
    public static class CalculatorSession
    {
        public static bool TryCalculate(string firstText, string secondText, string operation,
            out string result, out string error)
        {
            result = string.Empty;
            error = string.Empty;

            if (!TryReadNumber(firstText, out decimal firstNumber) ||
                !TryReadNumber(secondText, out decimal secondNumber))
            {
                error = "Введите два числа.";
                return false;
            }

            try
            {
                decimal value = Calculator.Calculate(firstNumber, secondNumber, operation);
                result = value.ToString("G29", CultureInfo.CurrentCulture);
                return true;
            }
            catch (DivideByZeroException)
            {
                error = "На ноль делить нельзя.";
            }
            catch (OverflowException)
            {
                error = "Результат слишком велик.";
            }
            catch (ArgumentException)
            {
                error = "Выберите операцию.";
            }

            return false;
        }

        private static bool TryReadNumber(string text, out decimal number)
        {
            string input = text.Trim();

            if (input.Contains(',') && input.Contains('.'))
            {
                bool commaIsGroup = input.LastIndexOf(',') < input.LastIndexOf('.');
                string pattern = commaIsGroup
                    ? @"^[+-]?[0-9]{1,3}(?:,[0-9]{3})+\.[0-9]+$"
                    : @"^[+-]?[0-9]{1,3}(?:\.[0-9]{3})+,[0-9]+$";

                if (!Regex.IsMatch(input, pattern))
                {
                    number = 0;
                    return false;
                }

                string normalizedGroup = input.Replace(commaIsGroup ? "," : ".", string.Empty)
                    .Replace(",", ".");
                return decimal.TryParse(normalizedGroup,
                    NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture, out number);
            }

            string separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            string normalized = input.Replace(",", separator).Replace(".", separator);
            return decimal.TryParse(normalized, NumberStyles.Number,
                CultureInfo.CurrentCulture, out number);
        }
    }
}
