using System.Globalization;
using System.Text;

namespace SoftwareTesting.Lab3;

public class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.Write("Введите n (количество узлов минус 1): ");
        int n = Convert.ToInt32(Console.ReadLine());
        double[] x = new double[n + 1];

        for (int i = 0; i <= n; i++)
        {
            Console.Write($"x[{i}] = ");
            x[i] = double.Parse(Console.ReadLine()!.Replace(',', '.'), CultureInfo.InvariantCulture);
        }

        Console.Write("Введите индекс k: ");
        int k = Convert.ToInt32(Console.ReadLine());

        Console.Write("Введите точку t: ");
        double t = double.Parse(Console.ReadLine()!.Replace(',', '.'), CultureInfo.InvariantCulture);

        double L = Calculate(x, n, k, t);
        Console.WriteLine($"L = {L}");
    }

    public static double Calculate(double[] x, int n, int k, double t)
    {
        double L = 1.0;
        int i = 0;

        while (i <= n)
        {
            if (i == k)
            {
                ++i;
                continue;
            }

            L *= (t - x[i]) / (x[k] - x[i]);
            ++i;
        }

        return L;
    }
}
