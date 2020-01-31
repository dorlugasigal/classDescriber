using System;
using System.Collections.Generic;
using System.Net.Sockets;
using ClassDescriber.Library;

namespace ClassDescriber.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var per1 = new Person { Name = "Dor1", address = new Address { Number = 1, Street = "haha1" } };
            var per2 = new Person { Name = "Dor2", address = new Address { Number = 2, Street = "haha2" } };
            var per3 = new Person { Name = "Dor3", address = new Address { Number = 3, Street = "haha3" } };
            IEnumerable<Person> arr = new List<Person> { per1, per2, per3 };
            Console.Write(Describer.Describe(arr));
        }



    }
    public class Person
    {
        public string Name { get; set; }
        public Address address { get; set; }
    }
    public class Address
    {
        public string Street { get; set; }
        public int Number { get; set; }
    }
}
