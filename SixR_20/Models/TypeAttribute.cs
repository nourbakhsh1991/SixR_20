using System;

namespace SixR_20.Models
{
    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    internal sealed class TypeAttribute : Attribute
    {
        internal enum RW
        {
            ReadOnly,
            WriteOnly,
            FullControll
        }
        internal enum Types
        {
            [Numeric(1)]
            Bool,
            [Numeric(1)]
            SInt,
            [Numeric(2)]
            Int,
            [Numeric(2)]
            Word,
            [Numeric(4)]
            DInt,
            [Numeric(4)]
            UDInt,
            [Numeric(4)]
            Real,
            [Numeric(8)]
            LReal
        }
        public Types Type { get; set; }
        public int Length { get; set; }
        public RW RWStatus { get; set; }
        public string SourceFunction { get; set; }
        public bool Notify { get; set; }
        public bool IsUnsign { get; set; }
        public TypeAttribute(Types type, RW rWStatus, int length = 1, string sourceFunction = "", bool notify = false, bool isUnsign = false)
        {
            Type = type;
            Length = length;
            RWStatus = rWStatus;
            SourceFunction = sourceFunction;
            Notify = notify;
            IsUnsign = isUnsign;
        }
    }
}
