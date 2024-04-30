using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixR_20.Models
{
    public class TrajectoryPointList<T>
    {
        public List<T> q = new List<T>();
        public List<T> v = new List<T>();
        public List<T> a = new List<T>();
        public int TrajLength;
        public TrajectoryPointList()
        {
        }

        public void AddPoint(T q, T v, T a)
        {
            this.q.Add(q);
            this.v.Add(v);
            this.a.Add(a);
            this.TrajLength++;
        }
        public void FillTraj(T[] q, T[] v, T[] a, int len)
        {
            this.q = new List<T>(q);
            this.v = new List<T>(v);
            this.a = new List<T>(a);
            TrajLength = len;
        }
    }
}
