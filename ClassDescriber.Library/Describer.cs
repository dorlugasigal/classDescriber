using System;

namespace ClassDescriber.Library
{
    public class Describer
    {
        public static void Describe(object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
        }
    }
}