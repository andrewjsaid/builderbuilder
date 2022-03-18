using System;

namespace BuilderDemo;

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

        AddressBuilder builder2 = new();
        builder2.State = "NY";
        builder2.Street = "100 Park Avenue";
        builder2.City = "New York";
        builder2.ZipCode = "10001";

        Address address = builder2.Build();

        Console.WriteLine(address);

        // TODO: Figure out later.

        FooBuilder<string> builder3 = new();
        builder3.X = 3;
        builder3.Y = "A";
        var foo = builder3.Build();
        Console.WriteLine(foo);

    }
}
