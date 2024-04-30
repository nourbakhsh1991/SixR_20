using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Double;
using SixR_20.Library;
using static System.Math;

namespace SixR_20.Models
{
    public class TrajectoryPoints
    {
        public double[] q;
        public double[] v;
        public double[] a;
        public int TrajLength;
        public TrajectoryPoints()
        {
            q = new double[1000000];
            v = new double[1000000];
            a = new double[1000000];
        }
        public void FillTraj(double[] q, double[] v, double[] a, int len)
        {
            this.q = q;
            this.v = v;
            this.a = a;
            TrajLength = len;
        }
    }
    public class Traj7Seg
    {
        readonly decimal[] L = { 389.5m, 600.0m, 200, 685.5m, 135 };

        // TODO
        public decimal[] GetCartPos(decimal[] theta, decimal[] ToolParams)
        {
            decimal[] Q1 = { Convert.ToDecimal(Cos((double)theta[0] / 2.0)), 0, 0, Convert.ToDecimal(Sin((double)theta[0] / 2.0)), 0, 0, 0, L[0] };
            decimal[] Q2 = { Convert.ToDecimal(Cos((double)theta[1] / 2.0)), 0, Convert.ToDecimal(Sin((double)theta[1] / 2.0)), 0, 0, 0, 0, 0 };
            decimal[] Q3 = { Convert.ToDecimal(Cos((double)theta[2] / 2.0)), 0, Convert.ToDecimal(Sin((double)theta[2] / 2.0)), 0, 0, 0, 0, L[1] };
            decimal[] Q4 = { Convert.ToDecimal(Cos((double)theta[3] / 2.0)), Convert.ToDecimal(Sin((double)theta[3] / 2.0)), 0, 0, 0, L[3], 0, L[2] };
            decimal[] Q5 = { Convert.ToDecimal(Cos((double)theta[4] / 2.0)), 0, Convert.ToDecimal(Sin((double)theta[4] / 2.0)), 0, 0, 0, 0, 0 };
            decimal[] Q6 = { Convert.ToDecimal(Cos((double)theta[5] / 2.0)), Convert.ToDecimal(Sin((double)theta[5] / 2.0)), 0, 0, 0, 0, 0, 0 };

            var M6 = DQmultiply(Q6, ToolParams);
            var M5 = DQmultiply(Q5, M6);
            var M4 = DQmultiply(Q4, M5);
            var M3 = DQmultiply(Q3, M4);
            var M2 = DQmultiply(Q2, M3);
            return DQmultiply(Q1, M2);
        }
        public decimal[,] Inversekinematic(decimal[] QG, decimal[] ToolParams)
        {
            var IKtheta = new decimal[8, 6];

            var wrist = DQmultiply(QG, DQinv(ToolParams));

            var t1 = new decimal[2];
            var t2 = new decimal[4];
            var t3 = new decimal[2];
            var t4 = new decimal[4];
            var t5 = new decimal[4];
            var t6 = new decimal[4];
            
            t1[0] = BeckhoffContext.Atan3(wrist[6], wrist[5]);
            t1[1] = t1[0] - Convert.ToDecimal(Math.PI);

            var a = 2 * L[1] * wrist[5] * Convert.ToDecimal(Cos((double)t1[0])) + 2 * L[1] * wrist[6] * Convert.ToDecimal(Sin((double)t1[0]));
            var b = 2 * L[0] * L[1] - 2 * L[1] * wrist[7];
            var c = (wrist[6] * wrist[6] - wrist[5] * wrist[5]) * Convert.ToDecimal(Cos(((double)t1[0])) * Cos(((double)t1[0]))) - L[0] * L[0] - L[1] * L[1] + L[2] * L[2] + L[3] * L[3] - wrist[6] * wrist[6] - wrist[7] * wrist[7] + 2 * L[0] * wrist[7] - wrist[5] * wrist[6] * Convert.ToDecimal(Sin(2 * (double)t1[0]));

            t2[0] = BeckhoffContext.Atan3(b, a) - BeckhoffContext.Atan3(c, +Convert.ToDecimal( Sqrt((double)(a * a + b * b - c * c))));
            t2[1] = BeckhoffContext.Atan3(b, a) - BeckhoffContext.Atan3(c, -Convert.ToDecimal(Sqrt((double)(a * a + b * b - c * c))));
            t2[3] = BeckhoffContext.Atan3(b, -a) - BeckhoffContext.Atan3(c, -Convert.ToDecimal(Sqrt((double)(a * a + b * b - c * c))));
            t2[2] = BeckhoffContext.Atan3(b, -a) - BeckhoffContext.Atan3(c, +Convert.ToDecimal(Sqrt((double)(a * a + b * b - c * c))));

            a = 2 * L[1] * L[3];
            b = 2 * L[1] * L[2];
            c = -L[1] * L[1] - L[2] * L[2] - L[3] * L[3] + (L[0] - wrist[7]) * (L[0] - wrist[7]) + (wrist[5] * Convert.ToDecimal(Cos((double)t1[0])) + wrist[6] * Convert.ToDecimal(Sin((double)t1[0]))) * (wrist[5] * Convert.ToDecimal(Cos((double)t1[0])) + wrist[6] * Convert.ToDecimal(Sin((double)t1[0])));

            t3[0] = BeckhoffContext.Atan3(b, a) - BeckhoffContext.Atan3(c, +Convert.ToDecimal(Sqrt((double)(a * a + b * b - c * c))));
            t3[1] = BeckhoffContext.Atan3(b, a) - BeckhoffContext.Atan3(c, -Convert.ToDecimal(Sqrt((double)(a * a + b * b - c * c))));

            IKtheta[0, 0] = t1[0];
            IKtheta[1, 0] = t1[0];
            IKtheta[2, 0] = t1[0];
            IKtheta[3, 0] = t1[0];
            IKtheta[4, 0] = t1[1];
            IKtheta[5, 0] = t1[1];
            IKtheta[6, 0] = t1[1];
            IKtheta[7, 0] = t1[1];

            IKtheta[0, 1] = t2[0];
            IKtheta[1, 1] = t2[0];
            IKtheta[2, 1] = t2[1];
            IKtheta[3, 1] = t2[1];
            IKtheta[4, 1] = t2[2];
            IKtheta[5, 1] = t2[2];
            IKtheta[6, 1] = t2[3];
            IKtheta[7, 1] = t2[3];

            IKtheta[0, 2] = t3[0];
            IKtheta[1, 2] = t3[0];
            IKtheta[2, 2] = t3[1];
            IKtheta[3, 2] = t3[1];
            IKtheta[4, 2] = t3[0];
            IKtheta[5, 2] = t3[0];
            IKtheta[6, 2] = t3[1];
            IKtheta[7, 2] = t3[1];

            var Q1 = new decimal[4][];
            var Q2 = new decimal[4][];
            var Q3 = new decimal[4][];
            var N4 = new decimal[4][];
            var Beta = new decimal[4];
            var Gamma = new decimal[4];

            for (int i = 0, j = 0; i < 8; i += 2, j++)
            {
                Q1[j] = new[] {Convert.ToDecimal( Cos((double)IKtheta[i, 0] / 2.0)), 0, 0, Convert.ToDecimal(Sin((double)IKtheta[i, 0] / 2.0)), 0, 0, 0, L[0] };
                Q2[j] = new[] { Convert.ToDecimal(Cos((double)IKtheta[i, 1] / 2.0)), 0, Convert.ToDecimal(Sin((double)IKtheta[i, 1] / 2.0)), 0, 0, 0, 0, 0 };
                Q3[j] = new[] { Convert.ToDecimal(Cos((double)IKtheta[i, 2] / 2.0)), 0, Convert.ToDecimal(Sin((double)IKtheta[i, 2] / 2.0)), 0, 0, 0, 0, L[1] };

                N4[j] = DQmultiply(DQinv(Q3[j]), DQmultiply(DQinv(Q2[j]), DQmultiply(DQinv(Q1[j]), wrist)));

                Beta[j] = 2 * BeckhoffContext.Atan3(N4[j][1], N4[j][0]);
                Gamma[j] = 2 * BeckhoffContext.Atan3(N4[j][3], N4[j][2]);
            }

            for (int i = 0; i < 4; i++)
            {
                t4[i] = (Beta[i] + Gamma[i]) / 2;
                t6[i] = (Beta[i] - Gamma[i]) / 2;
                t5[i] = (2 *
                         BeckhoffContext.Atan3(Convert.ToDecimal(Sqrt(Pow(((double)N4[i][2]), 2) + Pow(((double)N4[i][3]), 2))),
                             Convert.ToDecimal(Sqrt(Pow(((double)N4[i][0]), 2) + Pow(((double)N4[i][1]), 2)))));
            }
            IKtheta[0, 3] = t4[0];
            IKtheta[1, 3] = t4[0] + Convert.ToDecimal(Math.PI);
            IKtheta[2, 3] = t4[1];
            IKtheta[3, 3] = t4[1] + Convert.ToDecimal(Math.PI);
            IKtheta[4, 3] = t4[2];
            IKtheta[5, 3] = t4[2] + Convert.ToDecimal(Math.PI);
            IKtheta[6, 3] = t4[3];
            IKtheta[7, 3] = t4[3] + Convert.ToDecimal(Math.PI);

            IKtheta[0, 4] = t5[0];
            IKtheta[1, 4] = -t5[0];
            IKtheta[2, 4] = t5[1];
            IKtheta[3, 4] = -t5[1];
            IKtheta[4, 4] = t5[2];
            IKtheta[5, 4] = -t5[2];
            IKtheta[6, 4] = t5[3];
            IKtheta[7, 4] = -t5[3];

            IKtheta[0, 5] = t6[0];
            IKtheta[1, 5] = t6[0] + Convert.ToDecimal(Math.PI);
            IKtheta[2, 5] = t6[1];
            IKtheta[3, 5] = t6[1] + Convert.ToDecimal(Math.PI);
            IKtheta[4, 5] = t6[2];
            IKtheta[5, 5] = t6[2] + Convert.ToDecimal(Math.PI);
            IKtheta[6, 5] = t6[3];
            IKtheta[7, 5] = t6[3] + Convert.ToDecimal(Math.PI);

            for (int i = 0; i < 8; i++)
            {
                for (int k = 0; k < 6; k++)
                {
                    IKtheta[i, k] = Convert.ToDecimal((((double)IKtheta[i, k] + Math.PI) % (2 * Math.PI)) - Math.PI);
                }
            }

            return IKtheta;
        }
        public decimal[] DQmultiply(decimal[] Q1, decimal[] Q2)
        {
            var Q = new[] {
                Q1[0]*Q2[0] - Q1[1]*Q2[1] - Q1[2]*Q2[2] - Q1[3]*Q2[3],
                Q1[0]*Q2[1] + Q1[1]*Q2[0] + Q1[2]*Q2[3] - Q1[3]*Q2[2],
                Q1[0]*Q2[2] + Q1[2]*Q2[0] - Q1[1]*Q2[3] + Q1[3]*Q2[1],
                Q1[0]*Q2[3] + Q1[1]*Q2[2] - Q1[2]*Q2[1] + Q1[3]*Q2[0],
                0,
                Q1[5] + Q2[5] + Q1[2]*(Q1[1]*Q2[6] - Q1[2]*Q2[5])*2 + Q1[0]*(Q1[2]*Q2[7] - Q1[3]*Q2[6])*2 + Q1[3]*(Q1[1]*Q2[7] - Q1[3]*Q2[5])*2,
                Q1[6] + Q2[6] - Q1[1]*(Q1[1]*Q2[6] - Q1[2]*Q2[5])*2 - Q1[0]*(Q1[1]*Q2[7] - Q1[3]*Q2[5])*2 + Q1[3]*(Q1[2]*Q2[7] - Q1[3]*Q2[6])*2,
                Q1[7] + Q2[7] + Q1[0]*(Q1[1]*Q2[6] - Q1[2]*Q2[5])*2 - Q1[1]*(Q1[1]*Q2[7] - Q1[3]*Q2[5])*2 - Q1[2]*(Q1[2]*Q2[7] - Q1[3]*Q2[6])*2};

            return Q;
        }
        public decimal[] DQinv(decimal[] Q1)
        {
            decimal[] Q = new decimal[8];
            Q[0] = Q1[0];
            Q[1] = -Q1[1];
            Q[2] = -Q1[2];
            Q[3] = -Q1[3];
            Q[4] = 0;
            Q[5] = -Q1[5] - Q1[2] * (Q1[1] * Q1[6] - Q1[2] * Q1[5]) * 2 + Q1[0] * (Q1[2] * Q1[7] - Q1[3] * Q1[6]) * 2 - Q1[3] * (Q1[1] * Q1[7] - Q1[3] * Q1[5]) * 2;
            Q[6] = -Q1[6] + Q1[1] * (Q1[1] * Q1[6] - Q1[2] * Q1[5]) * 2 - Q1[0] * (Q1[1] * Q1[7] - Q1[3] * Q1[5]) * 2 - Q1[3] * (Q1[2] * Q1[7] - Q1[3] * Q1[6]) * 2;
            Q[7] = -Q1[7] + Q1[0] * (Q1[1] * Q1[6] - Q1[2] * Q1[5]) * 2 + Q1[1] * (Q1[1] * Q1[7] - Q1[3] * Q1[5]) * 2 + Q1[2] * (Q1[2] * Q1[7] - Q1[3] * Q1[6]) * 2;
            return Q;
        }

