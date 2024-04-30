using MathNet.Numerics.LinearAlgebra.Double;

namespace SixR_20.Library
{
    // --------------------------------------------------------------------------------------------------------------------
    // <copyright file="DualQuaternion.cs" company="Stéphane Pareilleux">
    // Copyright 2015 Stéphane Pareilleux
    //
    // This program is free software: you can redistribute it and/or modify
    // it under the terms of the GNU Lesser General Public License as published by
    // the Free Software Foundation, either version 3 of the License, or
    // (at your option) any later version.
    //
    // This program is distributed in the hope that it will be useful,
    // but WITHOUT ANY WARRANTY; without even the implied warranty of
    // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    // GNU General Public License for more details.
    //
    // You should have received a copy of the GNU General Public License
    // along with this program.  If not, see http://www.gnu.org/licenses/.
    // </copyright>
    // <summary>
    //   Ported from C to C#:
    //   - The C libdq library (See http://www.iri.upc.edu/people/esimo/code/libdq/)
    //
    //   Inspired from:
    //   - "A Beginners Guide to Dual-Quaternions" (See http://wscg.zcu.cz/wscg2012/short/a29-full.pdf)
    //   - SharpDX Quaternion class
    // </summary>
    // --------------------------------------------------------------------------------------------------------------------

    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.InteropServices;
    /// <summary>
    /// The dual quaternion.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public class DualQuaternion : IEquatable<DualQuaternion>, IFormattable
    {
        #region Fields

        const double tolerance = .000001;
        /// <summary>
        /// A dual quaternion with and identity default part and a zero dual part.
        /// </summary>
        public static readonly DualQuaternion Default = new DualQuaternion(Quaternion.Identity, Quaternion.Zero);

        /// <summary>
        /// A dual quaternion with all of its components set to zero.
        /// </summary>
        public static readonly DualQuaternion Zero = new DualQuaternion(0, 0, 0, 0, 0, 0, 0, 0);

        /// <summary>
        /// A dual quaternion that represents the origin point (X:0, Y:0, Z:0).
        /// </summary>
        public static readonly DualQuaternion OriginPoint = CreatePoint(Vector3.Zero);

        /// <summary>
        /// The real part of the dual quaternion.
        /// </summary>
        public Quaternion Real;

        /// <summary>
        /// The dual part of the dual quaternion.
        /// </summary>
        public Quaternion Dual;

        /// <summary>
        /// The format template.
        /// </summary>
        private const string FormatTemplate = "({0}) + ε({1})";

        /// <summary>
        /// Number digits used for rounding. 
        /// </summary>
        private static readonly int RoundingDigits = 3;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DualQuaternion"/> struct (From real and dual quaternions, the real gets normalized).
        /// </summary>
        /// <param name="real">
        /// The real.
        /// </param>
        /// <param name="dual">
        /// The dual.
        /// </param>
        public DualQuaternion(Quaternion real, Quaternion dual)
        {
            this.Real = Quaternion.Normalize(real);
            this.Dual = dual;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DualQuaternion"/> struct.
        /// </summary>
        /// <param name="realX">
        /// The real x.
        /// </param>
        /// <param name="realY">
        /// The real y.
        /// </param>
        /// <param name="realZ">
        /// The real z.
        /// </param>
        /// <param name="realW">
        /// The real w.
        /// </param>
        /// <param name="dualX">
        /// The dual x.
        /// </param>
        /// <param name="dualY">
        /// The dual y.
        /// </param>
        /// <param name="dualZ">
        /// The dual z.
        /// </param>
        /// <param name="dualW">
        /// The dual w.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Suppression is OK here.")]
        public DualQuaternion(double realX, double realY, double realZ, double realW, double dualX, double dualY, double dualZ, double dualW)
            : this(new Quaternion(realX, realY, realZ, realW), new Quaternion(dualX, dualY, dualZ, dualW))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DualQuaternion"/> struct.
        /// </summary>
        /// <param name="values">
        /// The values to assign to the real and dual {X, Y, Z, W} components of the dual quaternion. 
        /// This must be an array with eight elements.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// Thrown when <paramref name="values"/> is <c>null</c>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="values"/> contains more or less than eight elements.
        /// </exception>
        public DualQuaternion(IEnumerable<double> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values", "values are null.");
            }

            var valuesList = values as IList<double> ?? values.ToList();

            if (valuesList.Count != 8)
            {
                throw new ArgumentOutOfRangeException("values", "There must be four and only four input values for Quaternion.");
            }

            this.Real.X = valuesList[0];
            this.Real.Y = valuesList[1];
            this.Real.Z = valuesList[2];
            this.Real.W = valuesList[3];

            this.Dual.X = valuesList[4];
            this.Dual.Y = valuesList[5];
            this.Dual.Z = valuesList[6];
            this.Dual.W = valuesList[7];
        }

        public DualQuaternion()
        {
            this.Real = Quaternion.Identity;
            this.Dual = Quaternion.Zero;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the rotation from the dual quaternion.
        /// </summary>
        public Quaternion Rotation => this.Real;

        /// <summary>
        /// Gets the translation from the dual quaternion.
        /// </summary>
        public Vector3 Translation
        {
            get
            {
                var t = (this.Dual * 2.0f) * Quaternion.Conjugate(this.Real);
                return new Vector3(t.X, t.Y, t.Z);
            }
        }

        /// <summary>
        /// Gets the length of the dual quaternion.
        /// </summary>
        public double Length => Dot(this, this);

        /// <summary>
        /// Gets a value indicating whether the dual quaternion is a unit dual quaternion.
        /// </summary>
        public bool IsUnit
        {
            get
            {
                double realLengthSquared, dualLengthSquared;

                this.GetLengthSquared(out realLengthSquared, out dualLengthSquared);
                var tolerance = .000001;
                var result =
                    !(Math.Abs(realLengthSquared - 1) > tolerance) &&
                    !(Math.Abs(dualLengthSquared) > tolerance);

                return result;
            }
        }

        #endregion

        #region Indexer

        /// <summary>
        /// Gets or sets a dual quaternion component by index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="double"/> component of the dual quaternion.
        /// </returns>
        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.Real.X;
                    case 1:
                        return this.Real.Y;
                    case 2:
                        return this.Real.Z;
                    case 3:
                        return this.Real.W;
                    case 4:
                        return this.Dual.X;
                    case 5:
                        return this.Dual.Y;
                    case 6:
                        return this.Dual.Z;
                    case 7:
                        return this.Dual.W;
                    default:
                        throw new ArgumentOutOfRangeException("index", "Indices for DualQuaternion run from 0 to 7, inclusive.");
                }
            }

