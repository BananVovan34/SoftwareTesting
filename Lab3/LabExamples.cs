namespace SoftwareTesting.Lab3;

public sealed record ExampleResult(string Id, double[] Nodes, int N, int K, double T,
    double Expected, double Actual, IReadOnlyList<int> Path, bool Passed, bool InDomain);

public static class LabExamples
{
    public const double Tolerance = 1e-12;

    public static IReadOnlyList<ExampleResult> Run()
    {
        (string Id, double[] Nodes, int K, double T, double Expected)[] cases =
        [
            ("T01", [3], 0, 100, 1),
            ("T02", [0, 2], 1, 1, 0.5),
            ("T03", [0, 2], 0, 1, 0.5),
            ("T04", [0, 1, 2], 1, 1.5, 0.75),
            ("T05", [0, 1, 2], 1, 1, 1),
            ("T06", [0, 1, 2], 1, 0, 0),
            ("T07", [-2, 0, 2], 1, 3, -1.25),
            ("T08", [-1, 0, 2, 4], 2, 1, 0.5)
        ];

        List<ExampleResult> results = [];
        foreach (var example in cases)
        {
            EvaluationResult actual = LagrangeBasis.Calculate(example.Nodes, example.K, example.T);
            results.Add(new ExampleResult(example.Id, example.Nodes, example.Nodes.Length - 1,
                example.K, example.T, example.Expected, actual.Value, actual.Path,
                Math.Abs(actual.Value - example.Expected) <= Tolerance, true));
        }

        // Только структурный эксперимент над исходным фрагментом.
        // n = -1 не является допустимой степенью интерполяции и не принимается Calculate.
        EvaluationResult probe = LagrangeBasis.EvaluateFragment([], -1, 0, 0);
        results.Add(new ExampleResult("S01", [], -1, 0, 0, 1, probe.Value, probe.Path,
            probe.Value == 1 && probe.Path.SequenceEqual(new[] { 1, 2, 7 }), false));
        return results.AsReadOnly();
    }
}
