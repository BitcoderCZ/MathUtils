using MathUtils.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace MathUtils.Vectors
{
    public struct Vector3US : IEnumerable<ushort>
    {
        // vec 1
        public ushort X;
        public ushort Y;
        public ushort Z;

        // vec2
        public Vector2US XX => new Vector2US(X, X);
        public Vector2US XY => new Vector2US(X, Y);
        public Vector2US XZ => new Vector2US(X, Z);
        public Vector2US YX => new Vector2US(Y, X);
        public Vector2US YY => new Vector2US(Y, Y);
        public Vector2US YZ => new Vector2US(Y, Z);
        public Vector2US ZX => new Vector2US(Z, X);
        public Vector2US ZY => new Vector2US(Z, Y);
        public Vector2US ZZ => new Vector2US(Z, Z);

        // vec 3
        public Vector3US XXX => new Vector3US(X, X, X);
        public Vector3US XXY => new Vector3US(X, X, Y);
        public Vector3US XXZ => new Vector3US(X, X, Z);
        public Vector3US XYX => new Vector3US(X, Y, X);
        public Vector3US XYY => new Vector3US(X, Y, Y);
        public Vector3US XYZ => new Vector3US(X, Y, Z);
        public Vector3US XZX => new Vector3US(X, Z, X);
        public Vector3US XZY => new Vector3US(X, Z, Y);
        public Vector3US XZZ => new Vector3US(X, Z, Z);
        public Vector3US YXX => new Vector3US(Y, X, X);
        public Vector3US YXY => new Vector3US(Y, X, Y);
        public Vector3US YXZ => new Vector3US(Y, X, Z);
        public Vector3US YYX => new Vector3US(Y, Y, X);
        public Vector3US YYY => new Vector3US(Y, Y, Y);
        public Vector3US YYZ => new Vector3US(Y, Y, Z);
        public Vector3US YZX => new Vector3US(Y, Z, X);
        public Vector3US YZY => new Vector3US(Y, Z, Y);
        public Vector3US YZZ => new Vector3US(Y, Z, Z);
        public Vector3US ZXX => new Vector3US(Z, X, X);
        public Vector3US ZXY => new Vector3US(Z, X, Y);
        public Vector3US ZXZ => new Vector3US(Z, X, Z);
        public Vector3US ZYX => new Vector3US(Z, Y, X);
        public Vector3US ZYY => new Vector3US(Z, Y, Y);
        public Vector3US ZYZ => new Vector3US(Z, Y, Z);
        public Vector3US ZZX => new Vector3US(Z, Z, X);
        public Vector3US ZZY => new Vector3US(Z, Z, Y);
        public Vector3US ZZZ => new Vector3US(Z, Z, Z);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public ushort this[int index]
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

        public static readonly Vector3US Zero = new Vector3US(0, 0, 0);
        public static readonly Vector3US One = new Vector3US(1, 1, 1);

        public static readonly Vector3US UnitX = new Vector3US(1, 0, 0);
        public static readonly Vector3US UnitY = new Vector3US(0, 1, 0);
        public static readonly Vector3US UnitZ = new Vector3US(0, 0, 1);

        public Vector3US(int _x, int _y, int _z)
            : this((ushort)_x, (ushort)_y, (ushort)_z)
        {
        }
        public Vector3US(ushort _x, ushort _y, ushort _z)
        {
            X = _x;
            Y = _y;
            Z = _z;
        }

        public IEnumerator<ushort> GetEnumerator()
            => new ArrayEnumerator<ushort>(X, Y, Z);

        IEnumerator IEnumerable.GetEnumerator()
            => new ArrayEnumerator<ushort>(X, Y, Z);

        public static Vector3US Min(Vector3US a, Vector3US b)
            => new Vector3US(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
        public static Vector3US Max(Vector3US a, Vector3US b)
            => new Vector3US(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));

        public static double Distance(Vector3US a, Vector3US b)
            => (a - b).Length;

        public static int Dot(Vector3US a, Vector3US b)
            => a.X * b.X + a.Y * b.Y + a.Z;
        public static Vector3US Cross(Vector3US a, Vector3US b)
            => new Vector3US(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X
            );

        public static Vector3US operator +(Vector3US a, Vector3US b)
            => new Vector3US(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector3US operator -(Vector3US a, Vector3US b)
            => new Vector3US(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vector3US operator *(Vector3US a, Vector3US b)
            => new Vector3US(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        public static Vector3US operator /(Vector3US a, Vector3US b)
            => new Vector3US(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        public static Vector3US operator %(Vector3US a, Vector3US b)
            => new Vector3US(a.X % b.X, a.Y % b.Y, a.Z % b.Z);

        public static Vector3US operator *(Vector3US a, int b)
            => new Vector3US(a.X * b, a.Y * b, a.Z * b);
        public static Vector3US operator /(Vector3US a, int b)
            => new Vector3US(a.X / b, a.Y / b, a.Z / b);
        public static Vector3US operator %(Vector3US a, int b)
            => new Vector3US(a.X % b, a.Y % b, a.Z % b);

        public static bool operator ==(Vector3US a, Vector3US b)
            => a.Equals(b);
        public static bool operator !=(Vector3US a, Vector3US b)
            => !a.Equals(b);

        public static explicit operator Vector3US(Vector3F v)
            => new Vector3US((ushort)v.X, (ushort)v.Y, (ushort)v.Z);
        public static explicit operator Vector3US(Vector3I v)
            => new Vector3US((ushort)v.X, (ushort)v.Y, (ushort)v.Z);
        public static explicit operator Vector3US(Vector3S v)
            => new Vector3US((ushort)v.X, (ushort)v.Y, (ushort)v.Z);
        public static implicit operator Vector3US(Vector3B v)
            => new Vector3US(v.X, v.Y, v.Z);

        public override int GetHashCode()
            => HashCode.Combine(X, Y, Z);

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is Vector3US other) return Equals(other);
            else return false;
        }

        public bool Equals(Vector3US other)
            => X == other.X && Y == other.Y && Z == other.Z;

        public override string ToString()
            => ToString("G", CultureInfo.CurrentCulture);

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