        public decimal[] toEulerianAngle(decimal[] quar)
        {

            var quar0 = quar[0];
            var quar1 = quar[1];
            var quar2 = quar[2];
            var quar3 = quar[3];
            //quar = quar.Normalize(2).ToArray();
            var output = new decimal[3];
            var ysqr = quar2 * quar2;

            // roll (x-axis rotation)
            var t0 = +2.0m * (quar0 * quar1 + quar2 * quar3);
            var t1 = +1.0m - 2.0m * (quar1 * quar1 + ysqr);
            output[0] = (BeckhoffContext.Atan3(t0, t1) * 180) / Convert.ToDecimal(Math.PI);

            // pitch (y-axis rotation)
            var t2 = +2.0m * (quar0 * quar2 - quar3 * quar1);
            t2 = t2 > 1.0m ? 1.0m : t2;
            t2 = t2 < -1.0m ? -1.0m : t2;
            output[1] = Convert.ToDecimal((Math.Asin((double)t2) * 180) / Math.PI);

            // yaw (z-axis rotation)
            var t3 = +2.0m * (quar0 * quar3 + quar1 * quar2);
            var t4 = +1.0m - 2.0m * (ysqr + quar3 * quar3);
            output[2] = (BeckhoffContext.Atan3(t3, t4) * 180) / Convert.ToDecimal(Math.PI);

            return output;
        }