            set
            {
                switch (index)
                {
                    case 0:
                        this.Real.X = value;
                        break;
                    case 1:
                        this.Real.Y = value;
                        break;
                    case 2:
                        this.Real.Z = value;
                        break;
                    case 3:
                        this.Real.W = value;
                        break;
                    case 4:
                        this.Dual.X = value;
                        break;
                    case 5:
                        this.Dual.Y = value;
                        break;
                    case 6:
                        this.Dual.Z = value;
                        break;
                    case 7:
                        this.Dual.W = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("index", "Indices for DualQuaternion run from 0 to 7, inclusive.");
                }
            }
        }

        #endregion

        #region Operators

        /// <summary>
        /// Adds two dual quaternions.
        /// </summary>
        /// <param name="left">
        /// The left dual quaternion.
        /// </param>
        /// <param name="right">
        /// The second dual quaternion.
        /// </param>
        /// <returns>
        /// The sum of the two dual quaternions.
        /// </returns>
        public static DualQuaternion operator +(DualQuaternion left, DualQuaternion right)
        {
            var result = new DualQuaternion(left.Real + right.Real, left.Dual + right.Dual);

            return result;
        }

        /// <summary>
        /// Subtracts two dual quaternions.
        /// </summary>
        /// <param name="left">
        /// The left dual quaternion.
        /// </param>
        /// <param name="right">
        /// The second dual quaternion.
        /// </param>
        /// <returns>
        /// The difference of the two dual quaternions.
        /// </returns>
        public static DualQuaternion operator -(DualQuaternion left, DualQuaternion right)
        {
            var result = new DualQuaternion(left.Real - right.Real, left.Dual - right.Dual);

            return result;
        }

        /// <summary>
        /// Swaps the sign of all the elements in a dual quaternion.
        /// </summary>
        /// <param name="value">The dual quaternion to negate.</param>
        /// <returns>
        /// Result of swapping all values of the elements.
        /// </returns>
        public static DualQuaternion operator -(DualQuaternion value)
        {
            var result = new DualQuaternion();

            for (var i = 0; i < 8; i++)
            {
                result[i] = -value[i];
            }

            return result;
        }

        /// <summary>
        /// Applies a scale factor to a dual quaternion.
        /// </summary>
        /// <param name="value">
        /// The dual quaternion to scale.
        /// </param>
        /// <param name="scale">
        /// The amount by which to scale the dual quaternion.
        /// </param>
        /// <returns>
        /// The scaled dual quaternion.
        /// </returns>
        public static DualQuaternion operator *(DualQuaternion value, double scale)
        {
            var result = new DualQuaternion(value.Real * scale, value.Dual * scale);

            return result;
        }

        /// <summary>
        /// Applies a scale factor to a dual quaternion.
        /// </summary>
        /// <param name="scale">
        /// The amount by which to scale the dual quaternion.
        /// </param>
        /// <param name="value">
        /// The dual quaternion to scale.
        /// </param>
        /// <returns>
        /// The scaled dual quaternion.
        /// </returns>
        public static DualQuaternion operator *(double scale, DualQuaternion value)
        {
            var result = new DualQuaternion(value.Real * scale, value.Dual * scale);

            return result;
        }

        public DualQuaternion DQmultiply(DualQuaternion Q2)
        {

            var Q = new DualQuaternion(Real.W * Q2.Real.X + Real.X * Q2.Real.W + Real.Y * Q2.Real.Z - Real.Z * Q2.Real.Y,
                Real.W * Q2.Real.Y + Real.Y * Q2.Real.W - Real.X * Q2.Real.Z + Real.Z * Q2.Real.X,
                Real.W * Q2.Real.Z + Real.X * Q2.Real.Y - Real.Y * Q2.Real.X + Real.Z * Q2.Real.W,
                Real.W * Q2.Real.W - Real.X * Q2.Real.X - Real.Y * Q2.Real.Y - Real.Z * Q2.Real.Z,
                Dual.X + Q2.Dual.X + Real.Y * (Real.X * Q2.Dual.Y - Real.Y * Q2.Dual.X) * 2.0 + Real.W * (Real.Y * Q2.Dual.Z - Real.Z * Q2.Dual.Y) * 2.0 + Real.Z * (Real.X * Q2.Dual.Z - Real.Z * Q2.Dual.X) * 2.0,
                Dual.Y + Q2.Dual.Y - Real.X * (Real.X * Q2.Dual.Y - Real.Y * Q2.Dual.X) * 2.0 - Real.W * (Real.X * Q2.Dual.Z - Real.Z * Q2.Dual.X) * 2.0 + Real.X * (Real.Y * Q2.Dual.Z - Real.Z * Q2.Dual.Y) * 2.0,
                Dual.Z + Q2.Dual.Z + Real.X * (Real.X * Q2.Dual.Y - Real.Y * Q2.Dual.X) * 2.0 - Real.X * (Real.X * Q2.Dual.Z - Real.Z * Q2.Dual.X) * 2.0 - Real.Y * (Real.Y * Q2.Dual.Z - Real.Z * Q2.Dual.Y) * 2.0,
                0);
            /*
            var Q = new[] {
                Q1[0]*Q2[0] - Q1[1]*Q2[1] - Q1[2]*Q2[2] - Q1[3]*Q2[3],
                Q1[0]*Q2[1] + Q1[1]*Q2[0] + Q1[2]*Q2[3] - Q1[3]*Q2[2],
                Q1[0]*Q2[2] + Q1[2]*Q2[0] - Q1[1]*Q2[3] + Q1[3]*Q2[1],
                Q1[0]*Q2[3] + Q1[1]*Q2[2] - Q1[2]*Q2[1] + Q1[3]*Q2[0],
                0,
                Q1[5] + Q2[5] + Q1[2]*(Q1[1]*Q2[6] - Q1[2]*Q2[5])*2.0 + Q1[0]*(Q1[2]*Q2[7] - Q1[3]*Q2[6])*2.0 + Q1[3]*(Q1[1]*Q2[7] - Q1[3]*Q2[5])*2.0,
                Q1[6] + Q2[6] - Q1[1]*(Q1[1]*Q2[6] - Q1[2]*Q2[5])*2.0 - Q1[0]*(Q1[1]*Q2[7] - Q1[3]*Q2[5])*2.0 + Q1[3]*(Q1[2]*Q2[7] - Q1[3]*Q2[6])*2.0,
                Q1[7] + Q2[7] + Q1[0]*(Q1[1]*Q2[6] - Q1[2]*Q2[5])*2.0 - Q1[1]*(Q1[1]*Q2[7] - Q1[3]*Q2[5])*2.0 - Q1[2]*(Q1[2]*Q2[7] - Q1[3]*Q2[6])*2.0};

            return Q;/*
            var result = new DualQuaternion(value.Real * scale, value.Dual * scale);

            return result;*/
            return Q;
        }

        /// <summary>
        /// Multiplies a dual quaternion by another.
        /// </summary>
        /// <param name="left">
        /// The left dual quaternion.
        /// </param>
        /// <param name="right">
        /// The right dual quaternion.
        /// </param>
        /// <returns>
        /// The product of the two dual quaternions.
        /// </returns>
        public static DualQuaternion operator *(DualQuaternion left, DualQuaternion right)
        {
            ////  We can decompose the problem into quaternion multiplication:            
            ////  Q = q + εq0
            ////  P = p + εp0
            ////  Q*P = q*p + ε(q*p0 + q0*p) 

            var q = left.Real;
            var p = right.Real;
            var q0 = left.Dual;
            var p0 = right.Dual;

            var result = new DualQuaternion(q * p, (q * p0) + (q0 * p));

            return result;
        }

        /// <summary>
        /// Tests for equality between two dual quaternions.
        /// </summary>
        /// <param name="left">
        /// The first dual quaternion to compare.
        /// </param><param name="right">
        /// The second dual quaternion to compare.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="left"/> has the same value as <paramref name="right"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(DualQuaternion left, DualQuaternion right)
        {
            return left.Equals(ref right);
        }

        /// <summary>
        /// Tests for inequality between two dual quaternions.
        /// </summary>
        /// <param name="left">
        /// The first value to compare.</param><param name="right">The second value to compare.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="left"/> has a different value than <paramref name="right"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(DualQuaternion left, DualQuaternion right)
        {
            return !left.Equals(ref right);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a pure rotation dual quaternion.
        /// </summary>
        /// <param name="angle">
        /// Angle to rotate.
        /// </param>
        /// <param name="axis">
        /// Axis to rotate around (normalized vector).
        /// </param>
        /// <param name="point">
        /// Any point of the axis (to create Plücker coordinates).
        /// </param>
        /// <returns>
        /// The dual quaternion.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Suppression is OK here.")]
        public static DualQuaternion CreateRotation(double angle, Vector3 axis, Vector3 point)
        {
            // We do cross product with the line point and line vector to get the Plücker coordinates.
            var moment = Vector3.Cross(point, axis);
            var result = CreateRotationPlucker(angle, axis, moment);
            return result;
        }

        /// <summary>
        /// Creates a pure rotation dual quaternion using Plücker coordinates.
        /// </summary>
        /// <param name="angle">
        /// The angle to rotate.
        /// </param>
        /// <param name="axis">
        /// The axis to rotate around (normalized vector).
        /// </param>
        /// <param name="moment">
        /// Moment of the axis.
        /// </param>
        /// <returns>
        /// The <see cref="DualQuaternion"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Suppression is OK here.")]
        public static DualQuaternion CreateRotationPlucker(double angle, Vector3 axis, Vector3 moment)
        {
            var s = Adjust(Math.Sin(angle / 2f));
            var c = Adjust(Math.Cos(angle / 2f));

            var result = new DualQuaternion(
                (double)(s * axis.X),
                (double)(s * axis.Y),
                (double)(s * axis.Z),
                (double)c,
                (double)(s * moment.X),
                (double)(s * moment.Y),
                (double)(s * moment.Z),
                0);

            return result;
        }

        /// <summary>
        /// Creates a pure translation dual unit quaternion (1 + ε½t).
        /// </summary>
        /// <param name="vector">
        /// The translation vector.
        /// </param>
        /// <returns>
        /// The <see cref="DualQuaternion"/>.
        /// </returns>
        public static DualQuaternion CreateTranslation(Vector3 vector)
        {
            return CreateTranslation(1, vector);
        }

        /// <summary>
        /// Creates a pure translation dual quaternion (1 + ε½t * amount)
        /// </summary>
        /// <param name="amount">
        /// The translation amount
        /// </param>
        /// <param name="vector">
        /// The translation vector.
        /// </param>
        /// <returns>
        /// The <see cref="DualQuaternion"/>.
        /// </returns>
        public static DualQuaternion CreateTranslation(double amount, Vector3 vector)
        {
            var result = new DualQuaternion(
                0,
                0,
                0,
                1.0f,
                (double)(amount * vector.X / 2d),
                (double)(amount * vector.Y / 2d),
                (double)(amount * vector.Z / 2d),
                0);

            return result;
        }

        /// <summary>
        /// Creates a dual quaternion representing a point.
        /// </summary>
        /// <param name="point">
        /// The position of the point.
        /// </param>
        /// <returns>
        /// The <see cref="DualQuaternion"/>.
        /// </returns>
        public static DualQuaternion CreatePoint(Vector3 point)
        {
            var result = new DualQuaternion(
                0,
                0,
                0,
                1.0f,
                point.X,
                point.Y,
                point.Z,
                0);

            return result;
        }

        /// <summary>
        /// Creates a dual quaternion representing a line.
        /// </summary>
        /// <param name="vector">
        /// Direction vector of the line.
        /// </param>
        /// <param name="point">
        /// A point of the line. 
        /// </param>
        /// <returns>
        /// The <see cref="DualQuaternion"/>.
        /// </returns>
        public static DualQuaternion CreateLine(Vector3 vector, Vector3 point)
        {
            // We do cross product with the line point and line vector to get the Plücker coordinates.
            var s0 = Vector3.Cross(point, vector);
            var result = CreateLinePlucker(vector, s0);

            return result;
        }

        /// <summary>
        /// Creates a dual quaternion representing a line from Plücker coordinates.
        /// </summary>
        /// <param name="vector">
        /// Direction vector of the line.
        /// </param>
        /// <param name="moment">
        /// The momento f the line.
        /// </param>
        /// <returns>
        /// The <see cref="DualQuaternion"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Suppression is OK here.")]
        public static DualQuaternion CreateLinePlucker(Vector3 vector, Vector3 moment)
        {
            var result = new DualQuaternion(
                vector.X,
                vector.Y,
                vector.Z,
                0,
                moment.X,
                moment.Y,
                moment.Z,
                0);

            return result;
        }

        /// <summary>
        /// Creates a unit dual quaternion representing a plane.
        /// </summary>
        /// <param name="normal">
        /// Normal of the plane.
        /// </param>
        /// <param name="distance">
        /// Distance from the origin to the plane. 
        /// </param>
        /// <returns>
        /// The <see cref="DualQuaternion"/>.
        /// </returns>
        public static DualQuaternion CreatePlane(Vector3 normal, double distance)
        {
            var result = new DualQuaternion(
                normal.X,
                normal.Y,
                normal.Z,
                0,
                0,
                0,
                0,
                distance);

            return result;
        }

        /// <summary>
        /// Creates a pure rotation dual quaternion from a rotation matrix.
        /// </summary>
        /// <param name="rotation">
        /// The 3x3 Rotation matrix.
        /// </param>
        /// <returns>
        /// The dual quaternion.
        /// </returns>
        public static DualQuaternion CreateRotationMatrix(DenseMatrix rotation)
        {
            // B = (rotation - I).(R + I)^{-1}
            var rminus = rotation - DenseMatrix.CreateIdentity(3);
            var rplus = rotation + DenseMatrix.CreateIdentity(3);
            rplus.Inverse();
            var b = rminus * rplus;

            /*
             *      0 -b_z  b_y
             * B = b_z  0  -b_x
             *    -b_y b_x   0
             *
             * b = { b_x b_y b_z }
             *
             * s           = b / ||b||
             * tan(theta/2) = ||b||
             */

            var s = new Vector3((float)b[3, 2], (float)b[1, 3], (float)b[2, 1]);
            var tz = s.Length();

            // Avoid normalizing 0. vectors.
            if (tz > 0)
            {
                s.X = s.X / tz;
                s.Y = s.Y / tz;
                s.Z = s.Z / tz;
            }

            var z2 = Math.Atan(tz);

            // Build the rotational part.
            var sz = Math.Sin(z2);
            var cz = Math.Cos(z2);

            var result = new DualQuaternion(
            (double)(sz * s.X),
            (double)(sz * s.Y),
            (double)(sz * s.Z),
            (double)cz,
            0,
            0,
            0,
            0);

            return result;
        }

        /// <summary>
        /// Creates a rotation then translation dual quaternion (r + ε½tr).
        /// </summary>
        /// <param name="rotation">
        /// The rotation.
        /// </param>
        /// <param name="translation">
        /// The translation.
        /// </param>
        /// <returns>
        /// The <see cref="DualQuaternion"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Suppression is OK here.")]
        public static DualQuaternion CreateRotationThenTranslation(Quaternion rotation, Vector3 translation)
        {
            var factor = Quaternion.Normalize(rotation) * 0.5f;
            var dual = new Quaternion(translation, 0) * factor;

            var result = new DualQuaternion(factor, dual);

            return result;
        }

        /// <summary>
        /// Calculates the dot product of two dual quaternions.
        /// </summary>
        /// <param name="left">
        /// The left dual quaternion.
        /// </param>
        /// <param name="right">
        /// The right dual quaternion.
        /// </param>
        /// <returns>
        /// The result of the dot product.
        /// </returns>
        public static double Dot(DualQuaternion left, DualQuaternion right)
        {
            var result = Quaternion.Dot(left.Real, right.Real);

            return result;
        }

        /// <summary>
        /// Converts the dual quaternion to a unit dual quaternion.
        /// </summary>
        /// <param name="value">
        /// The dual quaternion to normalize.
        /// </param>
        /// <returns>
        /// The normalized dual quaternion.
        /// </returns>
        public static DualQuaternion Normalize(DualQuaternion value)
        {
            var scale = (double)Adjust(1 / value.Length);

            var result = new DualQuaternion(value.Real * scale, value.Dual * scale);

            return result;
        }

        /// <summary>
        /// Conjugates a dual quaternion.
        /// </summary>
        /// <param name="value">
        /// The dual quaternion to conjugate.
        /// </param>
        /// <returns>
        /// The conjugated dual quaternion.
        /// </returns>
        public static DualQuaternion Conjugate(DualQuaternion value)
        {
            var result = new DualQuaternion(
                Quaternion.Conjugate(value.Real),
                Quaternion.Conjugate(value.Dual));

            return result;
        }

        /// <summary>
        /// Clifford conjugation transformation of type F1G (Alba Perez notation).
        /// </summary>
        /// <remarks>
        /// f1G : C(V, &lt;, &gt;) −→ C(V, &lt;, &gt;)
        /// A : B −→ ABA
        /// </remarks>
        /// <param name="transformation">
        /// The dual quaternion representing the transformation.
        /// </param>
        /// <param name="value">
        /// The dual quaternion being transformed.
        /// </param>
        /// <returns>
        /// The resulting dual quaternion.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1631:DocumentationMustMeetCharacterPercentage", Justification = "Suppression is OK here.")]
        public static DualQuaternion CliffordConjugationTransformationF1G(DualQuaternion transformation, DualQuaternion value)
        {
            var result = (transformation * value) * transformation;

            return result;
        }

        /// <summary>
        /// Clifford conjugation transformation of type F2G (Alba Perez notation) - This transformation is useful for lines.
        /// </summary>
        /// <remarks>
        /// A : B −→ ABA∗
        /// f2G : C(V, &lt;, &gt;) −→ C(V, &lt;, &gt;)
        /// </remarks>
        /// <param name="value">
        ///     Dual quaternion being transformed.
        /// </param>
        /// <param name="transformation">
        ///     Dual quaternion representing the transformation.
        /// </param>
        /// <returns>
        /// The resulting dual quaternion.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1631:DocumentationMustMeetCharacterPercentage", Justification = "Suppression is OK here.")]
        public static DualQuaternion CliffordConjugationTransformationF2G(DualQuaternion value, DualQuaternion transformation)
        {
            var transformationStar = Conjugate(transformation);

            var result = (transformation * value) * transformationStar;

            return result;
        }

        /// <summary>
        /// Clifford conjugation transformation of type F3G (Alba Perez notation).
        /// </summary>
        /// <remarks>
        /// f3G : C(V, &lt;, &gt;) −→ C(V, &lt;, &gt;)
        /// A : B −→ AB(a0 + a − E(a^0 + a7))
        /// </remarks>
        /// <param name="value">
        ///     Dual quaternion being transformed. 
        /// </param>
        /// <param name="transformation">
        ///     Dual quaternion representing the transformation.
        /// </param>
        /// <returns>
        /// The resulting dual quaternion.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1603:DocumentationMustContainValidXml", Justification = "Suppression is OK here.")]
        public static DualQuaternion CliffordConjugationTransformationF3G(DualQuaternion value, DualQuaternion transformation)
        {
            var tr = transformation.Real;
            var td = transformation.Dual;

            var transformationStar = new DualQuaternion(tr.X, tr.Y, tr.Z, tr.W, -td.X, -td.Y, -td.Z, -td.W);

            var result = (transformation * value) * transformationStar;

            return result;
        }

        /// <summary>
        /// Applies a clifford conjugation transformation of type F4G (Alba Perez notation) - This transformation is useful for points.
        /// </summary>
        /// <param name="value">
        /// Dual quaternion being transformed. 
        /// </param>
        /// <param name="transformation">
        /// Dual quaternion representing the transformation.
        /// </param>
        /// <param name="adjust">
        /// Set to true for adjusting the resulting dual quaternion (Defaulted to true).
        /// </param>
        /// <returns>
        /// The resulting dual quaternion.
        /// </returns>
        public static DualQuaternion CliffordConjugationTransformationF4G(DualQuaternion value, DualQuaternion transformation, bool adjust)
        {
            var t = transformation.Real;
            var t0 = transformation.Dual;

            var transformationStar = new DualQuaternion(-t.X, -t.Y, -t.Z, t.W, t0.X, t0.Y, t0.Z, -t0.W);

            var result = transformation * value;
            result = result * transformationStar;

            if (adjust)
            {
                result.Adjust();
            }

            return result;
        }

        /// <summary>
        /// Multiplies a collection of dual quaternion transformations.
        /// </summary>
        /// <param name="values">
        /// The collection of dual quaternions.
        /// </param>
        /// <returns>
        /// The product of all dual quaternions.
        /// </returns>
        // ReSharper disable InconsistentNaming
        public static DualQuaternion Multiply(params DualQuaternion[] values)
        {
            if (!values.Any())
            {
                throw new ArgumentException("Must define one or more transformations");
            }

            var q = values[values.Length - 1];

            double x = q[0];
            double y = q[1];
            double z = q[2];
            double w = q[3];

            double x0 = q[4];
            double y0 = q[5];
            double z0 = q[6];
            double w0 = q[7];

            for (var i = values.Length - 2; i >= 0; i--)
            {
                Combine(
                    values[i],
                    ref x,
                    ref y,
                    ref z,
                    ref w,
                    ref x0,
                    ref y0,
                    ref z0,
                    ref w0);
            }

            var result = new DualQuaternion((double)x, (double)y, (double)z, (double)w, (double)x0, (double)y0, (double)z0, (double)w0);

            return result;
            // ReSharper restore InconsistentNaming
        }

        /// <summary>
        /// Applies a Clifford conjugation transformation of type F4G, then convert the result to a point.
        /// </summary>
        /// <param name="value">
        /// The point being transformed.
        /// </param>
        /// <param name="transformation">
        /// The dual quaternion representing the transformation.
        /// </param>
        /// <param name="round">
        /// If true, applies rounding to the transformed point coordinates.
        /// </param>
        /// <returns>
        /// The transformed point.
        /// </returns>
        public static Vector3 TransformPoint(Vector3 value, DualQuaternion transformation, bool round)
        {
            var initialPoint = CreatePoint(value);
            var q = CliffordConjugationTransformationF4G(initialPoint, transformation, round);

            var result = q.GetPoint(round);

            return result;
        }

        /// <summary>
        /// Checks to see if a point Q is on the plane P.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="plane">The plane.</param>
        /// <returns>
        /// True if the point is on the plane.
        /// </returns>
        public static bool IsPointOnPlane(DualQuaternion point, DualQuaternion plane)
        {
            var tolerance = .000001;
            var result = Math.Abs(
                (plane.Real.X * point.Dual.X) +
                (plane.Real.Y * point.Dual.Y) +
                (plane.Real.Z * point.Dual.Z) - plane.Dual.Z)
                < tolerance;

            return result;
        }

        /// <summary>
        /// Compares two dual quaternions.
        /// </summary>
        /// <param name="left">
        /// The first dual quaternion.
        /// </param>
        /// <param name="right">
        /// The second dual quaternion.
        /// </param>
        /// <param name="precision">
        /// The comparison precision.
        /// </param>
        /// <returns>
        /// 0 if both dual quaternions are equal.
        /// </returns>
        public static int Compare(DualQuaternion left, DualQuaternion right, double precision = tolerance)
        {
            int i, ret1, ret2;

            // To compensate the rotational ambiguity we see if the 'z' component is the same.
            ret1 = 0;
            for (i = 0; i < 8; i++)
            {
                if (Math.Abs(left[i] - right[i]) > precision)
                {
                    ret1++;
                }
            }

            ret2 = 0;
            for (i = 0; i < 8; i++)
            {
                if (Math.Abs(left[i] + right[i]) > precision)
                {
                    ret2++;
                }
            }

            var result = Math.Min(ret1, ret2);

            return result;
        }

        /// <summary>
        /// Conjugates a dual quaternion.
        /// </summary>
        public void Conjugate()
        {
            this.Real = Quaternion.Conjugate(this.Real);
            this.Dual = Quaternion.Conjugate(this.Dual);
        }

        /// <summary>
        /// Inverses a dual quaternion
        /// </summary>
        public DualQuaternion Inverse()
        {
            var result = new DualQuaternion();
            double realLengthSquared, dualLengthSquared;

            // Get the length squared
            this.GetLengthSquared(out realLengthSquared, out dualLengthSquared);

            // Set the values
            result.Real = new Quaternion(
                (double)Adjust(-this.Real.X * realLengthSquared),
                (double)Adjust(-this.Real.Y * realLengthSquared),
                (double)Adjust(-this.Real.Z * realLengthSquared),
                (double)Adjust(this.Real.W * realLengthSquared));

            var a = dualLengthSquared - realLengthSquared;
            var b = -a;

            result.Dual = new Quaternion(
                (double)Adjust(this.Dual.X * a),
                (double)Adjust(this.Dual.Y * a),
                (double)Adjust(this.Dual.Y * a),
                (double)Adjust(this.Dual.Y * b));
            return result;
        }

        /// <summary>
        /// Gets the length squared for a dual quaternion.
        /// </summary>
        /// <param name="realLengthSquared">
        /// The real part of the length squared.
        /// </param>
        /// <param name="dualLengthSquared">
        /// The dual part of the length squared.
        /// </param>
        public void GetLengthSquared(out double realLengthSquared, out double dualLengthSquared)
        {
            realLengthSquared = this.Real.LengthSquared();
            dualLengthSquared = this.Dual.LengthSquared();
        }

        /// <summary>
        /// Converts a dual quaternion to a 4x4 homogeneous matrix.
        /// </summary>
        /// <returns>
        /// The 4x4 homogeneous matrix.
        /// </returns>
        public DenseMatrix ToMatrix()
        {
            var q = Normalize(this);

            var w = q.Real.W;
            var x = q.Real.X;
            var y = q.Real.Y;
            var z = q.Real.Z;

            // Extract rotational information
            var result = DenseMatrix.CreateIdentity(4);

            result[1, 1] = (w * w) + (x * x) - (y * y) - (z * z);
            result[1, 2] = (2 * x * y) + (2 * w * z);
            result[1, 3] = (2 * x * z) - (2 * w * y);

            result[2, 1] = (2 * x * y) - (2 * w * z);
            result[2, 2] = (w * w) + (y * y) - (x * x) - (z * z);
            result[2, 3] = (2 * y * z) + (2 * w * x);

            result[3, 1] = (2 * x * z) + (2 * w * y);
            result[3, 2] = (2 * y * z) - (2 * w * x);
            result[3, 3] = (w * w) + (z * z) - (x * x) - (y * y);

            // Extract translation information
            var t = (q.Dual * 2.0f) * Quaternion.Conjugate(q.Real);

            result[4, 1] = t.X;
            result[4, 2] = t.Y;
            result[4, 3] = t.Z;

            return result;
        }

        /// <summary>
        /// Retrieves a point from a dual quaternion.
        /// </summary>
        /// <param name="round">
        /// If true, rounding is applied (Defaulted to false).
        /// </param>
        /// <returns>
        /// The point.
        /// </returns>
        public Vector3 GetPoint(bool round = false)
        {
            var result = new Vector3(this.Dual.X, this.Dual.Y, this.Dual.Z);

            if (round)
            {
                result.X = Round(result.X);
                result.Y = Round(result.Y);
                result.Z = Round(result.Z);
            }

            return result;
        }

        /// <summary>
        /// Creates an array containing the elements of the dual quaternion.
        /// </summary>
        /// <returns>
        /// An eight-element array containing the components of the dual quaternion.
        /// </returns>
        public double[] ToArray()
        {
            return new[]
            {
              this.Real.X,
              this.Real.Y,
              this.Real.Z,
              this.Real.W,
              this.Dual.X,
              this.Dual.Y,
              this.Dual.Z,
              this.Dual.W
            };
        }

        /// <summary>
        /// Determines whether the specified <see cref="DualQuaternion"/> is equal to this instance.
        /// </summary>
        /// <param name="other">
        /// The <see cref="DualQuaternion"/> to compare with this instance.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="DualQuaternion"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(ref DualQuaternion other)
        {
            var result = this.Real.Equals(other.Real) && this.Dual.Equals(other.Dual);

            return result;
        }

        /// <summary>
        /// Determines whether the specified <see cref="DualQuaternion"/> is equal to this instance.
        /// </summary>
        /// <param name="other">
        /// The <see cref="DualQuaternion"/> to compare with this instance.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="DualQuaternion"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(DualQuaternion other)
        {
            return this.Equals(ref other);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="T:System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object value)
        {
            if (!(value is DualQuaternion))
            {
                return false;
            }

            var other = (DualQuaternion)value;

            return this.Equals(ref other);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                FormatTemplate,
                this.Real,
                this.Dual);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents this instance.
        /// </returns>
        public string ToString(string format)
        {
            if (format == null)
            {
                return this.ToString();
            }

            return string.Format(
                CultureInfo.CurrentCulture,
                FormatTemplate,
                this.Real.ToString(),
                this.Dual.ToString());
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyFieldInGetHashCode
            return (this.Real.GetHashCode() * 397) ^ this.Dual.GetHashCode();
            // ReSharper restore NonReadonlyFieldInGetHashCode
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents this instance.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents this instance.
        /// </returns>
        public string ToString(IFormatProvider formatProvider)
        {
            return string.Format(
                formatProvider,
                FormatTemplate,
                this.Real,
                this.Dual);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param><param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents this instance.
        /// </returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
            {
                return this.ToString(formatProvider);
            }

            return string.Format(
                formatProvider,
                FormatTemplate,
                this.Real.ToString(),
                this.Dual.ToString());
        }

        /// <summary>
        /// Adjusts a dual quaternion by setting its component to zero if less than <see cref="MathUtil.ZeroTolerance"/>.
        /// </summary>
        public void Adjust()
        {
            for (var i = 0; i < 8; i++)
            {
                this[i] = (double)Adjust(this[i]);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Adjusts the given double by setting to zero if less than <see cref="MathUtil.ZeroTolerance"/>.
        /// </summary>
        /// <param name="value">
        /// The double value to adjust.
        /// </param>
        /// <returns>
        /// The adjusted double value.
        /// </returns>
        private static double Adjust(double value)
        {
            var result = value;

            if (Math.Abs(result) < tolerance)
            {
                result = 0;
            }

            return (double)result;
        }

        /// <summary>
        /// Rounds a value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        private static double Round(double value)
        {
            var result = Math.Round(value, RoundingDigits);

            return (double)result;
        }

        /// <summary>
        /// Multiplies a quaternion by another.
        /// </summary>
        /// <param name="px">
        /// The x component of the left quaternion.
        /// </param>
        /// <param name="py">
        /// The y component of the left quaternion.
        /// </param>
        /// <param name="pz">
        /// The z component of the left quaternion.
        /// </param>
        /// <param name="pw">
        /// The w component of the left quaternion.
        /// </param>
        /// <param name="qx">
        /// The x component of the right quaternion.
        /// </param>
        /// <param name="qy">
        /// The y component of the right quaternion.
        /// </param>
        /// <param name="qz">
        /// The z component of the right quaternion.
        /// </param>
        /// <param name="qw">
        /// The w component of the right quaternion.
        /// </param>
        /// <param name="x">
        /// The x component of the product.
        /// </param>
        /// <param name="y">
        /// The y component of the product.
        /// </param>
        /// <param name="z">
        /// The z component of the product.
        /// </param>
        /// <param name="w">
        /// The w component of the product.
        /// </param>
        private static void Multiply(
            ref double px,
            ref double py,
            ref double pz,
            ref double pw,
            ref double qx,
            ref double qy,
            ref double qz,
            ref double qw,
            out double x,
            out double y,
            out double z,
            out double w)
        {
            var a = (py * qz) - (pz * qy);
            var b = (pz * qx) - (px * qz);
            var c = (px * qy) - (py * qx);
            var d = (px * qx) + (py * qy) + (pz * qz);

            x = (px * qw) + (qx * pw) + a;
            y = (py * qw) + (qy * pw) + b;
            z = (pz * qw) + (qz * pw) + c;
            w = (pw * qw) - d;
        }

        /// <summary>
        /// Combines a dual quaternion transformation to another.
        /// </summary>
        /// <param name="transformation">
        /// The value.
        /// </param>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <param name="z">
        /// The z.
        /// </param>
        /// <param name="w">
        /// The w.
        /// </param>
        /// <param name="x0">
        /// The x 0.
        /// </param>
        /// <param name="y0">
        /// The y 0.
        /// </param>
        /// <param name="z0">
        /// The z 0.
        /// </param>
        /// <param name="w0">
        /// The w 0.
        /// </param>
        private static void Combine(
            DualQuaternion transformation,
            ref double x,
            ref double y,
            ref double z,
            ref double w,
            ref double x0,
            ref double y0,
            ref double z0,
            ref double w0)
        {
            // ReSharper disable InconsistentNaming

            /*  We can decompose the problem into quaternion multiplication:            
                Q = q + εq0
                P = p + εp0
                Q*P = q*p + ε(q*p0 + q0*p) 
            */

            var qx = (double)transformation.Real.X;
            var qy = (double)transformation.Real.Y;
            var qz = (double)transformation.Real.Z;
            var qw = (double)transformation.Real.W;

            var q0x = (double)transformation.Dual.X;
            var q0y = (double)transformation.Dual.Y;
            var q0z = (double)transformation.Dual.Z;
            var q0w = (double)transformation.Dual.W;

            var px = x;
            var py = y;
            var pz = z;
            var pw = w;

            var p0x = x0;
            var p0y = y0;
            var p0z = z0;
            var p0w = w0;

            // q*p
            Multiply(ref px, ref qy, ref qz, ref qw, ref px, ref py, ref pz, ref pw, out x, out y, out z, out w);

            // a = ε(q*p0)
            double ax0;
            double ay0;
            double az0;
            double aw0;

            Multiply(ref qx, ref qy, ref qz, ref qw, ref p0x, ref p0y, ref p0z, ref p0w, out ax0, out ay0, out az0, out aw0);

            // b = ε(q0*p)
            double bx0;
            double by0;
            double bz0;
            double bw0;

            Multiply(ref q0x, ref q0y, ref q0z, ref q0w, ref px, ref py, ref pz, ref pw, out bx0, out by0, out bz0, out bw0);

            // a + b
            x0 = ax0 + bx0;
            y0 = ay0 + by0;
            z0 = az0 + bz0;
            w0 = aw0 + bw0;
        }

        #endregion
    }
}
