//HintName: FooBuilder.cs
using System;

namespace Generics.Test;

public class FooBuilder<T>{
    public int X { get; set; }
    public T Y { get; set; }

    public Foo<T> Build() =>
        new Foo<T>(X, Y);
}

