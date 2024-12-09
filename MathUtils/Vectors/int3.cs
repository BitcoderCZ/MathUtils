using MathUtils.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace MathUtils.Vectors;

public struct int3 : IEnumerable<int>
{
	// vec 1
	public int X;
	public int Y;
	public int Z;

	// vec2
	public int2 XX => new int2(X, X);
	public int2 XY => new int2(X, Y);
	public int2 XZ => new int2(X, Z);
	public int2 YX => new int2(Y, X);
	public int2 YY => new int2(Y, Y);
	public int2 YZ => new int2(Y, Z);
	public int2 ZX => new int2(Z, X);
	public int2 ZY => new int2(Z, Y);
	public int2 ZZ => new int2(Z, Z);

	// vec 3
	public int3 XXX => new int3(X, X, X);
	public int3 XXY => new int3(X, X, Y);
	public int3 XXZ => new int3(X, X, Z);
	public int3 XYX => new int3(X, Y, X);
	public int3 XYY => new int3(X, Y, Y);
	public int3 XYZ => new int3(X, Y, Z);
	public int3 XZX => new int3(X, Z, X);
	public int3 XZY => new int3(X, Z, Y);
	public int3 XZZ => new int3(X, Z, Z);
	public int3 YXX => new int3(Y, X, X);
	public int3 YXY => new int3(Y, X, Y);
	public int3 YXZ => new int3(Y, X, Z);
	public int3 YYX => new int3(Y, Y, X);
	public int3 YYY => new int3(Y, Y, Y);
	public int3 YYZ => new int3(Y, Y, Z);
	public int3 YZX => new int3(Y, Z, X);
	public int3 YZY => new int3(Y, Z, Y);
	public int3 YZZ => new int3(Y, Z, Z);
	public int3 ZXX => new int3(Z, X, X);
	public int3 ZXY => new int3(Z, X, Y);
	public int3 ZXZ => new int3(Z, X, Z);
	public int3 ZYX => new int3(Z, Y, X);
	public int3 ZYY => new int3(Z, Y, Y);
	public int3 ZYZ => new int3(Z, Y, Z);
	public int3 ZZX => new int3(Z, Z, X);
	public int3 ZZY => new int3(Z, Z, Y);
	public int3 ZZZ => new int3(Z, Z, Z);

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

	public double LengthSquared => X * X + Y * Y + Z * Z;
	public double Length => Math.Sqrt(LengthSquared);

	public static readonly int3 Zero = new int3(0, 0, 0);
	public static readonly int3 One = new int3(1, 1, 1);

	public static readonly int3 UnitX = new int3(1, 0, 0);
	public static readonly int3 UnitY = new int3(0, 1, 0);
	public static readonly int3 UnitZ = new int3(0, 0, 1);

	public int3(int _x, int _y, int _z)
	{
		X = _x;
		Y = _y;
		Z = _z;
	}

	public IEnumerator<int> GetEnumerator()
		=> new ArrayEnumerator<int>(X, Y, Z);

	IEnumerator IEnumerable.GetEnumerator()
		=> new ArrayEnumerator<int>(X, Y, Z);

	public static int3 Min(int3 a, int3 b)
		=> new int3(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
	public static int3 Max(int3 a, int3 b)
		=> new int3(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));

	public static double Distance(int3 a, int3 b)
		=> (a - b).Length;

	public static int Dot(int3 a, int3 b)
		=> a.X * b.X + a.Y * b.Y + a.Z;
	public static int3 Cross(int3 a, int3 b)
		=> new int3(
			a.Y * b.Z - a.Z * b.Y,
			a.Z * b.X - a.X * b.Z,
			a.X * b.Y - a.Y * b.X
		);

	public static int3 operator +(int3 a, int3 b)
		=> new int3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
	public static int3 operator -(int3 a, int3 b)
		=> new int3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
	public static int3 operator *(int3 a, int3 b)
		=> new int3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
	public static int3 operator /(int3 a, int3 b)
		=> new int3(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
	public static int3 operator %(int3 a, int3 b)
		=> new int3(a.X % b.X, a.Y % b.Y, a.Z % b.Z);

	public static int3 operator -(int3 a)
		=> new int3(-a.X, -a.Y, -a.Z);

	public static int3 operator *(int3 a, int b)
		=> new int3(a.X * b, a.Y * b, a.Z * b);
	public static int3 operator /(int3 a, int b)
		=> new int3(a.X / b, a.Y / b, a.Z / b);
	public static int3 operator %(int3 a, int b)
		=> new int3(a.X % b, a.Y % b, a.Z % b);

	public static bool operator ==(int3 a, int3 b)
		=> a.Equals(b);
	public static bool operator !=(int3 a, int3 b)
		=> !a.Equals(b);

	public static explicit operator int3(float3 v)
		=> new int3((int)v.X, (int)v.Y, (int)v.Z);
	public static implicit operator int3(short3 v)
		=> new int3(v.X, v.Y, v.Z);
	public static implicit operator int3(ushort3 v)
		=> new int3(v.X, v.Y, v.Z);
	public static implicit operator int3(byte3 v)
		=> new int3(v.X, v.Y, v.Z);

	public override int GetHashCode()
		=> HashCode.Combine(X, Y, Z);

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is int3 other) return Equals(other);
		else return false;
	}

	public bool Equals(int3 other)
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
