using MathUtils.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace MathUtils.Vectors
{
    public struct Vector3B : IEnumerable<byte>
    {
        // vec 1
        public byte X;
        public byte Y;
        public byte Z;

        // vec2
        public Vector2B XX => new Vector2B(X, X);
        public Vector2B XY => new Vector2B(X, Y);
        public Vector2B XZ => new Vector2B(X, Z);
        public Vector2B YX => new Vector2B(Y, X);
        public Vector2B YY => new Vector2B(Y, Y);
        public Vector2B YZ => new Vector2B(Y, Z);
        public Vector2B ZX => new Vector2B(Z, X);
        public Vector2B ZY => new Vector2B(Z, Y);
        public Vector2B ZZ => new Vector2B(Z, Z);

        // vec 3
        public Vector3B XXX => new Vector3B(X, X, X);
        public Vector3B XXY => new Vector3B(X, X, Y);
        public Vector3B XXZ => new Vector3B(X, X, Z);
        public Vector3B XYX => new Vector3B(X, Y, X);
        public Vector3B XYY => new Vector3B(X, Y, Y);
        public Vector3B XYZ => new Vector3B(X, Y, Z);
        public Vector3B XZX => new Vector3B(X, Z, X);
        public Vector3B XZY => new Vector3B(X, Z, Y);
        public Vector3B XZZ => new Vector3B(X, Z, Z);
        public Vector3B YXX => new Vector3B(Y, X, X);
        public Vector3B YXY => new Vector3B(Y, X, Y);
        public Vector3B YXZ => new Vector3B(Y, X, Z);
        public Vector3B YYX => new Vector3B(Y, Y, X);
        public Vector3B YYY => new Vector3B(Y, Y, Y);
        public Vector3B YYZ => new Vector3B(Y, Y, Z);
        public Vector3B YZX => new Vector3B(Y, Z, X);
        public Vector3B YZY => new Vector3B(Y, Z, Y);
        public Vector3B YZZ => new Vector3B(Y, Z, Z);
        public Vector3B ZXX => new Vector3B(Z, X, X);
        public Vector3B ZXY => new Vector3B(Z, X, Y);
        public Vector3B ZXZ => new Vector3B(Z, X, Z);
        public Vector3B ZYX => new Vector3B(Z, Y, X);
        public Vector3B ZYY => new Vector3B(Z, Y, Y);
        public Vector3B ZYZ => new Vector3B(Z, Y, Z);
        public Vector3B ZZX => new Vector3B(Z, Z, X);
        public Vector3B ZZY => new Vector3B(Z, Z, Y);
        public Vector3B ZZZ => new Vector3B(Z, Z, Z);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public byte this[int index]
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
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        public double LengthSquared => X * X + Y * Y + Z * Z;
        public double Length => Math.Sqrt(LengthSquared);

        public static readonly Vector3B Zero = new Vector3B(0, 0, 0);
        public static readonly Vector3B One = new Vector3B(1, 1, 1);

        public static readonly Vector3B UnitX = new Vector3B(1, 0, 0);
        public static readonly Vector3B UnitY = new Vector3B(0, 1, 0);
        public static readonly Vector3B UnitZ = new Vector3B(0, 0, 1);

        public Vector3B(int _x, int _y, int _z)
            : this((byte)_x, (byte)_y, (byte)_z)
        {
        }
        public Vector3B(byte _x, byte _y, byte _z)
        {
            X = _x;
            Y = _y;
            Z = _z;
        }

        public IEnumerator<byte> GetEnumerator()
            => new ArrayEnumerator<byte>(X, Y, Z);

        IEnumerator IEnumerable.GetEnumerator()
            => new ArrayEnumerator<byte>(X, Y, Z);

        public static Vector3B Min(Vector3B a, Vector3B b)
            => new Vector3B(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
        public static Vector3B Max(Vector3B a, Vector3B b)
            => new Vector3B(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));

        public static double Distance(Vector3B a, Vector3B b)
            => (a - b).Length;

        public static int Dot(Vector3B a, Vector3B b)
            => a.X * b.X + a.Y * b.Y + a.Z;
        public static Vector3B Cross(Vector3B a, Vector3B b)
            => new Vector3B(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X
            );

        public static Vector3B operator +(Vector3B a, Vector3B b)
            => new Vector3B(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector3B operator -(Vector3B a, Vector3B b)
            => new Vector3B(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vector3B operator *(Vector3B a, Vector3B b)
            => new Vector3B(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        public static Vector3B operator /(Vector3B a, Vector3B b)
            => new Vector3B(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        public static Vector3B operator %(Vector3B a, Vector3B b)
            => new Vector3B(a.X % b.X, a.Y % b.Y, a.Z % b.Z);

        public static Vector3B operator *(Vector3B a, int b)
            => new Vector3B(a.X * b, a.Y * b, a.Z * b);
        public static Vector3B operator /(Vector3B a, int b)
            => new Vector3B(a.X / b, a.Y / b, a.Z / b);
        public static Vector3B operator %(Vector3B a, int b)
            => new Vector3B(a.X % b, a.Y % b, a.Z % b);

        public static bool operator ==(Vector3B a, Vector3B b)
            => a.Equals(b);
        public static bool operator !=(Vector3B a, Vector3B b)
            => !a.Equals(b);

        public static explicit operator Vector3B(Vector3F v)
            => new Vector3B((byte)v.X, (byte)v.Y, (byte)v.Z);
        public static explicit operator Vector3B(Vector3I v)
            => new Vector3B((byte)v.X, (byte)v.Y, (byte)v.Z);
        public static explicit operator Vector3B(Vector3S v)
            => new Vector3B((byte)v.X, (byte)v.Y, (byte)v.Z);
        public static explicit operator Vector3B(Vector3US v)
            => new Vector3B((byte)v.X, (byte)v.Y, (byte)v.Z);

        public override int GetHashCode()
            => HashCode.Combine(X, Y, Z);

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is Vector3B other) return Equals(other);
            else return false;
        }

        public bool Equals(Vector3B other)
            => X == other.X && Y == other.Y && Z == other.Z;

        public override string ToString()
            => ToString("G", CultureInfo.InvariantCulture);

        public string ToString(string format)
            => ToString(format, CultureInfo.InvariantCulture);

        public string ToString(string format, IFormatProvider formatProvider)
        {
            StringBuilder sb = new StringBuilder();
            string separator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;
            sb.Append('<');
            sb.Append(X.ToString(format, formatProvider));
            sb.Append(separator);
            sb.Append(' ');
            sb.Append(Y.ToString(format, formatProvider));
            sb.Append(separator);
            sb.Append(' ');
            sb.Append(Z.ToString(format, formatProvider));
            sb.Append('>');
            return sb.ToString();
        }
    }
}
