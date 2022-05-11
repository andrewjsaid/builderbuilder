using System;

namespace BuilderDemo;

public static class Program
{
    public static void Main()
    {
        PersonBuilder builder = new()
        {
            Name = "Andrew",
            Age = 29
        };

        Person person = builder.Build();

        Console.WriteLine(person.Name);
        Console.WriteLine(person.Age);

        AddressBuilder builder2 = new()
        {
            State = "NY",
            Street = "100 Park Avenue",
            City = "New York",
            ZipCode = "10001"
        };
        Address address = builder2.Build();

        Console.WriteLine(address);

        FooBuilder<string> builder3 = new()
        {
            X = 3,
            Y = "A"
        };
        var foo = builder3.Build();
        Console.WriteLine(foo);
    }
}
