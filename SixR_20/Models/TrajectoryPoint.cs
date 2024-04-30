using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixR_20.Models
{
    public class TrajectoryPoint
    {
        public double Q;
        public double V;
        public TrajectoryPoint(double q1, double v1)
        {
            this.Q = q1;
            this.V = v1;
        }
        TrajectoryPoint()
        {
            Q = V = 0.0;
        }
    }
}
