﻿using MathUtils.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathUtils.Vectors
{
    public struct Vector2I : IEnumerable<int>
    {
        // vec 1
        public int X { get; set; }
        public int Y { get; set; }

        // vec 2
        public Vector2I XX => new Vector2I(X, X);
        public Vector2I XY => new Vector2I(X, Y);
        public Vector2I YX => new Vector2I(Y, X);
        public Vector2I YY => new Vector2I(Y, Y);

        // vec 3
        public Vector3I XXX => new Vector3I(X, X, X);
        public Vector3I XXY => new Vector3I(X, X, Y);
        public Vector3I XYX => new Vector3I(X, Y, X);
        public Vector3I XYY => new Vector3I(X, Y, Y);
        public Vector3I YXX => new Vector3I(Y, X, X);
        public Vector3I YXY => new Vector3I(Y, X, Y);
        public Vector3I YYX => new Vector3I(Y, Y, X);
        public Vector3I YYY => new Vector3I(Y, Y, Y);

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

        public static readonly Vector2I Zero = default;

        public static readonly Vector2I One = new Vector2I(1, 1);

        public static readonly Vector2I UnitX = new Vector2I(1, 0);
        public static readonly Vector2I UnitY = new Vector2I(0, 1);

        public Vector2I(int _x, int _y)
        {
            X = _x;
            Y = _y;
        }

        public IEnumerator<int> GetEnumerator()
            => new ArrayEnumerator<int>(X, Y);

        IEnumerator IEnumerable.GetEnumerator()
            => new ArrayEnumerator<int>(X, Y);

        public static Vector2I operator +(Vector2I a, Vector2I b)
            => new Vector2I(a.X + b.X, a.Y + b.Y);
        public static Vector2I operator -(Vector2I a, Vector2I b)
            => new Vector2I(a.X - b.X, a.Y - b.Y);
        public static Vector2I operator *(Vector2I a, Vector2I b)
            => new Vector2I(a.X * b.X, a.Y * b.Y);
        public static Vector2I operator /(Vector2I a, Vector2I b)
            => new Vector2I(a.X / b.X, a.Y / b.Y);
        public static Vector2I operator %(Vector2I a, Vector2I b)
            => new Vector2I(a.X % b.X, a.Y % b.Y);

        public static Vector2I operator -(Vector2I a)
            => new Vector2I(-a.X, -a.Y);

        public static Vector2I operator *(Vector2I a, int b)
            => new Vector2I(a.X * b, a.Y * b);
        public static Vector2I operator /(Vector2I a, int b)
            => new Vector2I(a.X / b, a.Y / b);
        public static Vector2I operator %(Vector2I a, int b)
            => new Vector2I(a.X % b, a.Y % b);

        public static bool operator ==(Vector2I a, Vector2I b)
            => a.Equals(b);
        public static bool operator !=(Vector2I a, Vector2I b)
            => !a.Equals(b);

        public static explicit operator Vector2I(Vector2F v)
            => new Vector2I((int)v.X, (int)v.Y);

        public override int GetHashCode()
            => HashCode.Combine(X, Y);

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is Vector2I other) return Equals(other);
            else return false;
        }

        public bool Equals(Vector2I other)
            => X == other.X && Y == other.Y;

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
            sb.Append('>');
            return sb.ToString();
        }
    }
}
