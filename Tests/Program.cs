using System.Globalization;
using SoftwareTesting;

namespace SoftwareTestingTests
{
    internal static class Program
    {
        private static int passed;

        [STAThread]
        private static void Main()
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");

            Check("Модуль: сложение", () => Equal(5m, Calculator.Calculate(2m, 3m, "+")));
            Check("Модуль: вычитание отрицательных чисел", () => Equal(-2m, Calculator.Calculate(-5m, -3m, "-")));
            Check("Модуль: умножение дробей", () => Equal(3m, Calculator.Calculate(1.5m, 2m, "×")));
            Check("Модуль: деление", () => Equal(2.5m, Calculator.Calculate(5m, 2m, "÷")));
            Check("Модуль: деление на ноль", () => Throws<DivideByZeroException>(() => Calculator.Calculate(5m, 0m, "÷")));
            Check("Модуль: неизвестная операция", () => Throws<ArgumentException>(() => Calculator.Calculate(1m, 2m, "?")));
            Check("Модуль: переполнение", () => Throws<OverflowException>(() => Calculator.Calculate(decimal.MaxValue, 1m, "+")));

            Check("Интеграция: дроби с запятой", () => Session("1,5", "2", "×", "3", ""));
            Check("Интеграция: дроби с точкой", () => Session("1.5", "2", "×", "3", ""));
            Check("Интеграция: пустое поле", () => Session("", "2", "+", "", "Введите два числа."));
            Check("Интеграция: деление на ноль", () => Session("5", "0", "÷", "", "На ноль делить нельзя."));
            Check("Интеграция: переполнение", () => Session(decimal.MaxValue.ToString(), "1", "+", "", "Результат слишком велик."));
            Check("Интеграция: неверный формат", () => Session("abc", "2", "+", "", "Введите два числа."));

            Check("Система: расчёт через форму", FormCalculation);
            Check("Система: ошибка удаляет старый результат", FormError);
            Check("Система: очистка возвращает начальное состояние", FormClear);

            Console.WriteLine($"Пройдено проверок: {passed}");
        }

        private static void Check(string name, Action test)
        {
            test();
            passed++;
            Console.WriteLine($"OK {name}");
        }

        private static void Equal<T>(T expected, T actual)
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
            {
                throw new Exception($"Ожидалось: {expected}; получено: {actual}");
            }
        }

        private static void Throws<T>(Action action) where T : Exception
        {
            try
            {
                action();
            }
            catch (T)
            {
                return;
            }

            throw new Exception($"Ожидалось исключение {typeof(T).Name}");
        }

        private static void Session(string first, string second, string operation, string result, string error)
        {
            bool success = CalculatorSession.TryCalculate(first, second, operation,
                out string actualResult, out string actualError);
            Equal(error.Length == 0, success);
            Equal(result, actualResult);
            Equal(error, actualError);
        }

        private static T Control<T>(Form form, string name) where T : Control
        {
            return (T)form.Controls.Find(name, true).Single();
        }

        private static void FormCalculation()
        {
            using Form1 form = new Form1();
            form.Show();
            Control<ComboBox>(form, "operationComboBox").SelectedItem = "×";
            Control<TextBox>(form, "firstNumberTextBox").Text = "2,5";
            Control<TextBox>(form, "secondNumberTextBox").Text = "4";
            Control<Button>(form, "calculateButton").PerformClick();
            Equal("10", Control<TextBox>(form, "resultTextBox").Text);
        }

        private static void FormError()
        {
            using Form1 form = new Form1();
            form.Show();
            Control<TextBox>(form, "firstNumberTextBox").Text = "5";
            Control<TextBox>(form, "secondNumberTextBox").Text = "2";
            Control<Button>(form, "calculateButton").PerformClick();
            Control<ComboBox>(form, "operationComboBox").SelectedItem = "÷";
            Control<TextBox>(form, "secondNumberTextBox").Text = "0";
            Control<Button>(form, "calculateButton").PerformClick();
            Equal("", Control<TextBox>(form, "resultTextBox").Text);
            Equal("На ноль делить нельзя.", Control<Label>(form, "errorLabel").Text);
        }

        private static void FormClear()
        {
            using Form1 form = new Form1();
            form.Show();
            Control<TextBox>(form, "firstNumberTextBox").Text = "5";
            Control<TextBox>(form, "secondNumberTextBox").Text = "0";
            Control<ComboBox>(form, "operationComboBox").SelectedItem = "÷";
            Control<Button>(form, "calculateButton").PerformClick();
            Control<Button>(form, "clearButton").PerformClick();
            Equal("", Control<TextBox>(form, "firstNumberTextBox").Text);
            Equal("", Control<TextBox>(form, "secondNumberTextBox").Text);
            Equal("", Control<TextBox>(form, "resultTextBox").Text);
            Equal("", Control<Label>(form, "errorLabel").Text);
            Equal("+", Control<ComboBox>(form, "operationComboBox").Text);
        }
    }
}