        public decimal[] toQuaternion(decimal roll, decimal pitch, decimal yaw)
        {
            var q = new decimal[4];
            double t0 = Cos((double)yaw * 0.5);
            double t1 = Sin((double)yaw * 0.5);
            double t2 = Cos((double)roll * 0.5);
            double t3 = Sin((double)roll * 0.5);
            double t4 = Cos((double)pitch * 0.5);
            double t5 = Sin((double)pitch * 0.5);

            q[0] = Convert.ToDecimal(t0 * t2 * t4 + t1 * t3 * t5);
            q[1] = Convert.ToDecimal(t0 * t3 * t4 - t1 * t2 * t5);
            q[2] = Convert.ToDecimal(t0 * t2 * t5 + t1 * t3 * t4);
            q[3] = Convert.ToDecimal(t1 * t2 * t4 - t0 * t3 * t5);
            return q;
        }

        public DenseVector Slerp(double[] v0, double[] v1, double t)
        {
            // Compute the cosine of the angle between the two vectors.
            var Q0 = new DenseVector(v0);
            var Q1 = new DenseVector(v1);
            Q0 = Q0.Normalize(2).ToArray();
            Q1 = Q1.Normalize(2).ToArray();
            var dotP = Q0.DotProduct(Q1);

            const double DOT_THRESHOLD = 0.9995;
            if (Math.Abs(dotP) > DOT_THRESHOLD)
            {
                // If the inputs are too close for comfort, linearly interpolate
                // and normalize the result.

                DenseVector result = Q0.Add(t * (Q1.Subtract(Q0))).ToArray();
                result = result.Normalize(2).ToArray();
                return result;
            }

            // If the dot product is negative, the quaternions
            // have opposite handed-ness and slerp won't take
            // the shorter path. Fix by reversing one quaternion.
            if (dotP < 0.0f)
            {
                Q1 = -Q1;
                dotP = -dotP;
            }
            if (dotP < -1)
                dotP = -1;
            else if (dotP > 1)
                dotP = 1;
            double theta_0 = Math.Acos(dotP);  // theta_0 = angle between input vectors
            double theta = theta_0 * t;    // theta = angle between v0 and result 

            DenseVector Q2 = Q1.Subtract((-Q0).Multiply(dotP)).ToArray();
            Q2 = Q2.Normalize(2).ToArray();              // { v0, v2 } is now an orthonormal basis

            return Q0.Multiply(Math.Cos(theta)).Add(Q2.Multiply(Math.Sin(theta))).ToArray();
        }

