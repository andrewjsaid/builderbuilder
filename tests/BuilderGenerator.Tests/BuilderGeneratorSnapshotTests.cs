using System.Threading.Tasks;
using NUnit.Framework;
using VerifyNUnit;

namespace BuilderBuilder.Tests;

[TestFixture]
public class BuilderGeneratorSnapshotTests
{
    [Test]
    public Task NoNamespaceShouldGenerateCodeCorrectly()
    {
        // The source code to test
        const string source = @"
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

        return TestHelper.Verify(source);
    }

    [Test]
    public Task GenericClassShouldGenerateCodeCorrectly()
    {
        // The source code to test
        const string source = @"
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

        return TestHelper.Verify(source);
    }
}
