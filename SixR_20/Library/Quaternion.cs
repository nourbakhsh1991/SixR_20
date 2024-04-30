using System;
using MathNet.Numerics.LinearAlgebra.Double;

namespace SixR_20.Library
{
    #region License
    /*
    MIT License
    Copyright Â© 2006 The Mono.Xna Team

    All rights reserved.

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
    */
    #endregion License

    public class Quaternion : IEquatable<Quaternion>
    {
        public double X;
        public double Y;
        public double Z;
        public double W;

        public Quaternion()
        {
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
            this.W = 1;
        }

        public Quaternion(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public Quaternion(Vector3 vectorPart, double scalarPart)
        {
            X = vectorPart.X;
            Y = vectorPart.Y;
            Z = vectorPart.Z;
            W = scalarPart;
        }

        public static Quaternion Identity { get; } = new Quaternion(0, 0, 0, 1);


        public static Quaternion Zero { get; } = new Quaternion(0, 0, 0, 0);

        public static Quaternion Add(Quaternion quaternion1, Quaternion quaternion2)
        {
            return new Quaternion
            {
                X = quaternion1.X + quaternion2.X,
                Y = quaternion1.Y + quaternion2.Y,
                Z = quaternion1.Z + quaternion2.Z,
                W = quaternion1.W + quaternion2.W
            };
        }

        public static void Add(ref Quaternion quaternion1, ref Quaternion quaternion2, out Quaternion result)
        {
            result = new Quaternion
            {
                X = quaternion1.X + quaternion2.X,
                Y = quaternion1.Y + quaternion2.Y,
                Z = quaternion1.Z + quaternion2.Z,
                W = quaternion1.W + quaternion2.W
            };
        }

        public static Quaternion Concatenate(Quaternion value1, Quaternion value2)
        {
            var x = value2.X;
            var y = value2.Y;
            var z = value2.Z;
            var w = value2.W;
            var num4 = value1.X;
            var num3 = value1.Y;
            var num2 = value1.Z;
            var num = value1.W;
            var num12 = (y * num2) - (z * num3);
            var num11 = (z * num4) - (x * num2);
            var num10 = (x * num3) - (y * num4);
            var num9 = ((x * num4) + (y * num3)) + (z * num2);
            return new Quaternion
            {
                X = ((x * num) + (num4 * w)) + num12,
                Y = ((y * num) + (num3 * w)) + num11,
                Z = ((z * num) + (num2 * w)) + num10,
                W = (w * num) - num9
            };
        }

        public static void Concatenate(ref Quaternion value1, ref Quaternion value2, out Quaternion result)
        {
            var x = value2.X;
            var y = value2.Y;
            var z = value2.Z;
            var w = value2.W;
            var num4 = value1.X;
            var num3 = value1.Y;
            var num2 = value1.Z;
            var num = value1.W;
            var num12 = (y * num2) - (z * num3);
            var num11 = (z * num4) - (x * num2);
            var num10 = (x * num3) - (y * num4);
            var num9 = ((x * num4) + (y * num3)) + (z * num2);
            result = new Quaternion
            {
                X = ((x * num) + (num4 * w)) + num12,
                Y = ((y * num) + (num3 * w)) + num11,
                Z = ((z * num) + (num2 * w)) + num10,
                W = (w * num) - num9
            };
        }

        public void Conjugate()
        {
            this.X = -this.X;
            this.Y = -this.Y;
            this.Z = -this.Z;
        }

        public static Quaternion Conjugate(Quaternion value)
        {
            return new Quaternion
            {
                X = -value.X,
                Y = -value.Y,
                Z = -value.Z,
                W = value.W
            };
        }

        public static void Conjugate(ref Quaternion value, out Quaternion result)
        {
            result = new Quaternion
            {
                X = -value.X,
                Y = -value.Y,
                Z = -value.Z,
                W = value.W
            };
        }

        public static Quaternion CreateFromAxisAngle(Vector3 axis, double angle)
        {
            var num2 = angle * 0.5f;
            var num = (double)Math.Sin((double)num2);
            var num3 = (double)Math.Cos((double)num2);
            return new Quaternion
            {
                X = axis.X * num,
                Y = axis.Y * num,
                Z = axis.Z * num,
                W = num3
            };
        }


        public static void CreateFromAxisAngle(ref Vector3 axis, double angle, out Quaternion result)
        {
            var num2 = angle * 0.5f;
            var num = (double)Math.Sin((double)num2);
            var num3 = (double)Math.Cos((double)num2);
            result = new Quaternion
            {
                X = axis.X * num,
                Y = axis.Y * num,
                Z = axis.Z * num,
                W = num3
            };
        }

        public static Quaternion CreateFromRotationMatrix(MathNet.Numerics.LinearAlgebra.Double.DenseMatrix matrix)
        {
            var num8 = (matrix[1, 1] + matrix[2, 2]) + matrix[3, 3];
            var quaternion = new Quaternion();
            if (num8 > 0f)
            {
                var num = (double)Math.Sqrt((double)(num8 + 1f));
                quaternion.W = num * 0.5f;
                num = 0.5f / num;
                quaternion.X = (matrix[2, 3] - matrix[3, 2]) * num;
                quaternion.Y = (matrix[3, 1] - matrix[1, 3]) * num;
                quaternion.Z = (matrix[1, 2] - matrix[2, 1]) * num;
                return quaternion;
            }
            if ((matrix[1, 1] >= matrix[2, 2]) && (matrix[1, 1] >= matrix[3, 3]))
            {
                var num7 = (double)Math.Sqrt((double)(((1f + matrix[1, 1]) - matrix[2, 2]) - matrix[3, 3]));
                var num4 = 0.5f / num7;
                quaternion.X = 0.5f * num7;
                quaternion.Y = (matrix[1, 2] + matrix[2, 1]) * num4;
                quaternion.Z = (matrix[1, 3] + matrix[3, 1]) * num4;
                quaternion.W = (matrix[2, 3] - matrix[3, 2]) * num4;
                return quaternion;
            }
            if (matrix[2, 2] > matrix[3, 3])
            {
                var num6 = (double)Math.Sqrt((double)(((1f + matrix[2, 2]) - matrix[1, 1]) - matrix[3, 3]));
                var num3 = 0.5f / num6;
                quaternion.X = (matrix[2, 1] + matrix[1, 2]) * num3;
                quaternion.Y = 0.5f * num6;
                quaternion.Z = (matrix[3, 2] + matrix[2, 3]) * num3;
                quaternion.W = (matrix[3, 1] - matrix[1, 3]) * num3;
                return quaternion;
            }
            var num5 = (double)Math.Sqrt((double)(((1f + matrix[3, 3]) - matrix[1, 1]) - matrix[2, 2]));
            var num2 = 0.5f / num5;
            quaternion.X = (matrix[3, 1] + matrix[1, 3]) * num2;
            quaternion.Y = (matrix[3, 2] + matrix[2, 3]) * num2;
            quaternion.Z = 0.5f * num5;
            quaternion.W = (matrix[1, 2] - matrix[2, 1]) * num2;
            return quaternion;
        }

        public static void CreateFromRotationMatrix(ref MathNet.Numerics.LinearAlgebra.Double.DenseMatrix matrix, out Quaternion result)
        {
            result = new Quaternion();
            var num8 = (matrix[1, 1] + matrix[2, 2]) + matrix[3, 3];
            if (num8 > 0f)
            {
                var num = (double)Math.Sqrt((double)(num8 + 1f));
                result.W = num * 0.5f;
                num = 0.5f / num;
                result.X = (matrix[2, 3] - matrix[3, 2]) * num;
                result.Y = (matrix[3, 1] - matrix[1, 3]) * num;
                result.Z = (matrix[1, 2] - matrix[2, 1]) * num;
            }
            else if ((matrix[1, 1] >= matrix[2, 2]) && (matrix[1, 1] >= matrix[3, 3]))
            {
                var num7 = (double)Math.Sqrt((double)(((1f + matrix[1, 1]) - matrix[2, 2]) - matrix[3, 3]));
                var num4 = 0.5f / num7;
                result.X = 0.5f * num7;
                result.Y = (matrix[1, 2] + matrix[2, 1]) * num4;
                result.Z = (matrix[1, 3] + matrix[3, 1]) * num4;
                result.W = (matrix[2, 3] - matrix[3, 2]) * num4;
            }
            else if (matrix[2, 2] > matrix[3, 3])
            {
                var num6 = (double)Math.Sqrt((double)(((1f + matrix[2, 2]) - matrix[1, 1]) - matrix[3, 3]));
                var num3 = 0.5f / num6;
                result.X = (matrix[2, 1] + matrix[1, 2]) * num3;
                result.Y = 0.5f * num6;
                result.Z = (matrix[3, 2] + matrix[2, 3]) * num3;
                result.W = (matrix[3, 1] - matrix[1, 3]) * num3;
            }
            else
            {
                var num5 = (double)Math.Sqrt((double)(((1f + matrix[3, 3]) - matrix[1, 1]) - matrix[2, 2]));
                var num2 = 0.5f / num5;
                result.X = (matrix[3, 1] + matrix[1, 3]) * num2;
                result.Y = (matrix[3, 2] + matrix[2, 3]) * num2;
                result.Z = 0.5f * num5;
                result.W = (matrix[1, 2] - matrix[2, 1]) * num2;
            }
        }

        public static Quaternion CreateFromYawPitchRoll(double yaw, double pitch, double roll)
        {
            var num9 = roll * 0.5f;
            var num6 = (double)Math.Sin((double)num9);
            var num5 = (double)Math.Cos((double)num9);
            var num8 = pitch * 0.5f;
            var num4 = (double)Math.Sin((double)num8);
            var num3 = (double)Math.Cos((double)num8);
            var num7 = yaw * 0.5f;
            var num2 = (double)Math.Sin((double)num7);
            var num = (double)Math.Cos((double)num7);
            return new Quaternion
            {
                X = ((num * num4) * num5) + ((num2 * num3) * num6),
                Y = ((num2 * num3) * num5) - ((num * num4) * num6),
                Z = ((num * num3) * num6) - ((num2 * num4) * num5),
                W = ((num * num3) * num5) + ((num2 * num4) * num6)
            };
        }

        public static void CreateFromYawPitchRoll(double yaw, double pitch, double roll, out Quaternion result)
        {
            var num9 = roll * 0.5f;
            var num6 = (double)Math.Sin((double)num9);
            var num5 = (double)Math.Cos((double)num9);
            var num8 = pitch * 0.5f;
            var num4 = (double)Math.Sin((double)num8);
            var num3 = (double)Math.Cos((double)num8);
            var num7 = yaw * 0.5f;
            var num2 = (double)Math.Sin((double)num7);
            var num = (double)Math.Cos((double)num7);
            result = new Quaternion
            {
                X = ((num * num4) * num5) + ((num2 * num3) * num6),
                Y = ((num2 * num3) * num5) - ((num * num4) * num6),
                Z = ((num * num3) * num6) - ((num2 * num4) * num5),
                W = ((num * num3) * num5) + ((num2 * num4) * num6)
            };
        }

        public static Quaternion Divide(Quaternion quaternion1, Quaternion quaternion2)
        {
            double x = quaternion1.X;
            double y = quaternion1.Y;
            double z = quaternion1.Z;
            double w = quaternion1.W;
            double num14 = (((quaternion2.X * quaternion2.X) + (quaternion2.Y * quaternion2.Y)) + (quaternion2.Z * quaternion2.Z)) + (quaternion2.W * quaternion2.W);
            double num5 = 1f / num14;
            double num4 = -quaternion2.X * num5;
            double num3 = -quaternion2.Y * num5;
            double num2 = -quaternion2.Z * num5;
            double num = quaternion2.W * num5;
            double num13 = (y * num2) - (z * num3);
            double num12 = (z * num4) - (x * num2);
            double num11 = (x * num3) - (y * num4);
            double num10 = ((x * num4) + (y * num3)) + (z * num2);
            return new Quaternion
            {
                X = ((x * num) + (num4 * w)) + num13,
                Y = ((y * num) + (num3 * w)) + num12,
                Z = ((z * num) + (num2 * w)) + num11,
                W = (w * num) - num10
            };
        }

        public static void Divide(ref Quaternion quaternion1, ref Quaternion quaternion2, out Quaternion result)
        {
            double x = quaternion1.X;
            double y = quaternion1.Y;
            double z = quaternion1.Z;
            double w = quaternion1.W;
            double num14 = (((quaternion2.X * quaternion2.X) + (quaternion2.Y * quaternion2.Y)) + (quaternion2.Z * quaternion2.Z)) + (quaternion2.W * quaternion2.W);
            double num5 = 1f / num14;
            double num4 = -quaternion2.X * num5;
            double num3 = -quaternion2.Y * num5;
            double num2 = -quaternion2.Z * num5;
            double num = quaternion2.W * num5;
            double num13 = (y * num2) - (z * num3);
            double num12 = (z * num4) - (x * num2);
            double num11 = (x * num3) - (y * num4);
            double num10 = ((x * num4) + (y * num3)) + (z * num2);
            result = new Quaternion
            {
                X = ((x * num) + (num4 * w)) + num13,
                Y = ((y * num) + (num3 * w)) + num12,
                Z = ((z * num) + (num2 * w)) + num11,
                W = (w * num) - num10
            };
        }

        public static double Dot(Quaternion quaternion1, Quaternion quaternion2)
        {
            return ((((quaternion1.X * quaternion2.X) + (quaternion1.Y * quaternion2.Y)) + (quaternion1.Z * quaternion2.Z)) + (quaternion1.W * quaternion2.W));
        }

        public static void Dot(ref Quaternion quaternion1, ref Quaternion quaternion2, out double result)
        {
            result = (((quaternion1.X * quaternion2.X) + (quaternion1.Y * quaternion2.Y)) + (quaternion1.Z * quaternion2.Z)) + (quaternion1.W * quaternion2.W);
        }

        public override bool Equals(object obj)
        {
            bool flag = false;
            var other = obj as Quaternion;
            if (other != null)
            {
                flag = this.Equals(other);
            }
            return flag;
        }

        public bool Equals(Quaternion other)
        {
            var tolerance = .000001;
            return other != null &&
                   ((((Math.Abs(this.X - other.X) < tolerance) && (Math.Abs(this.Y - other.Y) < tolerance)) &&
                     (Math.Abs(this.Z - other.Z) < tolerance)) && (Math.Abs(this.W - other.W) < tolerance));
        }

        public override int GetHashCode()
        {
            return (((this.X.GetHashCode() + this.Y.GetHashCode()) + this.Z.GetHashCode()) + this.W.GetHashCode());
        }

        public static Quaternion Inverse(Quaternion quaternion)
        {

            double num2 = (((quaternion.X * quaternion.X) + (quaternion.Y * quaternion.Y)) + (quaternion.Z * quaternion.Z)) + (quaternion.W * quaternion.W);
            double num = 1f / num2;
            return new Quaternion
            {
                X = -quaternion.X * num,
                Y = -quaternion.Y * num,
                Z = -quaternion.Z * num,
                W = quaternion.W * num
            };
        }

        public static void Inverse(ref Quaternion quaternion, out Quaternion result)
        {
            double num2 = (((quaternion.X * quaternion.X) + (quaternion.Y * quaternion.Y)) + (quaternion.Z * quaternion.Z)) + (quaternion.W * quaternion.W);
            double num = 1f / num2;
            result = new Quaternion
            {
                X = -quaternion.X * num,
                Y = -quaternion.Y * num,
                Z = -quaternion.Z * num,
                W = quaternion.W * num
            };
        }

        public double Length()
        {
            double num = (((this.X * this.X) + (this.Y * this.Y)) + (this.Z * this.Z)) + (this.W * this.W);
            return (double)Math.Sqrt((double)num);
        }

        public double LengthSquared()
        {
            return ((((this.X * this.X) + (this.Y * this.Y)) + (this.Z * this.Z)) + (this.W * this.W));
        }

        public static Quaternion Lerp(Quaternion quaternion1, Quaternion quaternion2, double amount)
        {
            double num = amount;
            double num2 = 1f - num;
            Quaternion quaternion = new Quaternion();
            double num5 = (((quaternion1.X * quaternion2.X) + (quaternion1.Y * quaternion2.Y)) + (quaternion1.Z * quaternion2.Z)) + (quaternion1.W * quaternion2.W);
            if (num5 >= 0f)
            {
                quaternion.X = (num2 * quaternion1.X) + (num * quaternion2.X);
                quaternion.Y = (num2 * quaternion1.Y) + (num * quaternion2.Y);
                quaternion.Z = (num2 * quaternion1.Z) + (num * quaternion2.Z);
                quaternion.W = (num2 * quaternion1.W) + (num * quaternion2.W);
            }
            else
            {
                quaternion.X = (num2 * quaternion1.X) - (num * quaternion2.X);
                quaternion.Y = (num2 * quaternion1.Y) - (num * quaternion2.Y);
                quaternion.Z = (num2 * quaternion1.Z) - (num * quaternion2.Z);
                quaternion.W = (num2 * quaternion1.W) - (num * quaternion2.W);
            }
            double num4 = (((quaternion.X * quaternion.X) + (quaternion.Y * quaternion.Y)) + (quaternion.Z * quaternion.Z)) + (quaternion.W * quaternion.W);
            double num3 = 1f / ((double)Math.Sqrt((double)num4));
            quaternion.X *= num3;
            quaternion.Y *= num3;
            quaternion.Z *= num3;
            quaternion.W *= num3;
            return quaternion;
        }

        public static void Lerp(ref Quaternion quaternion1, ref Quaternion quaternion2, double amount, out Quaternion result)
        {
            result = new Quaternion();
            double num = amount;
            double num2 = 1f - num;
            double num5 = (((quaternion1.X * quaternion2.X) + (quaternion1.Y * quaternion2.Y)) + (quaternion1.Z * quaternion2.Z)) + (quaternion1.W * quaternion2.W);
            if (num5 >= 0f)
            {
                result.X = (num2 * quaternion1.X) + (num * quaternion2.X);
                result.Y = (num2 * quaternion1.Y) + (num * quaternion2.Y);
                result.Z = (num2 * quaternion1.Z) + (num * quaternion2.Z);
                result.W = (num2 * quaternion1.W) + (num * quaternion2.W);
            }
            else
            {
                result.X = (num2 * quaternion1.X) - (num * quaternion2.X);
                result.Y = (num2 * quaternion1.Y) - (num * quaternion2.Y);
                result.Z = (num2 * quaternion1.Z) - (num * quaternion2.Z);
                result.W = (num2 * quaternion1.W) - (num * quaternion2.W);
            }
            double num4 = (((result.X * result.X) + (result.Y * result.Y)) + (result.Z * result.Z)) + (result.W * result.W);
            double num3 = 1f / ((double)Math.Sqrt((double)num4));
            result.X *= num3;
            result.Y *= num3;
            result.Z *= num3;
            result.W *= num3;

        }

        public static Quaternion Slerp(Quaternion quaternion1, Quaternion quaternion2, double amount)
        {
            double num2;
            double num3;
            Quaternion quaternion = new Quaternion();
            double num = amount;
            double num4 = (((quaternion1.X * quaternion2.X) + (quaternion1.Y * quaternion2.Y)) + (quaternion1.Z * quaternion2.Z)) + (quaternion1.W * quaternion2.W);
            bool flag = false;
            if (num4 < 0f)
            {
                flag = true;
                num4 = -num4;
            }
            if (num4 > 0.999999f)
            {
                num3 = 1f - num;
                num2 = flag ? -num : num;
            }
            else
            {
                double num5 = (double)Math.Acos((double)num4);
                double num6 = (double)(1.0 / Math.Sin((double)num5));
                num3 = ((double)Math.Sin((double)((1f - num) * num5))) * num6;
                num2 = flag ? (((double)-Math.Sin((double)(num * num5))) * num6) : (((double)Math.Sin((double)(num * num5))) * num6);
            }
            quaternion.X = (num3 * quaternion1.X) + (num2 * quaternion2.X);
            quaternion.Y = (num3 * quaternion1.Y) + (num2 * quaternion2.Y);
            quaternion.Z = (num3 * quaternion1.Z) + (num2 * quaternion2.Z);
            quaternion.W = (num3 * quaternion1.W) + (num2 * quaternion2.W);
            return quaternion;
        }

        public static void Slerp(ref Quaternion quaternion1, ref Quaternion quaternion2, double amount, out Quaternion result)
        {
            double num2;
            double num3;
            result = new Quaternion();
            double num = amount;
            double num4 = (((quaternion1.X * quaternion2.X) + (quaternion1.Y * quaternion2.Y)) + (quaternion1.Z * quaternion2.Z)) + (quaternion1.W * quaternion2.W);
            bool flag = false;
            if (num4 < 0f)
            {
                flag = true;
                num4 = -num4;
            }
            if (num4 > 0.999999f)
            {
                num3 = 1f - num;
                num2 = flag ? -num : num;
            }
            else
            {
                double num5 = (double)Math.Acos((double)num4);
                double num6 = (double)(1.0 / Math.Sin((double)num5));
                num3 = ((double)Math.Sin((double)((1f - num) * num5))) * num6;
                num2 = flag ? (((double)-Math.Sin((double)(num * num5))) * num6) : (((double)Math.Sin((double)(num * num5))) * num6);
            }
            result.X = (num3 * quaternion1.X) + (num2 * quaternion2.X);
            result.Y = (num3 * quaternion1.Y) + (num2 * quaternion2.Y);
            result.Z = (num3 * quaternion1.Z) + (num2 * quaternion2.Z);
            result.W = (num3 * quaternion1.W) + (num2 * quaternion2.W);
        }

        public static Quaternion Subtract(Quaternion quaternion1, Quaternion quaternion2)
        {
            return new Quaternion
            {
                X = quaternion1.X - quaternion2.X,
                Y = quaternion1.Y - quaternion2.Y,
                Z = quaternion1.Z - quaternion2.Z,
                W = quaternion1.W - quaternion2.W
            };
        }

        public static void Subtract(ref Quaternion quaternion1, ref Quaternion quaternion2, out Quaternion result)
        {
            result = new Quaternion
            {
                X = quaternion1.X - quaternion2.X,
                Y = quaternion1.Y - quaternion2.Y,
                Z = quaternion1.Z - quaternion2.Z,
                W = quaternion1.W - quaternion2.W
            };
        }

        public static Quaternion Multiply(Quaternion quaternion1, Quaternion quaternion2)
        {
            double x = quaternion1.X;
            double y = quaternion1.Y;
            double z = quaternion1.Z;
            double w = quaternion1.W;
            double num4 = quaternion2.X;
            double num3 = quaternion2.Y;
            double num2 = quaternion2.Z;
            double num = quaternion2.W;
            double num12 = (y * num2) - (z * num3);
            double num11 = (z * num4) - (x * num2);
            double num10 = (x * num3) - (y * num4);
            double num9 = ((x * num4) + (y * num3)) + (z * num2);
            Quaternion quaternion = new Quaternion
            {
                X = ((x * num) + (num4 * w)) + num12,
                Y = ((y * num) + (num3 * w)) + num11,
                Z = ((z * num) + (num2 * w)) + num10,
                W = (w * num) - num9
            };
            return quaternion;
        }

        public static Quaternion Multiply(Quaternion quaternion1, double scaleFactor)
        {
            Quaternion quaternion = new Quaternion
            {
                X = quaternion1.X * scaleFactor,
                Y = quaternion1.Y * scaleFactor,
                Z = quaternion1.Z * scaleFactor,
                W = quaternion1.W * scaleFactor
            };
            return quaternion;
        }

        public static void Multiply(ref Quaternion quaternion1, double scaleFactor, out Quaternion result)
        {
            result = new Quaternion
            {
                X = quaternion1.X * scaleFactor,
                Y = quaternion1.Y * scaleFactor,
                Z = quaternion1.Z * scaleFactor,
                W = quaternion1.W * scaleFactor
            };
        }

        public static void Multiply(ref Quaternion quaternion1, ref Quaternion quaternion2, out Quaternion result)
        {
            double x = quaternion1.X;
            double y = quaternion1.Y;
            double z = quaternion1.Z;
            double w = quaternion1.W;
            double num4 = quaternion2.X;
            double num3 = quaternion2.Y;
            double num2 = quaternion2.Z;
            double num = quaternion2.W;
            double num12 = (y * num2) - (z * num3);
            double num11 = (z * num4) - (x * num2);
            double num10 = (x * num3) - (y * num4);
            double num9 = ((x * num4) + (y * num3)) + (z * num2);
            result = new Quaternion
            {
                X = ((x * num) + (num4 * w)) + num12,
                Y = ((y * num) + (num3 * w)) + num11,
                Z = ((z * num) + (num2 * w)) + num10,
                W = (w * num) - num9
            };
        }

        public static Quaternion Negate(Quaternion quaternion)
        {
            return new Quaternion
            {
                X = -quaternion.X,
                Y = -quaternion.Y,
                Z = -quaternion.Z,
                W = -quaternion.W
            };
        }

        public static void Negate(ref Quaternion quaternion, out Quaternion result)
        {
            result = new Quaternion
            {
                X = -quaternion.X,
                Y = -quaternion.Y,
                Z = -quaternion.Z,
                W = -quaternion.W
            };
        }

        public void Normalize()
        {
            double num2 = (((this.X * this.X) + (this.Y * this.Y)) + (this.Z * this.Z)) + (this.W * this.W);
            double num = 1f / ((double)Math.Sqrt((double)num2));
            this.X *= num;
            this.Y *= num;
            this.Z *= num;
            this.W *= num;
        }

        public static Quaternion Normalize(Quaternion quaternion)
        {
            double num2 = (((quaternion.X * quaternion.X) + (quaternion.Y * quaternion.Y)) + (quaternion.Z * quaternion.Z)) + (quaternion.W * quaternion.W);
            double num = 1f / ((double)Math.Sqrt((double)num2));
            return new Quaternion
            {
                X = quaternion.X * num,
                Y = quaternion.Y * num,
                Z = quaternion.Z * num,
                W = quaternion.W * num
            };
        }

        public static void Normalize(ref Quaternion quaternion, out Quaternion result)
        {
            double num2 = (((quaternion.X * quaternion.X) + (quaternion.Y * quaternion.Y)) + (quaternion.Z * quaternion.Z)) + (quaternion.W * quaternion.W);
            double num = 1f / ((double)Math.Sqrt((double)num2));
            result = new Quaternion
            {
                X = quaternion.X * num,
                Y = quaternion.Y * num,
                Z = quaternion.Z * num,
                W = quaternion.W * num
            };
        }

        public static Quaternion operator +(Quaternion quaternion1, Quaternion quaternion2)
        {
            return new Quaternion
            {
                X = quaternion1.X + quaternion2.X,
                Y = quaternion1.Y + quaternion2.Y,
                Z = quaternion1.Z + quaternion2.Z,
                W = quaternion1.W + quaternion2.W
            };
        }

        public static Quaternion operator /(Quaternion quaternion1, Quaternion quaternion2)
        {
            double x = quaternion1.X;
            double y = quaternion1.Y;
            double z = quaternion1.Z;
            double w = quaternion1.W;
            double num14 = (((quaternion2.X * quaternion2.X) + (quaternion2.Y * quaternion2.Y)) + (quaternion2.Z * quaternion2.Z)) + (quaternion2.W * quaternion2.W);
            double num5 = 1f / num14;
            double num4 = -quaternion2.X * num5;
            double num3 = -quaternion2.Y * num5;
            double num2 = -quaternion2.Z * num5;
            double num = quaternion2.W * num5;
            double num13 = (y * num2) - (z * num3);
            double num12 = (z * num4) - (x * num2);
            double num11 = (x * num3) - (y * num4);
            double num10 = ((x * num4) + (y * num3)) + (z * num2);
            return new Quaternion
            {
                X = ((x * num) + (num4 * w)) + num13,
                Y = ((y * num) + (num3 * w)) + num12,
                Z = ((z * num) + (num2 * w)) + num11,
                W = (w * num) - num10
            };
        }

        public static bool operator ==(Quaternion quaternion1, Quaternion quaternion2)
        {
            var tolerance = .000001;
            return quaternion2 != null &&
                   (quaternion1 != null &&
                    ((((Math.Abs(quaternion1.X - quaternion2.X) < tolerance) &&
                       (Math.Abs(quaternion1.Y - quaternion2.Y) < tolerance)) &&
                      (Math.Abs(quaternion1.Z - quaternion2.Z) < tolerance)) &&
                     (Math.Abs(quaternion1.W - quaternion2.W) < tolerance)));
        }

        public static bool operator !=(Quaternion quaternion1, Quaternion quaternion2)
        {
            var tolerance = .000001;
            if (quaternion2 != null &&
                (quaternion1 != null &&
                 (((Math.Abs(quaternion1.X - quaternion2.X) < tolerance) &&
                   (Math.Abs(quaternion1.Y - quaternion2.Y) < tolerance)) &&
                  (Math.Abs(quaternion1.Z - quaternion2.Z) < tolerance))))
            {
                return (Math.Abs(quaternion1.W - quaternion2.W) > tolerance);
            }
            return true;
        }

        public static Quaternion operator *(Quaternion quaternion1, Quaternion quaternion2)
        {
            double x = quaternion1.X;
            double y = quaternion1.Y;
            double z = quaternion1.Z;
            double w = quaternion1.W;
            double num4 = quaternion2.X;
            double num3 = quaternion2.Y;
            double num2 = quaternion2.Z;
            double num = quaternion2.W;
            double num12 = (y * num2) - (z * num3);
            double num11 = (z * num4) - (x * num2);
            double num10 = (x * num3) - (y * num4);
            double num9 = ((x * num4) + (y * num3)) + (z * num2);
            return new Quaternion
            {
                X = ((x * num) + (num4 * w)) + num12,
                Y = ((y * num) + (num3 * w)) + num11,
                Z = ((z * num) + (num2 * w)) + num10,
                W = (w * num) - num9
            };
        }

        public static Quaternion operator *(Quaternion quaternion1, double scaleFactor)
        {
            return new Quaternion
            {
                X = quaternion1.X * scaleFactor,
                Y = quaternion1.Y * scaleFactor,
                Z = quaternion1.Z * scaleFactor,
                W = quaternion1.W * scaleFactor
            };
        }

        public static Quaternion operator -(Quaternion quaternion1, Quaternion quaternion2)
        {
            return new Quaternion
            {
                X = quaternion1.X - quaternion2.X,
                Y = quaternion1.Y - quaternion2.Y,
                Z = quaternion1.Z - quaternion2.Z,
                W = quaternion1.W - quaternion2.W
            };
        }

        public static Quaternion operator -(Quaternion quaternion)
        {
            return new Quaternion
            {
                X = -quaternion.X,
                Y = -quaternion.Y,
                Z = -quaternion.Z,
                W = -quaternion.W
            };
        }

        public DenseVector ToEulerianAngle()
        {
            var output = new double[3];
            var ysqr = Y * Y;
            var tolerance = .00001;
            // roll (x-axis rotation)
            var t0 = +2.0 * (W * X + Y * Z);
            var t1 = +1.0 - 2.0 * (X * X + ysqr);
            if (Math.Abs(t1) < tolerance && Math.Abs(t0) < tolerance)
            {
                output[0] = Double.NaN;
            }
            else
            {
                output[0] = (Math.Atan(t0 / t1) * 180) / Math.PI;
                output[0] = ((output[0] + 180) % 360) - 180;
            }
            // pitch (y-axis rotation)
            var t2 = +2.0 * (W * Y - Z * X);
            t2 = t2 > 1.0 ? 1.0 : t2;
            t2 = t2 < -1.0 ? -1.0 : t2;
            output[1] = (Math.Asin(t2) * 180) / Math.PI;
            output[1] = ((output[1] + 180) % 360) - 180;
            // yaw (z-axis rotation)
            var t3 = +2.0 * (W * Z + X * Y);
            var t4 = +1.0 - 2.0 * (ysqr + Z * Z);
            if (Math.Abs(t3) < tolerance && Math.Abs(t4) < tolerance)
            {
                output[0] = Double.NaN;
            }
            else
            {
                output[2] = (Math.Atan(t3 / t4) * 180) / Math.PI;
                output[2] = ((output[2] + 180) % 360) - 180;
            }
            return output;
        }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(32);
            sb.Append("{X:");
            sb.Append(this.X.ToString("0.000"));
            sb.Append(" Y:");
            sb.Append(this.Y.ToString("0.000"));
            sb.Append(" Z:");
            sb.Append(this.Z.ToString("0.000"));
            sb.Append(" W:");
            sb.Append(this.W.ToString("0.000"));
            sb.Append("}");
            return sb.ToString();
        }