        public TrajectoryPointList<decimal> SingleAxisTraj(TrajectoryPoint p0, TrajectoryPoint p1, double vmax, double amax, double jmax, double TS, double landa)
        {
            TrajectoryPointList<decimal> trjp = new TrajectoryPointList<decimal>();
            //double[] q = new double[100000];
            //double[] v = new double[100000];
            //double[] a = new double[100000];
            double vlim, alima, alimd;
            if (p1.Q - p0.Q == 0)
            {
                return null;
            }
            int sigma = 1;
            if (p1.Q < p0.Q)
            {
                sigma = -1;
                p0.Q *= -1;
                p0.V *= -1;
                p1.Q *= -1;
                p1.V *= -1;
            }
            double Tj1, Ta, Tj2, Td;
            if ((vmax - p0.V) * jmax < (amax * amax))
            {
                Tj1 = Sqrt((vmax - p0.V) / jmax);
                Ta = 2 * Tj1;
            }
            else
            {
                Tj1 = amax / jmax;
                Ta = Tj1 + (vmax - p0.V) / amax;
            }

            if ((vmax - p1.V) * jmax < (amax * amax))
            {
                Tj2 = Sqrt((vmax - p1.V) / jmax);
                Td = 2 * Tj2;
            }
            else
            {
                Tj2 = amax / jmax;
                Td = Tj2 + (vmax - p1.V) / amax;
            }
            var Tv = (p1.Q - p0.Q) / vmax - (Ta / 2) * (1 + p0.V / vmax) - (Td / 2) * (1 + p1.V / vmax);
            if (Tv < 0)
            {
                Tv = 0;
                while (true)
                {
                    Tj1 = Tj2 = amax / jmax;
                    var amax4 = Pow(amax, 4);
                    var delta = (amax4 / (jmax * jmax)) + 2 * (p0.V * p0.V + p1.V * p1.V) + amax * (4 * (p1.Q - p0.Q) - 2 * (amax / jmax) * (p0.V + p1.V));
                    Ta = ((amax * amax / jmax) - 2 * p0.V + Sqrt(delta)) / (2 * amax);
                    Td = ((amax * amax / jmax) - 2 * p1.V + Sqrt(delta)) / (2 * amax);
                    if (Ta < 0)
                    {
                        Ta = 0;
                        Td = 2 * (p1.Q - p0.Q) / (p1.V + p0.V);
                        Tj2 = (jmax * (p1.Q - p0.Q) - Sqrt(jmax * (jmax * (p1.Q - p0.Q) * (p1.Q - p0.Q) + (p1.V + p0.V) * (p1.V + p0.V) * (p1.V - p0.V)))) / (jmax * (p1.V + p0.V));
                    }
                    else if (Td < 0)
                    {
                        Td = 0;
                        Ta = 2 * (p1.Q - p0.Q) / (p1.V + p0.V);
                        Tj1 = (jmax * (p1.Q - p0.Q) - Sqrt(jmax * (jmax * (p1.Q - p0.Q) * (p1.Q - p0.Q) + (p1.V + p0.V) * (p1.V + p0.V) * (p1.V - p0.V)))) / (jmax * (p1.V + p0.V));
                    }

                    if (Ta >= 2 * Tj1 && Td >= 2 * Tj2)
                    {
                        break;
                    }
                    else
                        amax = landa * amax;
                }
                alima = jmax * Tj1;
                alimd = -jmax * Tj2;
                vlim = p0.V + (Ta - Tj1) * alima;
            }
            else
            {
                alima = amax;
                alimd = -amax;
                vlim = vmax;
            }
            double dur = Ta + Tv + Td;
            double jmin = -jmax;
            int i = 0;
            double t = 0;
            while (t <= dur)
            {
                if (t < Tj1)
                {
                    trjp.AddPoint(
                       Convert.ToDecimal(p0.Q + p0.V * t + jmax * t * t * t / 6),
                        Convert.ToDecimal(p0.V + jmax * t * t / 2),
                        Convert.ToDecimal(jmax * t));

                }
                else if (t < Ta - Tj1)
                {
                    trjp.AddPoint(
                         Convert.ToDecimal(p0.Q + p0.V * t + alima * (3 * t * t - 3 * Tj1 * t + Tj1 * Tj1) / 6),
                         Convert.ToDecimal(p0.V + alima * (t - Tj1 / 2)),
                         Convert.ToDecimal(alima));
                }
                else if (t < Ta)
                {
                    trjp.AddPoint(
                        Convert.ToDecimal(p0.Q + (vlim + p0.V) * Ta / 2 - vlim * (Ta - t) - jmin * (Ta - t) * (Ta - t) * (Ta - t) / 6),
                        Convert.ToDecimal(vlim + jmin * (Ta - t) * (Ta - t) / 2),
                        Convert.ToDecimal(-jmin * (Ta - t)));
                }
                else if (t < Ta + Tv)
                {
                    trjp.AddPoint(
                        Convert.ToDecimal(p0.Q + (vlim + p0.V) * Ta / 2 + vlim * (t - Ta)),
                        Convert.ToDecimal(vlim),
                        0);
                }
                else if (t < dur - Ta + Tj2)
                {
                    trjp.AddPoint(
                        Convert.ToDecimal(p1.Q - (vlim + p1.V) * Td / 2 + vlim * (t - dur + Td) -
                        jmax * (t - dur + Td) * (t - dur + Td) * (t - dur + Td) / 6),
                        Convert.ToDecimal(vlim - jmax * (t - dur + Td) * (t - dur + Td) / 2),
                        Convert.ToDecimal(-jmax * (t - dur + Td)));
                }
                else if (t < dur - Tj2)
                {
                    trjp.AddPoint(
                        Convert.ToDecimal(p1.Q - (vlim + p1.V) * Td / 2 + vlim * (t - dur + Td) +
                        alimd * (3 * (t - dur + Td) * (t - dur + Td) - 3 * Tj2 * (t - dur + Td) + Tj2 * Tj2) / 6),
                        Convert.ToDecimal(vlim + alimd * (t - dur + Td - Tj2 / 2)),
                        Convert.ToDecimal(alimd));
                }
                else if (t <= dur)
                {
                    trjp.AddPoint(
                        Convert.ToDecimal(p1.Q - p1.V * (dur - t) - (jmax * (dur - t) * (dur - t) * (dur - t) / 6)),
                        Convert.ToDecimal(p1.V + (jmax * (dur - t) * (dur - t) / 2)),
                        Convert.ToDecimal(-jmax * (dur - t)));
                }
                trjp.q[i] *= sigma;
                trjp.v[i] *= sigma;
                trjp.a[i] *= sigma;
                t += TS;
                i++;
            }
            if (p1.Q >= p0.Q)
            {
                p0.Q *= -1;
                p0.V *= -1;
                p1.Q *= -1;
                p1.V *= -1;
            }
            if (trjp == null)
                trjp = new TrajectoryPointList<decimal>();
            //trjp.FillTraj(q, v, a, i - 1);
            return trjp;
        }
        public TrajectoryPointList<decimal>[] MultiAxisTraj(TrajectoryPoint[] p0, TrajectoryPoint[] p1, double[] vmax, double[] amax, double[] jmax, double TS, double landa)
        {
            var trjp = new TrajectoryPointList<decimal>[6];
            for (int i = 0; i < 6; i++)
            {
                TrajectoryPoint p00 = new TrajectoryPoint(p0[i].Q, p0[i].V);
                TrajectoryPoint p11 = new TrajectoryPoint(p1[i].Q, p1[i].V);
                trjp[i] = SingleAxisTraj(p00, p11, vmax[i], amax[i], jmax[i], TS, landa);
            }
            return trjp;
        }
        //public TrajectoryPoints[] PTP(double[] ActualPos, List<string> keys, List<double> vals)
        //{
        //    var wmax_def = 300;
        //    var almax_def = 100; // Rotational Acceleration
        //    var gamax_def = 250;   // Rotational Jerk
        //    var wmax = Min(vals[keys.IndexOf("F")], wmax_def);
        //    TrajectoryPoints[] outputs = new TrajectoryPoints[6];
        //    var X0 = new TrajectoryPoint[6];

