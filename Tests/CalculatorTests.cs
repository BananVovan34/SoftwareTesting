using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareTesting;

namespace SoftwareTestingTests
{
    [TestClass]
    public class CalculatorTests
    {
        [TestInitialize]
        public void SetCulture()
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
        }

        [TestMethod]
        public void Addition()
        {
            Assert.AreEqual(5m, Calculator.Calculate(2m, 3m, "+"));
        }

        [TestMethod]
        public void SubtractionOfNegativeNumbers()
        {
            Assert.AreEqual(-2m, Calculator.Calculate(-5m, -3m, "-"));
        }

        [TestMethod]
        public void MultiplicationOfFractions()
        {
            Assert.AreEqual(3m, Calculator.Calculate(1.5m, 2m, "×"));
        }

        [TestMethod]
        public void Division()
        {
            Assert.AreEqual(2.5m, Calculator.Calculate(5m, 2m, "÷"));
        }

        [TestMethod]
        public void DivisionByZero()
        {
            Assert.ThrowsException<DivideByZeroException>(() => Calculator.Calculate(5m, 0m, "÷"));
        }

        [TestMethod]
        public void UnknownOperation()
        {
            Assert.ThrowsException<ArgumentException>(() => Calculator.Calculate(1m, 2m, "?"));
        }

        [TestMethod]
        public void Overflow()
        {
            Assert.ThrowsException<OverflowException>(() => Calculator.Calculate(decimal.MaxValue, 1m, "+"));
        }

        [TestMethod]
        public void MultiplicationWithAsciiSign()
        {
            Assert.AreEqual(12m, Calculator.Calculate(3m, 4m, "*"));
        }

        [TestMethod]
        public void DivisionWithAsciiSign()
        {
            Assert.AreEqual(3m, Calculator.Calculate(12m, 4m, "/"));
        }
    }

    [TestClass]
    public class CalculatorSessionTests
    {
        [TestInitialize]
        public void SetCulture()
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
        }

        [TestMethod]
        public void FractionWithComma()
        {
            Check("1,5", "2", "×", "3", "");
        }

        [TestMethod]
        public void FractionWithDot()
        {
            Check("1.5", "2", "×", "3", "");
        }

        [TestMethod]
        public void EmptyInput()
        {
            Check("", "2", "+", "", "Введите два числа.");
        }

        [TestMethod]
        public void DivisionByZero()
        {
            Check("5", "0", "÷", "", "На ноль делить нельзя.");
        }

        [TestMethod]
        public void Overflow()
        {
            Check(decimal.MaxValue.ToString(), "1", "+", "", "Результат слишком велик.");
        }

        [TestMethod]
        public void InvalidInput()
        {
            Check("abc", "2", "+", "", "Введите два числа.");
        }

        private static void Check(string first, string second, string operation, string result, string error)
        {
            bool success = CalculatorSession.TryCalculate(first, second, operation,
                out string actualResult, out string actualError);
            Assert.AreEqual(error.Length == 0, success);
            Assert.AreEqual(result, actualResult);
            Assert.AreEqual(error, actualError);
        }
    }

    [TestClass]
    public class CalculatorFormTests
    {
        [TestMethod]
        public void CalculationThroughForm()
        {
            RunOnSta(() =>
            {
                using Form1 form = new Form1();
                form.Show();
                Control<ComboBox>(form, "operationComboBox").SelectedItem = "×";
                Control<TextBox>(form, "firstNumberTextBox").Text = "2,5";
                Control<TextBox>(form, "secondNumberTextBox").Text = "4";
                Control<Button>(form, "calculateButton").PerformClick();
                Assert.AreEqual("10", Control<TextBox>(form, "resultTextBox").Text);
            });
        }

        [TestMethod]
        public void ErrorClearsPreviousResult()
        {
            RunOnSta(() =>
            {
                using Form1 form = new Form1();
                form.Show();
                Control<TextBox>(form, "firstNumberTextBox").Text = "5";
                Control<TextBox>(form, "secondNumberTextBox").Text = "2";
                Control<Button>(form, "calculateButton").PerformClick();
                Control<ComboBox>(form, "operationComboBox").SelectedItem = "÷";
                Control<TextBox>(form, "secondNumberTextBox").Text = "0";
                Control<Button>(form, "calculateButton").PerformClick();
                Assert.AreEqual("", Control<TextBox>(form, "resultTextBox").Text);
                Assert.AreEqual("На ноль делить нельзя.", Control<Label>(form, "errorLabel").Text);
            });
        }

        [TestMethod]
        public void ClearRestoresInitialState()
        {
            RunOnSta(() =>
            {
                using Form1 form = new Form1();
                form.Show();
                Control<TextBox>(form, "firstNumberTextBox").Text = "5";
                Control<TextBox>(form, "secondNumberTextBox").Text = "0";
                Control<ComboBox>(form, "operationComboBox").SelectedItem = "÷";
                Control<Button>(form, "calculateButton").PerformClick();
                Control<Button>(form, "clearButton").PerformClick();
                Assert.AreEqual("", Control<TextBox>(form, "firstNumberTextBox").Text);
                Assert.AreEqual("", Control<TextBox>(form, "secondNumberTextBox").Text);
                Assert.AreEqual("", Control<TextBox>(form, "resultTextBox").Text);
                Assert.AreEqual("", Control<Label>(form, "errorLabel").Text);
                Assert.AreEqual("+", Control<ComboBox>(form, "operationComboBox").Text);
            });
        }

        private static T Control<T>(Form form, string name) where T : Control
        {
            return (T)form.Controls.Find(name, true).Single();
        }

        private static void RunOnSta(Action action)
        {
            Exception? error = null;
            Thread thread = new Thread(() =>
            {
                try
                {
                    CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
                    action();
                }
                catch (Exception exception)
                {
                    error = exception;
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (error != null)
            {
                throw error;
            }
        }
    }
}