        internal MathNet.Numerics.LinearAlgebra.Double.DenseMatrix ToMatrix()
        {
            MathNet.Numerics.LinearAlgebra.Double.DenseMatrix matrix = MathNet.Numerics.LinearAlgebra.Double.DenseMatrix.CreateIdentity(4);
            ToMatrix(out matrix);
            return matrix;
        }

        internal void ToMatrix(out MathNet.Numerics.LinearAlgebra.Double.DenseMatrix matrix)
        {
            Quaternion.ToMatrix(this, out matrix);
        }

        internal static void ToMatrix(Quaternion quaternion, out MathNet.Numerics.LinearAlgebra.Double.DenseMatrix matrix)
        {
            // source -> http://content.gpwiki.org/index.php/OpenGL:Tutorials:Using_Quaternions_to_represent_rotation#Quaternion_to_Matrix
            double x2 = quaternion.X * quaternion.X;
            double y2 = quaternion.Y * quaternion.Y;
            double z2 = quaternion.Z * quaternion.Z;
            double xy = quaternion.X * quaternion.Y;
            double xz = quaternion.X * quaternion.Z;
            double yz = quaternion.Y * quaternion.Z;
            double wx = quaternion.W * quaternion.X;
            double wy = quaternion.W * quaternion.Y;
            double wz = quaternion.W * quaternion.Z;

            // This calculation would be a lot more complicated for non-unit length quaternions
            // Note: The constructor of Matrix4 expects the Matrix in column-major format like expected by
            //   OpenGL

            matrix = new DenseMatrix(4, 4)
            {
                [0, 0] = 1.0f - 2.0f * (y2 + z2),
                [0, 1] = 2.0f * (xy - wz),
                [0, 2] = 2.0f * (xz + wy),
                [0, 3] = 0.0f,
                [1, 0] = 2.0f * (xy + wz),
                [1, 1] = 1.0f - 2.0f * (x2 + z2),
                [1, 2] = 2.0f * (yz - wx),
                [1, 3] = 0.0f,
                [2, 0] = 2.0f * (xz - wy),
                [2, 1] = 2.0f * (yz + wx),
                [2, 2] = 1.0f - 2.0f * (x2 + y2),
                [2, 3] = 0.0f,
                [3, 0] = 0.0f,
                [3, 1] = 0.0f,
                [3, 2] = 0.0f,
                [3, 3] = 1.0f
            };
        }

        internal Vector3 Xyz
        {
            get
            {
                return new Vector3(X, Y, Z);
            }

            set
            {
                X = value.X;
                Y = value.Y;
                Z = value.Z;
            }
        }


    }
}
