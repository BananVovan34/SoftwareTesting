using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareTesting.Lab3;

namespace SoftwareTesting.Tests;

[TestClass]
public class LagrangeBasisTests
{
    [DataTestMethod]
    [DataRow("T01")]
    [DataRow("T02")]
    [DataRow("T03")]
    [DataRow("T04")]
    [DataRow("T05")]
    [DataRow("T06")]
    [DataRow("T07")]
    [DataRow("T08")]
    public void ReportExampleMatchesExpectedValue(string id)
    {
        ExampleResult example = LabExamples.Run().Single(item => item.Id == id);
        Assert.IsTrue(example.InDomain);
        Assert.AreEqual(example.Expected, example.Actual, LabExamples.Tolerance);
    }

    [TestMethod]
    public void BasisPathP1IsOnlyAStructuralProbe()
    {
        EvaluationResult result = LagrangeBasis.EvaluateFragment([], -1, 0, 0);
        Assert.AreEqual(1, result.Value);
        CollectionAssert.AreEqual(new[] { 1, 2, 7 }, result.Path.ToArray());
        Assert.ThrowsException<ArgumentException>(() => LagrangeBasis.Calculate([], 0, 0));
    }

    [TestMethod]
    public void BasisPathP2SkipsTheOnlyNode()
    {
        EvaluationResult result = LagrangeBasis.Calculate([3], 0, 100);
        Assert.AreEqual(1, result.Value);
        CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 2, 7 }, result.Path.ToArray());
    }

    [TestMethod]
    public void BasisPathP3MultipliesBeforeSkippingK()
    {
        EvaluationResult result = LagrangeBasis.Calculate([0, 2], 1, 1);
        Assert.AreEqual(0.5, result.Value);
        CollectionAssert.AreEqual(new[] { 1, 2, 3, 5, 6, 2, 3, 4, 2, 7 }, result.Path.ToArray());
    }

    [TestMethod]
    public void FirstNodeIsSkippedBeforeMultiplication()
    {
        EvaluationResult result = LagrangeBasis.Calculate([0, 2], 0, 1);
        CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 2, 3, 5, 6, 2, 7 }, result.Path.ToArray());
    }

    [TestMethod]
    public void ValidExamplesCoverAllEightGraphEdges()
    {
        var actual = LabExamples.Run().Where(example => example.InDomain)
            .SelectMany(example => example.Path.Zip(example.Path.Skip(1)))
            .ToHashSet();
        (int, int)[] expected = [(1, 2), (2, 3), (2, 7), (3, 4), (3, 5), (4, 2), (5, 6), (6, 2)];
        CollectionAssert.AreEquivalent(expected, actual.ToArray());
    }

    [TestMethod]
    public void BasisFunctionsSumToOneAtAnIndependentPoint()
    {
        double[] nodes = [-2, 0, 3, 7];
        double sum = Enumerable.Range(0, nodes.Length)
            .Sum(k => LagrangeBasis.Calculate(nodes, k, 1.25).Value);
        Assert.AreEqual(1, sum, LabExamples.Tolerance);
    }

    [TestMethod]
    public void RejectsNullNodes() =>
        Assert.ThrowsException<ArgumentNullException>(() => LagrangeBasis.Calculate(null!, 0, 1));

    [TestMethod]
    public void RejectsEmptyNodes() =>
        Assert.ThrowsException<ArgumentException>(() => LagrangeBasis.Calculate([], 0, 1));

    [DataTestMethod]
    [DataRow(-1)]
    [DataRow(2)]
    public void RejectsIndexOutsideTheNodes(int k) =>
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => LagrangeBasis.Calculate([0, 2], k, 1));

    [TestMethod]
    public void RejectsRepeatedNodes() =>
        Assert.ThrowsException<ArgumentException>(() => LagrangeBasis.Calculate([0, 0], 0, 1));

    [TestMethod]
    public void RejectsNonFinitePoint() =>
        Assert.ThrowsException<ArgumentException>(() => LagrangeBasis.Calculate([0, 2], 0, double.NaN));

    [TestMethod]
    public void RejectsNonFiniteNode() =>
        Assert.ThrowsException<ArgumentException>(() => LagrangeBasis.Calculate([0, double.PositiveInfinity], 0, 1));

    [TestMethod]
    public void RejectsNonFiniteComputation() =>
        Assert.ThrowsException<ArithmeticException>(() =>
            LagrangeBasis.Calculate([0, 1, 2], 0, double.MaxValue));
}
