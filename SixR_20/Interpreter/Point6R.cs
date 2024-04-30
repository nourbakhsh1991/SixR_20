using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixR_20.Interpreter
{
    class Point6R
    {
        private double[] _j = new double[6];
        private double[] _eular = new double[6];
        private bool[] validVals = new bool[6];
        public double F, Con;
        public bool ValidPoint;
        public int IkSolutionBranchNumber { get; set; }
        public bool IsEular { get; }
        public double[] Js { get { return _j; } }
        public double[] Eulars { get { return _eular; } }
        public bool[] ValidVals { get { return validVals; } }

        public Point6R()
        {
            ValidPoint = false;
        }

        public Point6R(double[] vals, bool isEular, double f = 0, double con = 1)
        {
            if (isEular)
            {
                _eular = vals.Clone() as double[];
            }
            else
            {
                _j = vals.Clone() as double[];
            }
            validVals = new[] { true, true, true, true, true, true };
            F = f;
            ValidPoint = true;
            Con = con;
        }
        public Point6R(double[] vals, bool[] Validvals, bool isEular, double f = 0, double con = 1)
        {
            if (isEular)
            {
                _eular = vals.Clone() as double[];
            }
            else
            {
                _j = vals.Clone() as double[];
            }
            validVals = Validvals.Clone() as bool[];
            F = f;
            Con = con;
            this.IsEular = isEular;
            ValidPoint = true;
        }
    }
}
