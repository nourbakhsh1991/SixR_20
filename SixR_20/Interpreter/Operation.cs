using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixR_20.Interpreter
{
    class Operation
    {
        public List<Operation> Child = new List<Operation>();
        public Action Act { get; set; }
        public dynamic Data { get; set; }
    }
}