        //    for (int i = 0; i < 6; i++)
        //    {
        //        X0[i] = new TrajectoryPoint(ActualPos[i], 0);
        //    }

        //    var X1 = new TrajectoryPoint[6];
        //    var wmaxS = new double[6];
        //    var almaxS = new double[6];
        //    var gamaxS = new double[6];

        //    for (int i = 0; i < 6; i++)
        //    {
        //        X1[i] = new TrajectoryPoint(vals[i], 0);
        //        wmaxS[i] = wmax;
        //        almaxS[i] = almax_def;
        //        gamaxS[i] = gamax_def;
        //    }

        //    var points = new TrajectoryPoints[6];
        //    //points = MultiAxisTraj(X0, X1, wmaxS, almaxS, gamaxS, .001, .999, points);
        //    var maxLength = 0;
        //    if (points[0] == null)
        //        points[0] = new TrajectoryPoints();
        //    for (int i = 1; i < 6; i++)
        //    {

        //        if (points[i] == null)
        //            points[i] = new TrajectoryPoints();
        //        if (points[i].TrajLength > points[maxLength].TrajLength)
        //            maxLength = i;
        //    }
        //    for (int i = 0; i < 6; i++)
        //    {
        //        if (outputs[i] == null)
        //            outputs[i] = new TrajectoryPoints();
        //        var ratio = (X1[i].Q - X0[i].Q) / (X1[maxLength].Q - X0[maxLength].Q);
        //        for (int j = 0; j < points[maxLength].TrajLength; j++)
        //            outputs[i].q[j] = X0[i].Q + (points[maxLength].q[j] - X0[maxLength].Q) * ratio;

