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

        AddressBuilder builder2 = new();
        builder2.State = "NY";
        builder2.Street = "100 Park Avenue";
        builder2.City = "New York";
        builder2.Zip = "10001";

        Address address = builder2.Build();

        Console.WriteLine(address);
    }
}
