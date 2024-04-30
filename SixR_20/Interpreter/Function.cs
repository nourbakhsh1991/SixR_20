using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SixR_20.Interpreter
{
    class Function
    {
        private string _name;
        private object _output;
        private Dictionary<string,Variable> _params=new Dictionary<string, Variable>();
        private SixRGrammerParser.RoutineBodyContext _bodyContext;
        private List<Variable> _variables = new List<Variable>();
        public string Name
        {
            get { return _name; }
            set
            {
                if(!char.IsLetterOrDigit(value[0]) ) throw new Exception("");
                _name = value;
            }
        }

        public object Output
        {
            get { return _output; }
            set { _output = value; }
        }

        public Dictionary<string, Variable> Params
        {
            get { return _params; }
            set { _params = value; }
        }

        public SixRGrammerParser.RoutineBodyContext BodyContext
        {
            get { return _bodyContext; }
            set { _bodyContext = value; }
        }

        public List<Variable> Variables
        {
            get { return _variables; }
            set { _variables = value; }
        }
    }
}
