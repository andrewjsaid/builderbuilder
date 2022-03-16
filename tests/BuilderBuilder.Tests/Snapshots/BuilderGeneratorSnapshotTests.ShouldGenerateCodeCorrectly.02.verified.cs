//HintName: PointBuilder.cs
using System;

namespace <global namespace>;
public class PointBuilder {
public int X { get; set; }
public int Y { get; set; }

public Point Build() =>
    new Point(X, Y);

}
