using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SoftwareTesting.Tests")]

namespace SoftwareTesting.Lab3;

public sealed record EvaluationResult(double Value, IReadOnlyList<int> Path);

public static class LagrangeBasis
{
    /// <summary>Вычисляет один базисный многочлен L_k(t), а не весь интерполяционный многочлен.</summary>
    public static EvaluationResult Calculate(double[] nodes, int k, double t)
    {
        ArgumentNullException.ThrowIfNull(nodes);
        if (nodes.Length == 0)
            throw new ArgumentException("Нужен хотя бы один узел.", nameof(nodes));
        if (k < 0 || k >= nodes.Length)
            throw new ArgumentOutOfRangeException(nameof(k), "Индекс k должен быть от 0 до n.");
        if (!double.IsFinite(t) || nodes.Any(value => !double.IsFinite(value)))
            throw new ArgumentException("Узлы и точка t должны быть конечными числами.");
        if (nodes.Distinct().Count() != nodes.Length)
            throw new ArgumentException("Узлы должны быть различными.", nameof(nodes));

        EvaluationResult result = EvaluateFragment(nodes, nodes.Length - 1, k, t);
        if (!double.IsFinite(result.Value))
            throw new ArithmeticException("Результат выходит за диапазон double.");
        return result;
    }

    // Исходный фрагмент варианта 3. Предусловия проверяет Calculate.
    // Номера Path соответствуют графу в Docs/LAB3.md.
    // Диагностические path.Add не входят в анализируемую управляющую структуру.
    internal static EvaluationResult EvaluateFragment(double[] x, int n, int k, double t)
    {
        List<int> path = [1];
        double L = 1.0;
        int i = 0;

        while (Check(path, 2, i <= n))
        {
            if (Check(path, 3, i == k))
            {
                path.Add(4);
                ++i;
                continue;
            }

            path.Add(5);
            L *= (t - x[i]) / (x[k] - x[i]);
            path.Add(6);
            ++i;
        }

        path.Add(7);
        return new EvaluationResult(L, path.AsReadOnly());
    }

    private static bool Check(List<int> path, int node, bool condition)
    {
        path.Add(node);
        return condition;
    }
}
