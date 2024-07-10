using MathUtils.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace MathUtils.Vectors
{
    public struct Vector3I : IEnumerable<int>
    {
        // vec 1
        public int X;
        public int Y;
        public int Z;

        // vec2
        public Vector2I XX => new Vector2I(X, X);
        public Vector2I XY => new Vector2I(X, Y);
        public Vector2I XZ => new Vector2I(X, Z);
        public Vector2I YX => new Vector2I(Y, X);
        public Vector2I YY => new Vector2I(Y, Y);
        public Vector2I YZ => new Vector2I(Y, Z);
        public Vector2I ZX => new Vector2I(Z, X);
        public Vector2I ZY => new Vector2I(Z, Y);
        public Vector2I ZZ => new Vector2I(Z, Z);

        // vec 3
        public Vector3I XXX => new Vector3I(X, X, X);
        public Vector3I XXY => new Vector3I(X, X, Y);
        public Vector3I XXZ => new Vector3I(X, X, Z);
        public Vector3I XYX => new Vector3I(X, Y, X);
        public Vector3I XYY => new Vector3I(X, Y, Y);
        public Vector3I XYZ => new Vector3I(X, Y, Z);
        public Vector3I XZX => new Vector3I(X, Z, X);
        public Vector3I XZY => new Vector3I(X, Z, Y);
        public Vector3I XZZ => new Vector3I(X, Z, Z);
        public Vector3I YXX => new Vector3I(Y, X, X);
        public Vector3I YXY => new Vector3I(Y, X, Y);
        public Vector3I YXZ => new Vector3I(Y, X, Z);
        public Vector3I YYX => new Vector3I(Y, Y, X);
        public Vector3I YYY => new Vector3I(Y, Y, Y);
        public Vector3I YYZ => new Vector3I(Y, Y, Z);
        public Vector3I YZX => new Vector3I(Y, Z, X);
        public Vector3I YZY => new Vector3I(Y, Z, Y);
        public Vector3I YZZ => new Vector3I(Y, Z, Z);
        public Vector3I ZXX => new Vector3I(Z, X, X);
        public Vector3I ZXY => new Vector3I(Z, X, Y);
        public Vector3I ZXZ => new Vector3I(Z, X, Z);
        public Vector3I ZYX => new Vector3I(Z, Y, X);
        public Vector3I ZYY => new Vector3I(Z, Y, Y);
        public Vector3I ZYZ => new Vector3I(Z, Y, Z);
        public Vector3I ZZX => new Vector3I(Z, Z, X);
        public Vector3I ZZY => new Vector3I(Z, Z, Y);
        public Vector3I ZZZ => new Vector3I(Z, Z, Z);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public int this[int index]
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

        public static readonly Vector3I Zero = new Vector3I(0, 0, 0);
        public static readonly Vector3I One = new Vector3I(1, 1, 1);

        public static readonly Vector3I UnitX = new Vector3I(1, 0, 0);
        public static readonly Vector3I UnitY = new Vector3I(0, 1, 0);
        public static readonly Vector3I UnitZ = new Vector3I(0, 0, 1);

        public Vector3I(int _x, int _y, int _z)
        {
            X = _x;
            Y = _y;
            Z = _z;
        }

        public IEnumerator<int> GetEnumerator()
            => new ArrayEnumerator<int>(X, Y, Z);

        IEnumerator IEnumerable.GetEnumerator()
            => new ArrayEnumerator<int>(X, Y, Z);

        public static Vector3I Min(Vector3I a, Vector3I b)
            => new Vector3I(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));

        public static Vector3I Max(Vector3I a, Vector3I b)
            => new Vector3I(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));

        public static Vector3I operator +(Vector3I a, Vector3I b)
            => new Vector3I(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector3I operator -(Vector3I a, Vector3I b)
            => new Vector3I(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vector3I operator *(Vector3I a, Vector3I b)
            => new Vector3I(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        public static Vector3I operator /(Vector3I a, Vector3I b)
            => new Vector3I(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        public static Vector3I operator %(Vector3I a, Vector3I b)
            => new Vector3I(a.X % b.X, a.Y % b.Y, a.Z % b.Z);

        public static Vector3I operator -(Vector3I a)
            => new Vector3I(-a.X, -a.Y, -a.Z);

        public static Vector3I operator *(Vector3I a, int b)
            => new Vector3I(a.X * b, a.Y * b, a.Z * b);
        public static Vector3I operator /(Vector3I a, int b)
            => new Vector3I(a.X / b, a.Y / b, a.Z / b);
        public static Vector3I operator %(Vector3I a, int b)
            => new Vector3I(a.X % b, a.Y % b, a.Z % b);

        public static bool operator ==(Vector3I a, Vector3I b)
            => a.Equals(b);
        public static bool operator !=(Vector3I a, Vector3I b)
            => !a.Equals(b);

        public static explicit operator Vector3I(Vector3F v)
            => new Vector3I((int)v.X, (int)v.Y, (int)v.Z);
        public static implicit operator Vector3I(Vector3S v)
            => new Vector3I(v.X, v.Y, v.Z);

        public override int GetHashCode()
            => HashCode.Combine(X, Y, Z);

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is Vector3I other) return Equals(other);
            else return false;
        }

        public bool Equals(Vector3I other)
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
