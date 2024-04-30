using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace SixR_20.Models
{
    public static class SixRConstants
    {
        public const int BufferLen = 500;
        public const int GeneralBufferLen = 20;
        public const int NumberOfAxis = 6;
        public static ResourceManager ResourceManager = new ResourceManager("SixR_20.Properties.Resources", Assembly.GetExecutingAssembly());
        public const int NumberOfInputs = 8;
        public const int NumberOfOutputs = 8;
        public static int[] MotorMovementRange = new int[] { 180, 180, 180, int.MaxValue, int.MaxValue, int.MaxValue };
        public static decimal[] toolParam = new decimal[] { 1, 0, 0, 0, 0, 135, 0, 0 };
    }
}
