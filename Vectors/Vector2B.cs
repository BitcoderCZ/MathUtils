using MathUtils.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace MathUtils.Vectors
{
    public struct Vector2B : IEnumerable<byte>
    {
        // vec 1
        public byte X;
        public byte Y;

        // vec 2
        public Vector2B XX => new Vector2B(X, X);
        public Vector2B XY => new Vector2B(X, Y);
        public Vector2B YX => new Vector2B(Y, X);
        public Vector2B YY => new Vector2B(Y, Y);

        // vec 3
        public Vector3B XXX => new Vector3B(X, X, X);
        public Vector3B XXY => new Vector3B(X, X, Y);
        public Vector3B XYX => new Vector3B(X, Y, X);
        public Vector3B XYY => new Vector3B(X, Y, Y);
        public Vector3B YXX => new Vector3B(Y, X, X);
        public Vector3B YXY => new Vector3B(Y, X, Y);
        public Vector3B YYX => new Vector3B(Y, Y, X);
        public Vector3B YYY => new Vector3B(Y, Y, Y);

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

        public static readonly Vector2B Zero = default;

        public static readonly Vector2B One = new Vector2B(1, 1);

        public static readonly Vector2B UnitX = new Vector2B(1, 0);
        public static readonly Vector2B UnitY = new Vector2B(0, 1);

        public Vector2B(int _x, int _y)
            : this((byte)_x, (byte)_y)
        {
        }
        public Vector2B(byte _x, byte _y)
        {
            X = _x;
            Y = _y;
        }

        public IEnumerator<byte> GetEnumerator()
            => new ArrayEnumerator<byte>(X, Y);

        IEnumerator IEnumerable.GetEnumerator()
            => new ArrayEnumerator<byte>(X, Y);

        public static Vector2B Min(Vector2B a, Vector2B b)
            => new Vector2B(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
        public static Vector2B Max(Vector2B a, Vector2B b)
            => new Vector2B(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));

        public static double Distance(Vector2B a, Vector2B b)
            => (a - b).Length;

        public static int Dot(Vector2B a, Vector2B b)
            => a.X * b.X + a.Y * b.Y;

        public static Vector2B operator +(Vector2B a, Vector2B b)
            => new Vector2B(a.X + b.X, a.Y + b.Y);
        public static Vector2B operator -(Vector2B a, Vector2B b)
            => new Vector2B(a.X - b.X, a.Y - b.Y);
        public static Vector2B operator *(Vector2B a, Vector2B b)
            => new Vector2B(a.X * b.X, a.Y * b.Y);
        public static Vector2B operator /(Vector2B a, Vector2B b)
            => new Vector2B(a.X / b.X, a.Y / b.Y);
        public static Vector2B operator %(Vector2B a, Vector2B b)
            => new Vector2B(a.X % b.X, a.Y % b.Y);

        public static Vector2B operator *(Vector2B a, byte b)
            => new Vector2B(a.X * b, a.Y * b);
        public static Vector2B operator /(Vector2B a, byte b)
            => new Vector2B(a.X / b, a.Y / b);
        public static Vector2B operator %(Vector2B a, byte b)
            => new Vector2B(a.X % b, a.Y % b);

        public static bool operator ==(Vector2B a, Vector2B b)
            => a.Equals(b);
        public static bool operator !=(Vector2B a, Vector2B b)
            => !a.Equals(b);

        public static explicit operator Vector2B(Vector2F v)
            => new Vector2B((byte)v.X, (byte)v.Y);
        public static explicit operator Vector2B(Vector2I v)
            => new Vector2B((byte)v.X, (byte)v.Y);
        public static explicit operator Vector2B(Vector2S v)
            => new Vector2B((byte)v.X, (byte)v.Y);
        public static explicit operator Vector2B(Vector2US v)
            => new Vector2B((byte)v.X, (byte)v.Y);

        public override int GetHashCode()
            => HashCode.Combine(X, Y);

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is Vector2B other) return Equals(other);
            else return false;
        }

        public bool Equals(Vector2B other)
            => X == other.X && Y == other.Y;

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
            sb.Append('>');
            return sb.ToString();
        }
    }
}
