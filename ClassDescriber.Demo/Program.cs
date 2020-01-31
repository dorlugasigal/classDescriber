using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Sockets;
using ClassDescriber.Library;
using Microsoft.VisualBasic.CompilerServices;

namespace ClassDescriber.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var per1 = new Person
            {
                Names = new List<Address>
                {
                    new Address
                    {
                        Street = "asdasd",
                        Number = 2
                    },new Address
                    {
                        Street = "aa",
                        Number = 3
                    }
                }
            };


            IEnumerable<Person> arr = new List<Person> { per1 };
            Console.Write(Describer.Describe("asd"));
        }



    }
    public class Person
    {
        public IEnumerable<Address> Names { get; set; }
    }
    public class Address
    {
        public string Street { get; set; }
        public int Number { get; set; }
    }
}
