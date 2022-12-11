using System;
using System.Reflection;

namespace BuilderDemo;

public static class Program
{
    public static void Main()
    {
        // https://stackoverflow.com/a/60545278/781045
        AssemblyConfigurationAttribute? assemblyConfigurationAttribute = typeof(Program).Assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
        if (assemblyConfigurationAttribute is not null)
            Console.WriteLine($"Build Configuration is {assemblyConfigurationAttribute.Configuration}.");

        PersonBuilder builder = new()
        {
            Name = "Andrew",
            Age = 29
        };

        Person person = builder.Build();

        Console.WriteLine(person);

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
        Foo<string> foo = builder3.Build();
        Console.WriteLine(foo);
    }
}
