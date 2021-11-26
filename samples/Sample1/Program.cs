using System;

namespace Sample1;

public static class Program
{
    public static void Main()
    {
        PersonBuilder builder = new();
        builder.Name = "Andrew";
        builder.Age = 29;

        Person person = builder.Build();

        Console.WriteLine(person.Name);
        Console.WriteLine(person.Age);
    }
}
