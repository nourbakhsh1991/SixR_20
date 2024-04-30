using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixR_20.Models
{
    public static class UnitConverter
    {
        public static decimal DriveEncoderRes = 524287;
        public static decimal[] PulsToDegFactor = { 360.0m / (DriveEncoderRes * 162.0m), 360.0m / (DriveEncoderRes * 161.0m), -1.0m * 360.0m / (DriveEncoderRes * 161.0m), 360.0m / (DriveEncoderRes * 102.0m), 360.0m / (DriveEncoderRes * 100.0m), -1.0m * 360.0m / (DriveEncoderRes * 102.0m) };

        public static decimal PulsTomFactor = 1.0m / 3744.91m;
        public static decimal PulsTommFactor = PulsTomFactor / 1000;
        public static decimal PulsTocmFactor = PulsTomFactor / 100.0m;
        public static decimal mmTopulseFactor = 1.0m / PulsTommFactor;
        public static decimal mTocmFactor = 100.0m;

        public static decimal pulspsTorpsFactor = PulsTocmFactor;
        public static decimal pulspsTorpmFactor = PulsTocmFactor;

        public static decimal pulspsTomsFactor = PulsTomFactor;
        public static decimal pulspsTommsFactor = PulsTommFactor;
        public static decimal pulspsTocmsFactor = PulsTocmFactor;

        public static int cmsTommsFactor = 10;
        public static int rpsTommsFactor = 10;
        public static int rpmTommsFactor = 10;
        public static decimal DegToRadFactor = PulsTocmFactor;
        public static decimal mmTocmFactor = 1.0m / 10.0m;
        public static decimal DegpsToRadpsFactor = PulsTocmFactor;
        public static decimal mmpsTocmpsFactor = mmTocmFactor;

        public static decimal MMMTOMMsFactor = 1.0m / 60;
        public static decimal ActualToMM = PulsTommFactor;
        public static decimal MMsToMMM = 60;
        public static decimal MMToActual = 1 / PulsTommFactor;
        public static decimal GetMMMToMMs()
        {
            return MMMTOMMsFactor;
        }
        public static decimal GetMMsToMMM()
        {
            return MMsToMMM;
        }
        public static decimal GetActualToMM()
        {
            return ActualToMM;
        }
        public static decimal GetMMToActual()
        {
            return MMToActual;
        }
    }
}
