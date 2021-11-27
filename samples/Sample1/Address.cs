using System;
using BuilderBuilder;

namespace Sample1
{
    [Buildable]
    public class Address
    {
        public Address(string street, string city, string state, string zip)
        {
            Street = street;
            City = city;
            State = state;
            Zip = zip;
        }

        public string Street { get; }
        public string City { get; }
        public string State { get; }
        public string Zip { get; }

        public override string ToString() => $"{Street}, {City}, {State} {Zip}";
    }
}