        //        outputs[i].TrajLength = points[maxLength].TrajLength;
        //    }

        //    return outputs;

        //    //double[] vmaxNew = new double[6];
        //    //double[] amaxNew = new double[6];
        //    //double[] jmaxNew = new double[6];
        //    //for (int i = 0; i < 6; i++)
        //    //{
        //    //    vmaxNew[i] = Min(vmax, vmax_default[i]);
        //    //}
        //    //MultiAxisTraj(p0, p1, vmaxNew, amax_default, jmax_default, TS, landa, trjp);
        //    //int maxLen = -1;
        //    //for (int i = 0; i < 6; i++)
        //    //{
        //    //    maxLen = Max(trjp[i].TrajLength, maxLen);
        //    //}
        //    //double[] alpha = new double[6];
        //    //for (int i = 0; i < 6; i++)
        //    //{
        //    //    alpha[i] = (trjp[i].TrajLength - 1) / (double)(maxLen - 1);
        //    //    vmaxNew[i] = alpha[i] * vmaxNew[i];
        //    //    amaxNew[i] = alpha[i] * alpha[i] * amax_default[i];
        //    //    jmaxNew[i] = alpha[i] * alpha[i] * alpha[i] * jmax_default[i];
        //    //}
        //    //MultiAxisTraj(p0, p1, vmaxNew, amaxNew, jmaxNew, TS, landa, trjp);
        //    //return true;
        //}

