using System;

namespace ClassDescriber.Library.Extensions
{
    public static class Extensions
    {
        public static string Repeat(this char c, int n)
        {
            return new string(c, n);
        }
    }
}