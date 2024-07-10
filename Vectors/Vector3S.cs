using MathUtils.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace MathUtils.Vectors
{
    public struct Vector3S : IEnumerable<short>
    {
        // vec 1
        public short X;
        public short Y;
        public short Z;

        // vec2
        public Vector2S XX => new Vector2S(X, X);
        public Vector2S XY => new Vector2S(X, Y);
        public Vector2S XZ => new Vector2S(X, Z);
        public Vector2S YX => new Vector2S(Y, X);
        public Vector2S YY => new Vector2S(Y, Y);
        public Vector2S YZ => new Vector2S(Y, Z);
        public Vector2S ZX => new Vector2S(Z, X);
        public Vector2S ZY => new Vector2S(Z, Y);
        public Vector2S ZZ => new Vector2S(Z, Z);

        // vec 3
        public Vector3S XXX => new Vector3S(X, X, X);
        public Vector3S XXY => new Vector3S(X, X, Y);
        public Vector3S XXZ => new Vector3S(X, X, Z);
        public Vector3S XYX => new Vector3S(X, Y, X);
        public Vector3S XYY => new Vector3S(X, Y, Y);
        public Vector3S XYZ => new Vector3S(X, Y, Z);
        public Vector3S XZX => new Vector3S(X, Z, X);
        public Vector3S XZY => new Vector3S(X, Z, Y);
        public Vector3S XZZ => new Vector3S(X, Z, Z);
        public Vector3S YXX => new Vector3S(Y, X, X);
        public Vector3S YXY => new Vector3S(Y, X, Y);
        public Vector3S YXZ => new Vector3S(Y, X, Z);
        public Vector3S YYX => new Vector3S(Y, Y, X);
        public Vector3S YYY => new Vector3S(Y, Y, Y);
        public Vector3S YYZ => new Vector3S(Y, Y, Z);
        public Vector3S YZX => new Vector3S(Y, Z, X);
        public Vector3S YZY => new Vector3S(Y, Z, Y);
        public Vector3S YZZ => new Vector3S(Y, Z, Z);
        public Vector3S ZXX => new Vector3S(Z, X, X);
        public Vector3S ZXY => new Vector3S(Z, X, Y);
        public Vector3S ZXZ => new Vector3S(Z, X, Z);
        public Vector3S ZYX => new Vector3S(Z, Y, X);
        public Vector3S ZYY => new Vector3S(Z, Y, Y);
        public Vector3S ZYZ => new Vector3S(Z, Y, Z);
        public Vector3S ZZX => new Vector3S(Z, Z, X);
        public Vector3S ZZY => new Vector3S(Z, Z, Y);
        public Vector3S ZZZ => new Vector3S(Z, Z, Z);

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

        public static readonly Vector3S Zero = new Vector3S(0, 0, 0);
        public static readonly Vector3S One = new Vector3S(1, 1, 1);

        public static readonly Vector3S UnitX = new Vector3S(1, 0, 0);
        public static readonly Vector3S UnitY = new Vector3S(0, 1, 0);
        public static readonly Vector3S UnitZ = new Vector3S(0, 0, 1);

        public Vector3S(int _x, int _y, int _z)
            : this((short)_x, (short)_y, (short)_z)
        {
        }
        public Vector3S(short _x, short _y, short _z)
        {
            X = _x;
            Y = _y;
            Z = _z;
        }

        public IEnumerator<short> GetEnumerator()
            => new ArrayEnumerator<short>(X, Y, Z);

        IEnumerator IEnumerable.GetEnumerator()
            => new ArrayEnumerator<short>(X, Y, Z);

        public static Vector3S Min(Vector3S a, Vector3S b)
            => new Vector3S(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));

        public static Vector3S Max(Vector3S a, Vector3S b)
            => new Vector3S(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));

        public static Vector3S operator +(Vector3S a, Vector3S b)
            => new Vector3S(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector3S operator -(Vector3S a, Vector3S b)
            => new Vector3S(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vector3S operator *(Vector3S a, Vector3S b)
            => new Vector3S(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        public static Vector3S operator /(Vector3S a, Vector3S b)
            => new Vector3S(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        public static Vector3S operator %(Vector3S a, Vector3S b)
            => new Vector3S(a.X % b.X, a.Y % b.Y, a.Z % b.Z);

        public static Vector3S operator -(Vector3S a)
            => new Vector3S(-a.X, -a.Y, -a.Z);

        public static Vector3S operator *(Vector3S a, int b)
            => new Vector3S(a.X * b, a.Y * b, a.Z * b);
        public static Vector3S operator /(Vector3S a, int b)
            => new Vector3S(a.X / b, a.Y / b, a.Z / b);
        public static Vector3S operator %(Vector3S a, int b)
            => new Vector3S(a.X % b, a.Y % b, a.Z % b);

        public static bool operator ==(Vector3S a, Vector3S b)
            => a.Equals(b);
        public static bool operator !=(Vector3S a, Vector3S b)
            => !a.Equals(b);

        public static explicit operator Vector3S(Vector3F v)
            => new Vector3S((short)v.X, (short)v.Y, (short)v.Z);
        public static explicit operator Vector3S(Vector3I v)
            => new Vector3S((short)v.X, (short)v.Y, (short)v.Z);

        public override int GetHashCode()
            => HashCode.Combine(X, Y, Z);

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is Vector3S other) return Equals(other);
            else return false;
        }

        public bool Equals(Vector3S other)
            => X == other.X && Y == other.Y && Z == other.Z;

        /// <summary>
        /// Returns a String representing this Vector2 instance.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
            => ToString("G", CultureInfo.CurrentCulture);

        /// <summary>
        /// Returns a String representing this Vector2 instance, using the specified format to format individual elements.
        /// </summary>
        /// <param name="format">The format of individual elements.</param>
        /// <returns>The string representation.</returns>
        public string ToString(string format)
            => ToString(format, CultureInfo.InvariantCulture);

        /// <summary>
        /// Returns a String representing this Vector2 instance, using the specified format to format individual elements 
        /// and the given IFormatProvider.
        /// </summary>
        /// <param name="format">The format of individual elements.</param>
        /// <param name="formatProvider">The format provider to use when formatting elements.</param>
        /// <returns>The string representation.</returns>
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
