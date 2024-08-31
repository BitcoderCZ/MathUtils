using MathUtils.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace MathUtils.Vectors
{
    public struct Vector2S : IEnumerable<short>
    {
        // vec 1
        public short X;
        public short Y;

        // vec 2
        public Vector2S XX => new Vector2S(X, X);
        public Vector2S XY => new Vector2S(X, Y);
        public Vector2S YX => new Vector2S(Y, X);
        public Vector2S YY => new Vector2S(Y, Y);

        // vec 3
        public Vector3S XXX => new Vector3S(X, X, X);
        public Vector3S XXY => new Vector3S(X, X, Y);
        public Vector3S XYX => new Vector3S(X, Y, X);
        public Vector3S XYY => new Vector3S(X, Y, Y);
        public Vector3S YXX => new Vector3S(Y, X, X);
        public Vector3S YXY => new Vector3S(Y, X, Y);
        public Vector3S YYX => new Vector3S(Y, Y, X);
        public Vector3S YYY => new Vector3S(Y, Y, Y);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public short this[int index]
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

        public static readonly Vector2S Zero = default;

        public static readonly Vector2S One = new Vector2S(1, 1);

        public static readonly Vector2S UnitX = new Vector2S(1, 0);
        public static readonly Vector2S UnitY = new Vector2S(0, 1);

        public Vector2S(int _x, int _y)
            : this((short)_x, (short)_y)
        {
        }
        public Vector2S(short _x, short _y)
        {
            X = _x;
            Y = _y;
        }

        public IEnumerator<short> GetEnumerator()
            => new ArrayEnumerator<short>(X, Y);

        IEnumerator IEnumerable.GetEnumerator()
            => new ArrayEnumerator<short>(X, Y);

        public static Vector2S Min(Vector2S a, Vector2S b)
            => new Vector2S(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
        public static Vector2S Max(Vector2S a, Vector2S b)
            => new Vector2S(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));

        public static double Distance(Vector2S a, Vector2S b)
            => (a - b).Length;

        public static int Dot(Vector2S a, Vector2S b)
            => a.X * b.X + a.Y * b.Y;

        public static Vector2S operator +(Vector2S a, Vector2S b)
            => new Vector2S(a.X + b.X, a.Y + b.Y);
        public static Vector2S operator -(Vector2S a, Vector2S b)
            => new Vector2S(a.X - b.X, a.Y - b.Y);
        public static Vector2S operator *(Vector2S a, Vector2S b)
            => new Vector2S(a.X * b.X, a.Y * b.Y);
        public static Vector2S operator /(Vector2S a, Vector2S b)
            => new Vector2S(a.X / b.X, a.Y / b.Y);
        public static Vector2S operator %(Vector2S a, Vector2S b)
            => new Vector2S(a.X % b.X, a.Y % b.Y);

        public static Vector2S operator -(Vector2S a)
            => new Vector2S(-a.X, -a.Y);

        public static Vector2S operator *(Vector2S a, short b)
            => new Vector2S(a.X * b, a.Y * b);
        public static Vector2S operator /(Vector2S a, short b)
            => new Vector2S(a.X / b, a.Y / b);
        public static Vector2S operator %(Vector2S a, short b)
            => new Vector2S(a.X % b, a.Y % b);

        public static bool operator ==(Vector2S a, Vector2S b)
            => a.Equals(b);
        public static bool operator !=(Vector2S a, Vector2S b)
            => !a.Equals(b);

        public static explicit operator Vector2S(Vector2F v)
            => new Vector2S((short)v.X, (short)v.Y);
        public static explicit operator Vector2S(Vector2I v)
            => new Vector2S((short)v.X, (short)v.Y);
        public static explicit operator Vector2S(Vector2US v)
            => new Vector2S((short)v.X, (short)v.Y);
        public static implicit operator Vector2S(Vector2B v)
            => new Vector2S(v.X, v.Y);

        public override int GetHashCode()
            => HashCode.Combine(X, Y);

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is Vector2S other) return Equals(other);
            else return false;
        }

        public bool Equals(Vector2S other)
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
