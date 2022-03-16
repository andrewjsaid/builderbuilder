using System.Threading.Tasks;
using NUnit.Framework;
using VerifyNUnit;

namespace BuilderBuilder.Tests;

[TestFixture]
public class BuilderGeneratorSnapshotTests
{
    [Test]
    public Task ShouldGenerateCodeCorrectly()
    {
        // The source code to test
        const string source = @"
using BuilderBuilder;

[Buildable]
public class Point
{
    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; }
    public int Y { get; }
}";

        return TestHelper.Verify(source);
    }
}
