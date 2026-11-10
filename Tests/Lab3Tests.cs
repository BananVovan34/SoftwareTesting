using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareTesting.Lab3;

namespace SoftwareTesting.Tests;

[TestClass]
public class Lab3Tests
{
    [DataTestMethod]
    [DataRow(new double[] { 3 }, 0, 0, 100.0, 1.0, DisplayName = "T01")]
    [DataRow(new double[] { 0, 2 }, 1, 1, 1.0, 0.5, DisplayName = "T02")]
    [DataRow(new double[] { 0, 2 }, 1, 0, 1.0, 0.5, DisplayName = "T03")]
    [DataRow(new double[] { 0, 1, 2 }, 2, 1, 1.5, 0.75, DisplayName = "T04")]
    [DataRow(new double[] { 0, 1, 2 }, 2, 1, 1.0, 1.0, DisplayName = "T05")]
    [DataRow(new double[] { 0, 1, 2 }, 2, 1, 0.0, 0.0, DisplayName = "T06")]
    [DataRow(new double[] { -2, 0, 2 }, 2, 1, 3.0, -1.25, DisplayName = "T07")]
    [DataRow(new double[] { -1, 0, 2, 4 }, 3, 2, 1.0, 0.5, DisplayName = "T08")]
    public void ExampleMatchesExpectedValue(double[] x, int n, int k, double t, double expected)
    {
        double actual = Program.Calculate(x, n, k, t);
        Assert.AreEqual(expected, actual, 1e-12);
    }

    [TestMethod]
    public void S01LoopIsSkippedWhenNIsNegative()
    {
        double actual = Program.Calculate(new double[0], -1, 0, 0);
        Assert.AreEqual(1.0, actual);
    }

    [TestMethod]
    public void BasisFunctionsSumToOne()
    {
        double[] x = { -2, 0, 3, 7 };
        double sum = 0;

        for (int k = 0; k < x.Length; k++)
            sum += Program.Calculate(x, x.Length - 1, k, 1.25);

        Assert.AreEqual(1.0, sum, 1e-12);
    }
}
