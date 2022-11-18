using System.Threading.Tasks;

using NUnit.Framework;

namespace BuilderGenerator.Tests;

[TestFixture]
public class BuilderGeneratorSnapshotTests
{
    [Test]
    public Task NoNamespaceShouldGenerateCodeCorrectly()
    {
        // The Source code to test
        const string Source = @"
using BuilderGenerator;

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

        return TestHelper.Verify(Source);
    }

    [Test]
    public Task GenericClassShouldGenerateCodeCorrectly()
    {
        // The Source code to test
        const string Source = @"
using BuilderGenerator;

namespace Generics.Test;

[Buildable]
public class Foo<T>
{
    public Foo(int x, T y)
    {
        X = x;
        Y = y;
    }

    public int X { get; }
    public T Y { get; }
}";

        return TestHelper.Verify(Source);
    }
}
