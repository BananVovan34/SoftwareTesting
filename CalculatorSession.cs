using System.Globalization;

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
            string separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            string normalized = text.Trim().Replace(",", separator).Replace(".", separator);
            return decimal.TryParse(normalized, NumberStyles.Number,
                CultureInfo.CurrentCulture, out number);
        }
    }
}
