using System;

namespace SixR_20.Models
{
    internal class NumericAttribute : Attribute
    {
        public int Number { get; set; }
        public NumericAttribute(int number)
        {
            Number = number;
        }
    }
}
