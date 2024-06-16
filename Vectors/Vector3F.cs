using MathUtils.Utils;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace MathUtils.Vectors
{
    public struct Vector3F : IEnumerable<float>
    {
        // vec 1
        public float X;
        public float Y;
        public float Z;

        // vec2
        public Vector2F XX => new Vector2F(X, X);
        public Vector2F XY => new Vector2F(X, Y);
        public Vector2F XZ => new Vector2F(X, Z);
        public Vector2F YX => new Vector2F(Y, X);
        public Vector2F YY => new Vector2F(Y, Y);
        public Vector2F YZ => new Vector2F(Y, Z);
        public Vector2F ZX => new Vector2F(Z, X);
        public Vector2F ZY => new Vector2F(Z, Y);
        public Vector2F ZZ => new Vector2F(Z, Z);

        // vec 3
        public Vector3F XXX => new Vector3F(X, X, X);
        public Vector3F XXY => new Vector3F(X, X, Y);
        public Vector3F XXZ => new Vector3F(X, X, Z);
        public Vector3F XYX => new Vector3F(X, Y, X);
        public Vector3F XYY => new Vector3F(X, Y, Y);
        public Vector3F XYZ => new Vector3F(X, Y, Z);
        public Vector3F XZX => new Vector3F(X, Z, X);
        public Vector3F XZY => new Vector3F(X, Z, Y);
        public Vector3F XZZ => new Vector3F(X, Z, Z);
        public Vector3F YXX => new Vector3F(Y, X, X);
        public Vector3F YXY => new Vector3F(Y, X, Y);
        public Vector3F YXZ => new Vector3F(Y, X, Z);
        public Vector3F YYX => new Vector3F(Y, Y, X);
        public Vector3F YYY => new Vector3F(Y, Y, Y);
        public Vector3F YYZ => new Vector3F(Y, Y, Z);
        public Vector3F YZX => new Vector3F(Y, Z, X);
        public Vector3F YZY => new Vector3F(Y, Z, Y);
        public Vector3F YZZ => new Vector3F(Y, Z, Z);
        public Vector3F ZXX => new Vector3F(Z, X, X);
        public Vector3F ZXY => new Vector3F(Z, X, Y);
        public Vector3F ZXZ => new Vector3F(Z, X, Z);
        public Vector3F ZYX => new Vector3F(Z, Y, X);
        public Vector3F ZYY => new Vector3F(Z, Y, Y);
        public Vector3F ZYZ => new Vector3F(Z, Y, Z);
        public Vector3F ZZX => new Vector3F(Z, Z, X);
        public Vector3F ZZY => new Vector3F(Z, Z, Y);
        public Vector3F ZZZ => new Vector3F(Z, Z, Z);

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

        public static readonly Vector3F Zero = new Vector3F(0f, 0f, 0f);
        public static readonly Vector3F One = new Vector3F(1f, 1f, 1f);

        public static readonly Vector3F UnitX = new Vector3F(1f, 0f, 0f);
        public static readonly Vector3F UnitY = new Vector3F(0f, 1f, 0f);
        public static readonly Vector3F UnitZ = new Vector3F(0f, 0f, 1f);

        public Vector3F(float _x, float _y, float _z)
        {
            X = _x;
            Y = _y;
            Z = _z;
        }

        public IEnumerator<float> GetEnumerator()
            => new ArrayEnumerator<float>(X, Y, Z);

        IEnumerator IEnumerable.GetEnumerator()
            => new ArrayEnumerator<float>(X, Y, Z);

        public static Vector3F operator +(Vector3F a, Vector3F b)
            => new Vector3F(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector3F operator -(Vector3F a, Vector3F b)
            => new Vector3F(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vector3F operator *(Vector3F a, Vector3F b)
            => new Vector3F(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        public static Vector3F operator /(Vector3F a, Vector3F b)
            => new Vector3F(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        public static Vector3F operator %(Vector3F a, Vector3F b)
            => new Vector3F(a.X % b.X, a.Y % b.Y, a.Z % b.Z);

        public static Vector3F operator -(Vector3F a)
            => new Vector3F(-a.X, -a.Y, -a.Z);

        public static Vector3F operator *(Vector3F a, float b)
            => new Vector3F(a.X * b, a.Y * b, a.Z * b);
        public static Vector3F operator /(Vector3F a, float b)
            => new Vector3F(a.X / b, a.Y / b, a.Z / b);
        public static Vector3F operator %(Vector3F a, float b)
            => new Vector3F(a.X % b, a.Y % b, a.Z % b);

        public static bool operator ==(Vector3F a, Vector3F b)
            => a.Equals(b);
        public static bool operator !=(Vector3F a, Vector3F b)
            => !a.Equals(b);

        public static implicit operator Vector3F(Vector3I v)
            => new Vector3F(v.X, v.Y, v.Z);

        public override int GetHashCode()
            => HashCode.Combine(X, Y, Z);

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is Vector3F other) return Equals(other);
            else return false;
        }

        public bool Equals(Vector3F other)
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
