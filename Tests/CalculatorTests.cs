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
        public void MultiplicationOverflow()
        {
            Assert.ThrowsException<OverflowException>(() => Calculator.Calculate(decimal.MaxValue, 2m, "×"));
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
        public void MultiplicationOverflow()
        {
            Check(decimal.MaxValue.ToString(), "2", "×", "", "Результат слишком велик.");
        }

        [TestMethod]
        public void InvalidInput()
        {
            Check("abc", "2", "+", "", "Введите два числа.");
        }

        [TestMethod]
        public void GroupedNumberWithCommaThousands()
        {
            Check("1,234.5", "1", "+", "1235,5", "");
        }

        [TestMethod]
        public void GroupedNumberWithDotThousands()
        {
            Check("1.234,5", "1", "+", "1235,5", "");
        }

        [TestMethod]
        public void MillionWithDotGroups()
        {
            Check("1.000.000", "2", "+", "1000002", "");
        }

        [TestMethod]
        public void NegativeMillionWithDotGroups()
        {
            Check("-1.000.000", "2", "+", "-999998", "");
        }

        [TestMethod]
        public void InvalidDotGrouping()
        {
            Check("1.00.000", "2", "+", "", "Введите два числа.");
        }

        [TestMethod]
        public void SingleDotRemainsDecimal()
        {
            Check("1.234", "2", "+", "3,234", "");
        }

        [TestMethod]
        public void GroupedInputExceedsDecimalRange()
        {
            Check("100.000.000.000.000.000.000.000.000.000", "0", "+", "", "Введите два числа.");
        }

        [TestMethod]
        public void BelowScientificThreshold()
        {
            Check("999999999999", "0", "+", "999999999999", "");
        }

        [TestMethod]
        public void AtScientificThreshold()
        {
            Check("1000000000000", "0", "+", "≈ 1E+12", "");
        }

        [TestMethod]
        public void NegativeScientificResult()
        {
            Check("-1000000000000", "0", "+", "≈ -1E+12", "");
        }

        [TestMethod]
        public void ScientificResultRoundsToSixSignificantDigits()
        {
            Check("1234567890123", "0", "+", "≈ 1,23457E+12", "");
        }

        [TestMethod]
        public void ScientificRoundingChangesExponent()
        {
            Check("9999999999999", "0", "+", "≈ 1E+13", "");
        }

        [TestMethod]
        public void MaximumDecimalResult()
        {
            Check(decimal.MaxValue.ToString(), "0", "+", "≈ 7,92282E+28", "");
        }

        [TestMethod]
        public void ScientificResultUsesCurrentCulture()
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            Check("1234567890123", "0", "+", "≈ 1.23457E+12", "");
        }

        [TestMethod]
        public void GroupedMillionWithEnglishCulture()
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            Check("1.000.000", "2", "+", "1000002", "");
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

        [TestMethod]
        public void EditingNumberClearsPreviousResult()
        {
            RunOnSta(() =>
            {
                using Form1 form = new Form1();
                form.Show();
                Control<TextBox>(form, "firstNumberTextBox").Text = "2";
                Control<TextBox>(form, "secondNumberTextBox").Text = "3";
                Control<Button>(form, "calculateButton").PerformClick();
                Assert.AreEqual("5", Control<TextBox>(form, "resultTextBox").Text);
                Control<TextBox>(form, "firstNumberTextBox").Text = "10";
                Assert.AreEqual("", Control<TextBox>(form, "resultTextBox").Text);
            });
        }

        [TestMethod]
        public void ChangingOperationClearsPreviousResult()
        {
            RunOnSta(() =>
            {
                using Form1 form = new Form1();
                form.Show();
                Control<TextBox>(form, "firstNumberTextBox").Text = "2";
                Control<TextBox>(form, "secondNumberTextBox").Text = "3";
                Control<Button>(form, "calculateButton").PerformClick();
                Assert.AreEqual("5", Control<TextBox>(form, "resultTextBox").Text);
                Control<ComboBox>(form, "operationComboBox").SelectedItem = "×";
                Assert.AreEqual("", Control<TextBox>(form, "resultTextBox").Text);
            });
        }

        [TestMethod]
        public void EnterStartsCalculation()
        {
            RunOnSta(() =>
            {
                using Form1 form = new Form1();
                form.Show();
                Control<TextBox>(form, "firstNumberTextBox").Text = "2";
                Control<TextBox>(form, "secondNumberTextBox").Text = "3";
                Control<TextBox>(form, "secondNumberTextBox").Focus();
                System.Reflection.MethodInfo? processDialogKey = typeof(Form).GetMethod("ProcessDialogKey",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                bool handled = (bool)processDialogKey!.Invoke(form, new object[] { Keys.Enter })!;
                Assert.IsTrue(handled);
                Assert.AreEqual("5", Control<TextBox>(form, "resultTextBox").Text);
            });
        }

        [TestMethod]
        public void ResultIsReadOnly()
        {
            RunOnSta(() =>
            {
                using Form1 form = new Form1();
                form.Show();
                Control<TextBox>(form, "firstNumberTextBox").Text = "2";
                Control<TextBox>(form, "secondNumberTextBox").Text = "3";
                Control<Button>(form, "calculateButton").PerformClick();
                TextBox result = Control<TextBox>(form, "resultTextBox");
                Assert.AreEqual("5", result.Text);
                Assert.IsTrue(result.ReadOnly);
            });
        }

        [TestMethod]
        public void GroupedMillionThroughForm()
        {
            RunOnSta(() =>
            {
                using Form1 form = new Form1();
                form.Show();
                Control<TextBox>(form, "firstNumberTextBox").Text = "1.000.000";
                Control<TextBox>(form, "secondNumberTextBox").Text = "2";
                Control<Button>(form, "calculateButton").PerformClick();
                Assert.AreEqual("1000002", Control<TextBox>(form, "resultTextBox").Text);
                Assert.AreEqual("", Control<Label>(form, "errorLabel").Text);
            });
        }

        [TestMethod]
        public void LargeResultThroughForm()
        {
            RunOnSta(() =>
            {
                using Form1 form = new Form1();
                form.Show();
                Control<ComboBox>(form, "operationComboBox").SelectedItem = "×";
                Control<TextBox>(form, "firstNumberTextBox").Text = "1.000.000";
                Control<TextBox>(form, "secondNumberTextBox").Text = "1.000.000";
                Control<Button>(form, "calculateButton").PerformClick();
                Assert.AreEqual("≈ 1E+12", Control<TextBox>(form, "resultTextBox").Text);
                Assert.AreEqual("", Control<Label>(form, "errorLabel").Text);
            });
        }

        [TestMethod]
        public void InvalidDotGroupingThroughForm()
        {
            RunOnSta(() =>
            {
                using Form1 form = new Form1();
                form.Show();
                Control<TextBox>(form, "firstNumberTextBox").Text = "1.00.000";
                Control<TextBox>(form, "secondNumberTextBox").Text = "2";
                Control<Button>(form, "calculateButton").PerformClick();
                Assert.AreEqual("", Control<TextBox>(form, "resultTextBox").Text);
                Assert.AreEqual("Введите два числа.", Control<Label>(form, "errorLabel").Text);
            });
        }

        [TestMethod]
        public void MaximumResultThroughForm()
        {
            RunOnSta(() =>
            {
                using Form1 form = new Form1();
                form.Show();
                Control<TextBox>(form, "firstNumberTextBox").Text = decimal.MaxValue.ToString();
                Control<TextBox>(form, "secondNumberTextBox").Text = "0";
                Control<Button>(form, "calculateButton").PerformClick();
                Assert.AreEqual("≈ 7,92282E+28", Control<TextBox>(form, "resultTextBox").Text);
                Assert.AreEqual("", Control<Label>(form, "errorLabel").Text);
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
