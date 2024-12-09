using MathUtils.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace MathUtils.Vectors;

public struct ushort3 : IEnumerable<ushort>
{
	// vec 1
	public ushort X;
	public ushort Y;
	public ushort Z;

	// vec2
	public ushort2 XX => new ushort2(X, X);
	public ushort2 XY => new ushort2(X, Y);
	public ushort2 XZ => new ushort2(X, Z);
	public ushort2 YX => new ushort2(Y, X);
	public ushort2 YY => new ushort2(Y, Y);
	public ushort2 YZ => new ushort2(Y, Z);
	public ushort2 ZX => new ushort2(Z, X);
	public ushort2 ZY => new ushort2(Z, Y);
	public ushort2 ZZ => new ushort2(Z, Z);

	// vec 3
	public ushort3 XXX => new ushort3(X, X, X);
	public ushort3 XXY => new ushort3(X, X, Y);
	public ushort3 XXZ => new ushort3(X, X, Z);
	public ushort3 XYX => new ushort3(X, Y, X);
	public ushort3 XYY => new ushort3(X, Y, Y);
	public ushort3 XYZ => new ushort3(X, Y, Z);
	public ushort3 XZX => new ushort3(X, Z, X);
	public ushort3 XZY => new ushort3(X, Z, Y);
	public ushort3 XZZ => new ushort3(X, Z, Z);
	public ushort3 YXX => new ushort3(Y, X, X);
	public ushort3 YXY => new ushort3(Y, X, Y);
	public ushort3 YXZ => new ushort3(Y, X, Z);
	public ushort3 YYX => new ushort3(Y, Y, X);
	public ushort3 YYY => new ushort3(Y, Y, Y);
	public ushort3 YYZ => new ushort3(Y, Y, Z);
	public ushort3 YZX => new ushort3(Y, Z, X);
	public ushort3 YZY => new ushort3(Y, Z, Y);
	public ushort3 YZZ => new ushort3(Y, Z, Z);
	public ushort3 ZXX => new ushort3(Z, X, X);
	public ushort3 ZXY => new ushort3(Z, X, Y);
	public ushort3 ZXZ => new ushort3(Z, X, Z);
	public ushort3 ZYX => new ushort3(Z, Y, X);
	public ushort3 ZYY => new ushort3(Z, Y, Y);
	public ushort3 ZYZ => new ushort3(Z, Y, Z);
	public ushort3 ZZX => new ushort3(Z, Z, X);
	public ushort3 ZZY => new ushort3(Z, Z, Y);
	public ushort3 ZZZ => new ushort3(Z, Z, Z);

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

	public static readonly ushort3 Zero = new ushort3(0, 0, 0);
	public static readonly ushort3 One = new ushort3(1, 1, 1);

	public static readonly ushort3 UnitX = new ushort3(1, 0, 0);
	public static readonly ushort3 UnitY = new ushort3(0, 1, 0);
	public static readonly ushort3 UnitZ = new ushort3(0, 0, 1);

	public ushort3(int _x, int _y, int _z)
		: this((ushort)_x, (ushort)_y, (ushort)_z)
	{
	}
	public ushort3(ushort _x, ushort _y, ushort _z)
	{
		X = _x;
		Y = _y;
		Z = _z;
	}

	public IEnumerator<ushort> GetEnumerator()
		=> new ArrayEnumerator<ushort>(X, Y, Z);

	IEnumerator IEnumerable.GetEnumerator()
		=> new ArrayEnumerator<ushort>(X, Y, Z);

	public static ushort3 Min(ushort3 a, ushort3 b)
		=> new ushort3(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
	public static ushort3 Max(ushort3 a, ushort3 b)
		=> new ushort3(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));

	public static double Distance(ushort3 a, ushort3 b)
		=> (a - b).Length;

	public static int Dot(ushort3 a, ushort3 b)
		=> a.X * b.X + a.Y * b.Y + a.Z;
	public static ushort3 Cross(ushort3 a, ushort3 b)
		=> new ushort3(
			a.Y * b.Z - a.Z * b.Y,
			a.Z * b.X - a.X * b.Z,
			a.X * b.Y - a.Y * b.X
		);

	public static ushort3 operator +(ushort3 a, ushort3 b)
		=> new ushort3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
	public static ushort3 operator -(ushort3 a, ushort3 b)
		=> new ushort3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
	public static ushort3 operator *(ushort3 a, ushort3 b)
		=> new ushort3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
	public static ushort3 operator /(ushort3 a, ushort3 b)
		=> new ushort3(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
	public static ushort3 operator %(ushort3 a, ushort3 b)
		=> new ushort3(a.X % b.X, a.Y % b.Y, a.Z % b.Z);

	public static ushort3 operator *(ushort3 a, int b)
		=> new ushort3(a.X * b, a.Y * b, a.Z * b);
	public static ushort3 operator /(ushort3 a, int b)
		=> new ushort3(a.X / b, a.Y / b, a.Z / b);
	public static ushort3 operator %(ushort3 a, int b)
		=> new ushort3(a.X % b, a.Y % b, a.Z % b);

	public static bool operator ==(ushort3 a, ushort3 b)
		=> a.Equals(b);
	public static bool operator !=(ushort3 a, ushort3 b)
		=> !a.Equals(b);

	public static explicit operator ushort3(float3 v)
		=> new ushort3((ushort)v.X, (ushort)v.Y, (ushort)v.Z);
	public static explicit operator ushort3(int3 v)
		=> new ushort3((ushort)v.X, (ushort)v.Y, (ushort)v.Z);
	public static explicit operator ushort3(short3 v)
		=> new ushort3((ushort)v.X, (ushort)v.Y, (ushort)v.Z);
	public static implicit operator ushort3(byte3 v)
		=> new ushort3(v.X, v.Y, v.Z);

	public override int GetHashCode()
		=> HashCode.Combine(X, Y, Z);

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is ushort3 other) return Equals(other);
		else return false;
	}

	public bool Equals(ushort3 other)
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
