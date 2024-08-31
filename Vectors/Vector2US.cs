using MathUtils.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace MathUtils.Vectors
{
    public struct Vector2US : IEnumerable<ushort>
    {
        // vec 1
        public ushort X;
        public ushort Y;

        // vec 2
        public Vector2US XX => new Vector2US(X, X);
        public Vector2US XY => new Vector2US(X, Y);
        public Vector2US YX => new Vector2US(Y, X);
        public Vector2US YY => new Vector2US(Y, Y);

        // vec 3
        public Vector3US XXX => new Vector3US(X, X, X);
        public Vector3US XXY => new Vector3US(X, X, Y);
        public Vector3US XYX => new Vector3US(X, Y, X);
        public Vector3US XYY => new Vector3US(X, Y, Y);
        public Vector3US YXX => new Vector3US(Y, X, X);
        public Vector3US YXY => new Vector3US(Y, X, Y);
        public Vector3US YYX => new Vector3US(Y, Y, X);
        public Vector3US YYY => new Vector3US(Y, Y, Y);

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
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        public double LengthSquared => X * X + Y * Y;
        public double Length => Math.Sqrt(LengthSquared);

        public static readonly Vector2US Zero = default;

        public static readonly Vector2US One = new Vector2US(1, 1);

        public static readonly Vector2US UnitX = new Vector2US(1, 0);
        public static readonly Vector2US UnitY = new Vector2US(0, 1);

        public Vector2US(int _x, int _y)
            : this((ushort)_x, (ushort)_y)
        {
        }
        public Vector2US(ushort _x, ushort _y)
        {
            X = _x;
            Y = _y;
        }

        public IEnumerator<ushort> GetEnumerator()
            => new ArrayEnumerator<ushort>(X, Y);

        IEnumerator IEnumerable.GetEnumerator()
            => new ArrayEnumerator<ushort>(X, Y);

        public static Vector2US Min(Vector2US a, Vector2US b)
            => new Vector2US(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
        public static Vector2US Max(Vector2US a, Vector2US b)
            => new Vector2US(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));

        public static double Distance(Vector2US a, Vector2US b)
            => (a - b).Length;

        public static int Dot(Vector2US a, Vector2US b)
            => a.X * b.X + a.Y * b.Y;

        public static Vector2US operator +(Vector2US a, Vector2US b)
            => new Vector2US(a.X + b.X, a.Y + b.Y);
        public static Vector2US operator -(Vector2US a, Vector2US b)
            => new Vector2US(a.X - b.X, a.Y - b.Y);
        public static Vector2US operator *(Vector2US a, Vector2US b)
            => new Vector2US(a.X * b.X, a.Y * b.Y);
        public static Vector2US operator /(Vector2US a, Vector2US b)
            => new Vector2US(a.X / b.X, a.Y / b.Y);
        public static Vector2US operator %(Vector2US a, Vector2US b)
            => new Vector2US(a.X % b.X, a.Y % b.Y);

        public static Vector2US operator *(Vector2US a, ushort b)
            => new Vector2US(a.X * b, a.Y * b);
        public static Vector2US operator /(Vector2US a, ushort b)
            => new Vector2US(a.X / b, a.Y / b);
        public static Vector2US operator %(Vector2US a, ushort b)
            => new Vector2US(a.X % b, a.Y % b);

        public static bool operator ==(Vector2US a, Vector2US b)
            => a.Equals(b);
        public static bool operator !=(Vector2US a, Vector2US b)
            => !a.Equals(b);

        public static explicit operator Vector2US(Vector2F v)
            => new Vector2US((ushort)v.X, (ushort)v.Y);
        public static explicit operator Vector2US(Vector2I v)
            => new Vector2US((ushort)v.X, (ushort)v.Y);
        public static explicit operator Vector2US(Vector2S v)
            => new Vector2US((ushort)v.X, (ushort)v.Y);
        public static implicit operator Vector2US(Vector2B v)
            => new Vector2US(v.X, v.Y);

        public override int GetHashCode()
            => HashCode.Combine(X, Y);

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is Vector2US other) return Equals(other);
            else return false;
        }

        public bool Equals(Vector2US other)
            => X == other.X && Y == other.Y;

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
            sb.Append('>');
            return sb.ToString();
        }
    }
}
