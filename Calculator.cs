namespace SoftwareTesting
{
    public static class Calculator
    {
        public static decimal Calculate(decimal firstNumber, decimal secondNumber, string operation)
        {
            return operation switch
            {
                "+" => firstNumber + secondNumber,
                "-" => firstNumber - secondNumber,
                "×" => firstNumber * secondNumber,
                "÷" => secondNumber == 0
                    ? throw new DivideByZeroException("На ноль делить нельзя.")
                    : firstNumber / secondNumber,
                _ => throw new ArgumentException("Неизвестная операция.", nameof(operation))
            };
        }
    }
}
