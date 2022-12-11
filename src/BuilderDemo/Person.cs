using System;

using BuilderGenerator;

namespace BuilderDemo;

[Buildable]
public class Person
{
    public Person(string name, int age)
    {
        Name = name;
        Age = age;
    }

    public string Name { get; }
    public int Age { get; }

    public override string ToString() => $"Person(Name = {Name}, Age={Age})";

    public override int GetHashCode() => HashCode.Combine(Name, Age);
}
