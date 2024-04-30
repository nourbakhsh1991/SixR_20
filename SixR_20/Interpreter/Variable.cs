using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixR_20.Interpreter
{
    class Variable
    {
        public PrimitiveType Type { get; set; }
        public bool IsArray { get; set; }
        public int ArrayLength { get; set; }
        public List<int> ArrayDim { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public bool IsLeft { get; set; }
        public bool HasValue { get; set; }
        public bool IsReadOnly { get; set; }
    }
}