        public TrajectoryPointList<decimal>[] PTPList(decimal[] ActualPos, List<string> keys, List<decimal> vals)
        {
            var wmax_def = 300;
            var almax_def = 100; // Rotational Acceleration
            var gamax_def = 250; // Rotational Jerk
            var wmax = Min(vals[keys.IndexOf("F")], wmax_def);
            TrajectoryPointList<decimal>[] outputs = new TrajectoryPointList<decimal>[6];
            var X0 = new TrajectoryPoint[6];

            for (int i = 0; i < 6; i++)
            {
                X0[i] = new TrajectoryPoint((double)ActualPos[i], 0);
            }

            var X1 = new TrajectoryPoint[6];
            var wmaxS = new double[6];
            var almaxS = new double[6];
            var gamaxS = new double[6];

            for (int i = 0; i < 6; i++)
            {
                X1[i] = new TrajectoryPoint((double)vals[i], 0);
                wmaxS[i] = (double)wmax;
                almaxS[i] = almax_def;
                gamaxS[i] = gamax_def;
            }

            var points = MultiAxisTraj(X0, X1, wmaxS, almaxS, gamaxS, .001, .999);
            var maxLength = 0;
            if (points[0] == null)
                points[0] = new TrajectoryPointList<decimal>();
            for (int i = 1; i < 6; i++)
            {

                if (points[i] == null)
                    points[i] = new TrajectoryPointList<decimal>();
                if (points[i].TrajLength > points[maxLength].TrajLength)
                    maxLength = i;
            }
            for (int i = 0; i < 6; i++)
            {
                if (outputs[i] == null)
                    outputs[i] = new TrajectoryPointList<decimal>();
                var ratio = (X1[i].Q - X0[i].Q) / (X1[maxLength].Q - X0[maxLength].Q);
                for (int j = 0; j < points[maxLength].TrajLength; j++)
                    outputs[i].AddPoint(Convert.ToDecimal( X0[i].Q + ((double)points[maxLength].q[j] - X0[maxLength].Q) * ratio), 0, 0);
                // outputs[i].q[j] = X0[i].Q + (points[maxLength].q[j] - X0[maxLength].Q) * ratio;

                outputs[i].TrajLength = points[maxLength].TrajLength;
            }

            return outputs;

            //double[] vmaxNew = new double[6];
            //double[] amaxNew = new double[6];
            //double[] jmaxNew = new double[6];
            //for (int i = 0; i < 6; i++)
            //{
            //    vmaxNew[i] = Min(vmax, vmax_default[i]);
            //}
            //MultiAxisTraj(p0, p1, vmaxNew, amax_default, jmax_default, TS, landa, trjp);
            //int maxLen = -1;
            //for (int i = 0; i < 6; i++)
            //{
            //    maxLen = Max(trjp[i].TrajLength, maxLen);
            //}
            //double[] alpha = new double[6];
            //for (int i = 0; i < 6; i++)
            //{
            //    alpha[i] = (trjp[i].TrajLength - 1) / (double)(maxLen - 1);
            //    vmaxNew[i] = alpha[i] * vmaxNew[i];
            //    amaxNew[i] = alpha[i] * alpha[i] * amax_default[i];
            //    jmaxNew[i] = alpha[i] * alpha[i] * alpha[i] * jmax_default[i];
            //}
            //MultiAxisTraj(p0, p1, vmaxNew, amaxNew, jmaxNew, TS, landa, trjp);
            //return true;
        }

        //public double[][] InvKin(double[] QG)
        //{
        //    var QW=DQmultiply(QG,DQinv())















        //    return null;

