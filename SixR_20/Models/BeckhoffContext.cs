using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Financial;

namespace SixR_20.Models
{
    public static class BeckhoffContext
    {
        public static MyController Controller { get; set; }
        public static Traj7Seg Traj { get; set; }
        public static string BeckhoffAddress { get; set; }
        public static int BeckhoffPort { get; set; }
        public static decimal[] ValidJointSpace = { 165, 100, 95, 180, 100, 180 };
        public static int modeOfGUI = 1;
        public static double DoubleTolerance = .000001;
        static BeckhoffContext()
        {
            BeckhoffAddress = "5.35.215.196.1.1";
            BeckhoffPort = 801;
        }

        public static void StartController()
        {
            Controller =new MyController(BeckhoffAddress, BeckhoffPort);
            Traj=new Traj7Seg();
        }

        public static decimal Atan3(decimal y, decimal x)
        {
            if (Math.Abs((double)x) < DoubleTolerance && Math.Abs((double)y) < DoubleTolerance)
                return 0;
            if (Math.Abs((double)y) < DoubleTolerance)
            {
                return x > 0 ? 0 : Convert.ToDecimal( Math.PI);
            }
            if (Math.Abs((double)x) < DoubleTolerance)
            {
                return y > 0 ? Convert.ToDecimal(Math.PI / 2) : -Convert.ToDecimal(Math.PI / 2);
            }
            return Convert.ToDecimal(Math.Atan2((double)y, (double)x));
        }
    }
}
