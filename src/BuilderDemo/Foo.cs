using BuilderGenerator;
using System;

namespace BuilderDemo;

[Buildable]
public class Foo<T> :IEquatable<Foo<T>>
{
    public Foo(int x, T y)
    {
        X = x;
        Y = y;
    }

    public int X { get; }
    public T Y { get; }

    public override bool Equals(object obj) => Equals(obj as Foo<T>);

    public bool Equals(Foo<T> other)
    {
        if (other is null)
            return false;

        return X == other.X && Equals(Y, other.Y);
    }

    public override string ToString() => $"Foo{{X = {X}, Y = {Y}}}";
}
