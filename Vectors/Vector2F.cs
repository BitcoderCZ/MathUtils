﻿using MathUtils.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace MathUtils.Vectors
{
    public struct Vector2F : IEnumerable<float>
    {
        // vec 1
        public float X;
        public float Y;

        // vec 2
        public Vector2F XX => new Vector2F(X, X);
        public Vector2F XY => new Vector2F(X, Y);
        public Vector2F YX => new Vector2F(Y, X);
        public Vector2F YY => new Vector2F(Y, Y);

        // vec 3
        public Vector3F XXX => new Vector3F(X, X, X);
        public Vector3F XXY => new Vector3F(X, X, Y);
        public Vector3F XYX => new Vector3F(X, Y, X);
        public Vector3F XYY => new Vector3F(X, Y, Y);
        public Vector3F YXX => new Vector3F(Y, X, X);
        public Vector3F YXY => new Vector3F(Y, X, Y);
        public Vector3F YYX => new Vector3F(Y, Y, X);
        public Vector3F YYY => new Vector3F(Y, Y, Y);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public float this[int index]
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

        public static readonly Vector2F Zero = new Vector2F(0f, 0f);

        public static readonly Vector2F One = new Vector2F(1f, 1f);

        public static readonly Vector2F UnitX = new Vector2F(1f, 0f);
        public static readonly Vector2F UnitY = new Vector2F(0f, 1f);

        public Vector2F(float _x, float _y)
        {
            X = _x;
            Y = _y;
        }

        public Vector2F Normalized()
            => this / (float)Length;

        public IEnumerator<float> GetEnumerator()
            => new ArrayEnumerator<float>(X, Y);

        IEnumerator IEnumerable.GetEnumerator()
            => new ArrayEnumerator<float>(X, Y);

        public static Vector2F Min(Vector2F a, Vector2F b)
            => new Vector2F(MathF.Min(a.X, b.X), MathF.Min(a.Y, b.Y));
        public static Vector2F Max(Vector2F a, Vector2F b)
            => new Vector2F(MathF.Max(a.X, b.X), MathF.Max(a.Y, b.Y));

        public static double Distance(Vector2F a, Vector2F b)
            => (a - b).Length;

        public static float Dot(Vector2F a, Vector2F b)
            => a.X * b.X + a.Y * b.Y;

        public static Vector2F operator +(Vector2F a, Vector2F b)
            => new Vector2F(a.X + b.X, a.Y + b.Y);
        public static Vector2F operator -(Vector2F a, Vector2F b)
            => new Vector2F(a.X - b.X, a.Y - b.Y);
        public static Vector2F operator *(Vector2F a, Vector2F b)
            => new Vector2F(a.X * b.X, a.Y * b.Y);
        public static Vector2F operator /(Vector2F a, Vector2F b)
            => new Vector2F(a.X / b.X, a.Y / b.Y);
        public static Vector2F operator %(Vector2F a, Vector2F b)
            => new Vector2F(a.X % b.X, a.Y % b.Y);

        public static Vector2F operator -(Vector2F a)
            => new Vector2F(-a.X, -a.Y);

        public static Vector2F operator *(Vector2F a, float b)
            => new Vector2F(a.X * b, a.Y * b);
        public static Vector2F operator /(Vector2F a, float b)
            => new Vector2F(a.X / b, a.Y / b);
        public static Vector2F operator %(Vector2F a, float b)
            => new Vector2F(a.X % b, a.Y % b);

        public static bool operator ==(Vector2F a, Vector2F b)
            => a.Equals(b);
        public static bool operator !=(Vector2F a, Vector2F b)
            => !a.Equals(b);

        public static implicit operator Vector2F(Vector2I v)
            => new Vector2F(v.X, v.Y);
        public static implicit operator Vector2F(Vector2S v)
            => new Vector2F(v.X, v.Y);
        public static implicit operator Vector2F(Vector2US v)
            => new Vector2F(v.X, v.Y);
        public static implicit operator Vector2F(Vector2B v)
            => new Vector2F(v.X, v.Y);

        public override int GetHashCode()
            => HashCode.Combine(X, Y);

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is Vector2I other) return Equals(other);
            else return false;
        }

        public bool Equals(Vector2I other)
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
