using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace SoftwareTesting.Lab3;

internal static class Program
{
    private static int Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        try
        {
            if (args is ["--help"])
            {
                Console.WriteLine("ЛР3, вариант 3: вычисление L_k(t).");
                Console.WriteLine("Без аргументов: ввод узлов, индекса k и точки t по запросам.");
                Console.WriteLine("Аргументы: k t x0 x1 ... xn. Дроби: 1.5 или 1,5.");
                Console.WriteLine("Пример: dotnet run --project Lab3/SoftwareTesting.Lab3.csproj -- 1 1 0 2");
                Console.WriteLine("--demo: JSON с восемью примерами и отдельным структурным экспериментом.");
                return 0;
            }

            if (args is ["--demo"])
            {
                IReadOnlyList<ExampleResult> examples = LabExamples.Run();
                Console.WriteLine(JsonSerializer.Serialize(new
                {
                    Variant = 3,
                    Tolerance = LabExamples.Tolerance,
                    Results = examples,
                    Passed = examples.All(example => example.Passed)
                }, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                }));
                return examples.All(example => example.Passed) ? 0 : 1;
            }

            double[] nodes;
            int k;
            double t;
            if (args.Length == 0)
            {
                Console.WriteLine("Лабораторная работа 3, вариант 3. Вычисление L_k(t).");
                Console.Write("Узлы x0 ... xn через пробел: ");
                nodes = ReadLine().Split([' ', ';', '\t'], StringSplitOptions.RemoveEmptyEntries)
                    .Select(ParseNumber).ToArray();
                Console.Write("Индекс k от 0 до n: ");
                k = int.Parse(ReadLine(), CultureInfo.InvariantCulture);
                Console.Write("Точка t: ");
                t = ParseNumber(ReadLine());
            }
            else
            {
                if (args.Length < 3)
                    throw new ArgumentException("Укажите k, t и хотя бы один узел. Справка: --help.");
                k = int.Parse(args[0], CultureInfo.InvariantCulture);
                t = ParseNumber(args[1]);
                nodes = args.Skip(2).Select(ParseNumber).ToArray();
            }

            EvaluationResult result = LagrangeBasis.Calculate(nodes, k, t);
            Console.WriteLine($"n = {nodes.Length - 1}; k = {k}");
            Console.WriteLine($"L_{k}({t.ToString("G17", CultureInfo.InvariantCulture)}) = " +
                result.Value.ToString("G17", CultureInfo.InvariantCulture));
            Console.WriteLine("Путь: " + string.Join(" -> ", result.Path));
            return 0;
        }
        catch (Exception error) when (error is ArgumentException or FormatException
            or OverflowException or ArithmeticException or EndOfStreamException)
        {
            Console.Error.WriteLine("Ошибка: " + error.Message);
            return 1;
        }
    }

    private static string ReadLine() => Console.ReadLine()
        ?? throw new EndOfStreamException("Ввод завершён до получения всех данных.");

    private static double ParseNumber(string text) =>
        double.Parse(text.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture);
}