        //} 
        public TrajectoryPoints[] LIN(double[] ActualPos, List<string> keys, List<double> vals, TrajectoryPoints[] outputs)
        {

            var vmax_def = 10;
            var amax_def = 100;
            var jmax_def = 1000;

            var wmax_def = 10;
            var almax_def = 100; // Rotational Acceleration
            var gamax_def = 1000;   // Rotational Jerk

            var vmax = Min(vals[keys.IndexOf("F")], vmax_def);
            var wmax = Min(10, wmax_def);

            //[X, vX, aX, jX, time1, Y, vY, aY, jY, time2, Z, vZ, aZ, jZ, time3, Ro, wRo, alRo, gaRo4, time4, Pi, wPi, alPi, gaPi, time5, Ya, wYa, alYa, gaYa, time6]=Multi_Axis_7Segment_Trajectory(X0, X1,0,0, vmax, amax, jmax, Y0, Y1,0,0, vmax, amax, jmax, Z0, Z1,0,0, vmax, amax, jmax, Ro0, Ro1,0,0, wmax, almax, gamax, Pi0, Pi1,0,0, wmax, almax, gamax, Ya0, Ya1,0,0, wmax, almax, gamax);
            var X0 = new TrajectoryPoint[6];
            for (int i = 0; i < 6; i++)
            {
                X0[i] = new TrajectoryPoint(ActualPos[i], 0);
            }
            var X1 = new TrajectoryPoint[6];
            var vmaxS = new double[6];
            var amaxS = new double[6];
            var jmaxS = new double[6];
            for (int i = 0; i < 3; i++)
            {
                X1[i] = new TrajectoryPoint(vals[i], 0);
                vmaxS[i] = vmax;
                jmaxS[i] = 1000;
                amaxS[i] = 100;
            }
            for (int i = 3; i < 6; i++)
            {
                X1[i] = new TrajectoryPoint(vals[i], 0);
                vmaxS[i] = wmax;
                jmaxS[i] = 1000;
                amaxS[i] = 100;
            }
            var points = new TrajectoryPoints[6];
            //points = MultiAxisTraj(X0, X1, vmaxS, amaxS, jmaxS, .001, .999, points);
            double timemax = points[0].TrajLength;
            for (int i = 1; i < 6; i++)
                if (points[i].TrajLength > timemax)
                    timemax = points[i].TrajLength;

            var alpha1 = points[0].TrajLength / timemax;
            var alpha2 = points[1].TrajLength / timemax;
            var alpha3 = points[2].TrajLength / timemax;
            var alpha4 = points[3].TrajLength / timemax;
            var alpha5 = points[4].TrajLength / timemax;
            var alpha6 = points[5].TrajLength / timemax;

            var vXmax = alpha1 * vmax;
            var aXmax = Pow(alpha1, 2) * amax_def;
            var jXmax = Pow(alpha1, 3) * jmax_def;
            var vYmax = alpha2 * vmax;
            var aYmax = Pow(alpha2, 2) * amax_def;
            var jYmax = Pow(alpha2, 3) * jmax_def;
            var vZmax = alpha3 * vmax;
            var aZmax = Pow(alpha3, 2) * amax_def;
            var jZmax = Pow(alpha3, 3) * jmax_def;
            var wRomax = alpha4 * wmax;
            var alRomax = Pow(alpha4, 2) * almax_def;
            var gaRomax = Pow(alpha4, 3) * gamax_def;
            var wPimax = alpha5 * wmax;
            var alPimax = Pow(alpha5, 2) * almax_def;
            var gaPimax = Pow(alpha5, 3) * gamax_def;
            var wYamax = alpha6 * wmax;
            var alYamax = Pow(alpha6, 2) * almax_def;
            var gaYamax = Pow(alpha6, 3) * gamax_def;

            //[X, vX, aX, jX, time1, Y, vY, aY, jY, time2, Z, vZ, aZ, jZ, time3, Ro, wRo, alRo, gaRo, time4, Pi, wPi, alPi, gaPi, time5, Ya, wYa, alYa, gaYa, time6]=Multi_Axis_7Segment_Trajectory(X0, X1,0,0, vXmax, aXmax, jXmax, Y0, Y1,0,0, vYmax, aYmax, jYmax, Z0, Z1,0,0, vZmax, aZmax, jZmax, Ro0, Ro1,0,0, wRomax, alRomax, gaRomax, Pi0, Pi1,0,0, wPimax, alPimax, gaPimax, Ya0, Ya1,0,0, wYamax, alYamax, gaYamax);
            var vmaxx = new double[] { vXmax, vYmax, vZmax, wRomax, wPimax, wYamax };
            var amaxx = new double[] { aXmax, aYmax, aZmax, alRomax, alPimax, alYamax };
            var jmaxx = new double[] { jXmax, jYmax, jZmax, gaRomax, gaPimax, gaYamax };
            //outputs = MultiAxisTraj(X0, X1, vmaxx, amaxx, jmaxx, .001, .999, outputs);
            for (int i = 0; i < 6; i++)
            {
                if (outputs[i].TrajLength < timemax)
                {
                    for (int j = outputs[i].TrajLength; j < timemax; j++)
                    {
                        outputs[i].q[j] = outputs[i].q[outputs[i].TrajLength];
                    }
                    outputs[i].TrajLength = (int)timemax;
                }
            }
            return outputs;
        }

        //public TrajectoryPointList[] RotationCurrection(TrajectoryPointList[] data)
        //{
        //    for (int i = 1; i < data[3].TrajLength; i++)
        //    {
        //        if (Abs(data[3].q[i] - data[3].q[i - 1]) > PI-.0001)
        //        {
        //            var sign = -Sign(data[3].q[i] - data[3].q[i - 1]);
        //            data[3].q[i] += sign * 2 * PI;
        //            i--;
        //        }
        //    }
        //    for (int i = 1; i < data[5].TrajLength; i++)
        //    {
        //        if (Abs(data[5].q[i] - data[5].q[i - 1]) > PI - .0001)
        //        {
        //            var sign = -Sign(data[5].q[i] - data[5].q[i - 1]);
        //            data[5].q[i] += sign * 2 * PI;
        //            i--;
        //        }
        //    }
        //    return data;
        //}
    }
}
