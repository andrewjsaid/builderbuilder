using BuilderGenerator;

namespace BuilderDemo;

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
}
