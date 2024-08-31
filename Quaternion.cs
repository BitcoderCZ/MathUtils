using MathUtils.Utils;
using MathUtils.Vectors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace MathUtils
{
    public struct Quaternion : IEnumerable<double>
    {
        public double X;
        public double Y;
        public double Z;
        public double W;

        public static readonly Quaternion Identity = new Quaternion(0f, 0f, 0f, 1f);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    case 2:
                        return Z;
                    case 3:
                        return W;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    case 2:
                        Z = value;
                        break;
                    case 3:
                        W = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        public double LengthSquared => X * X + Y * Y + Z * Z + W * W;
        public double Length => Math.Sqrt(LengthSquared);

        public Vector3F EulerAngles
        {
            get
            {
                Matrix4 m = Matrix4.Rotation(this);

                double x;
                double y;
                double z;

                y = Math.Asin(Math.Clamp(m[8], -1f, 1f));

                if (Math.Abs(m[8]) < 0.9999999f)
                {
                    x = Math.Atan2(-m[9], m[10]);
                    z = Math.Atan2(-m[4], m[0]);
                }
                else
                {
                    x = Math.Atan2(m[6], m[5]);
                    z = 0f;
                }

                return new Vector3F((float)x, (float)y, (float)z);
            }
        }

        public Quaternion(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public static Quaternion Euler(Vector3F v)
            => Euler(v.X, v.Y, v.Z);
        public static Quaternion Euler(double x, double y, double z)
        {
            double c1 = Math.Cos(x / 2);
            double c2 = Math.Cos(y / 2);
            double c3 = Math.Cos(z / 2);

            double s1 = Math.Sin(x / 2);
            double s2 = Math.Sin(y / 2);
            double s3 = Math.Sin(z / 2);

            return new Quaternion(
                s1 * c2 * c3 + c1 * s2 * s3,
                c1 * s2 * c3 - s1 * c2 * s3,
                c1 * c2 * s3 + s1 * s2 * c3,
                c1 * c2 * c3 - s1 * s2 * s3
            );
        }

        public Quaternion Inversed()
        {
            double len = Length;
            double invLen = 1f / (len * len);

            return new Quaternion(
                -X * invLen,
                -Y * invLen,
                -Z * invLen,
                W * invLen
            );
        }

        public IEnumerator<double> GetEnumerator()
           => new ArrayEnumerator<double>(X, Y, Z, W);

        IEnumerator IEnumerable.GetEnumerator()
            => new ArrayEnumerator<double>(X, Y, Z, W);

        public static bool operator ==(Quaternion a, Quaternion b)
           => a.Equals(b);
        public static bool operator !=(Quaternion a, Quaternion b)
            => !a.Equals(b);

        public override int GetHashCode()
            => HashCode.Combine(X, Y);

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is Quaternion other) return Equals(other);
            else return false;
        }

        public bool Equals(Quaternion other)
            => X == other.X && Y == other.Y && Z == other.Z && W == other.W;
    }
}
