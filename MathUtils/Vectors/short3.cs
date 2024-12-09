using MathUtils.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace MathUtils.Vectors;

public struct short3 : IEnumerable<short>
{
	// vec 1
	public short X;
	public short Y;
	public short Z;

	// vec2
	public short2 XX => new short2(X, X);
	public short2 XY => new short2(X, Y);
	public short2 XZ => new short2(X, Z);
	public short2 YX => new short2(Y, X);
	public short2 YY => new short2(Y, Y);
	public short2 YZ => new short2(Y, Z);
	public short2 ZX => new short2(Z, X);
	public short2 ZY => new short2(Z, Y);
	public short2 ZZ => new short2(Z, Z);

	// vec 3
	public short3 XXX => new short3(X, X, X);
	public short3 XXY => new short3(X, X, Y);
	public short3 XXZ => new short3(X, X, Z);
	public short3 XYX => new short3(X, Y, X);
	public short3 XYY => new short3(X, Y, Y);
	public short3 XYZ => new short3(X, Y, Z);
	public short3 XZX => new short3(X, Z, X);
	public short3 XZY => new short3(X, Z, Y);
	public short3 XZZ => new short3(X, Z, Z);
	public short3 YXX => new short3(Y, X, X);
	public short3 YXY => new short3(Y, X, Y);
	public short3 YXZ => new short3(Y, X, Z);
	public short3 YYX => new short3(Y, Y, X);
	public short3 YYY => new short3(Y, Y, Y);
	public short3 YYZ => new short3(Y, Y, Z);
	public short3 YZX => new short3(Y, Z, X);
	public short3 YZY => new short3(Y, Z, Y);
	public short3 YZZ => new short3(Y, Z, Z);
	public short3 ZXX => new short3(Z, X, X);
	public short3 ZXY => new short3(Z, X, Y);
	public short3 ZXZ => new short3(Z, X, Z);
	public short3 ZYX => new short3(Z, Y, X);
	public short3 ZYY => new short3(Z, Y, Y);
	public short3 ZYZ => new short3(Z, Y, Z);
	public short3 ZZX => new short3(Z, Z, X);
	public short3 ZZY => new short3(Z, Z, Y);
	public short3 ZZZ => new short3(Z, Z, Z);

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

	public double LengthSquared => X * X + Y * Y + Z * Z;
	public double Length => Math.Sqrt(LengthSquared);

	public static readonly short3 Zero = new short3(0, 0, 0);
	public static readonly short3 One = new short3(1, 1, 1);

	public static readonly short3 UnitX = new short3(1, 0, 0);
	public static readonly short3 UnitY = new short3(0, 1, 0);
	public static readonly short3 UnitZ = new short3(0, 0, 1);

	public short3(int _x, int _y, int _z)
		: this((short)_x, (short)_y, (short)_z)
	{
	}
	public short3(short _x, short _y, short _z)
	{
		X = _x;
		Y = _y;
		Z = _z;
	}

	public IEnumerator<short> GetEnumerator()
		=> new ArrayEnumerator<short>(X, Y, Z);

	IEnumerator IEnumerable.GetEnumerator()
		=> new ArrayEnumerator<short>(X, Y, Z);

	public static short3 Min(short3 a, short3 b)
		=> new short3(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
	public static short3 Max(short3 a, short3 b)
		=> new short3(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));

	public static double Distance(short3 a, short3 b)
		=> (a - b).Length;

	public static int Dot(short3 a, short3 b)
		=> a.X * b.X + a.Y * b.Y + a.Z;
	public static short3 Cross(short3 a, short3 b)
		=> new short3(
			a.Y * b.Z - a.Z * b.Y,
			a.Z * b.X - a.X * b.Z,
			a.X * b.Y - a.Y * b.X
		);

	public static short3 operator +(short3 a, short3 b)
		=> new short3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
	public static short3 operator -(short3 a, short3 b)
		=> new short3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
	public static short3 operator *(short3 a, short3 b)
		=> new short3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
	public static short3 operator /(short3 a, short3 b)
		=> new short3(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
	public static short3 operator %(short3 a, short3 b)
		=> new short3(a.X % b.X, a.Y % b.Y, a.Z % b.Z);

	public static short3 operator -(short3 a)
		=> new short3(-a.X, -a.Y, -a.Z);

	public static short3 operator *(short3 a, int b)
		=> new short3(a.X * b, a.Y * b, a.Z * b);
	public static short3 operator /(short3 a, int b)
		=> new short3(a.X / b, a.Y / b, a.Z / b);
	public static short3 operator %(short3 a, int b)
		=> new short3(a.X % b, a.Y % b, a.Z % b);

	public static bool operator ==(short3 a, short3 b)
		=> a.Equals(b);
	public static bool operator !=(short3 a, short3 b)
		=> !a.Equals(b);

	public static explicit operator short3(float3 v)
		=> new short3((short)v.X, (short)v.Y, (short)v.Z);
	public static explicit operator short3(int3 v)
		=> new short3((short)v.X, (short)v.Y, (short)v.Z);
	public static explicit operator short3(ushort3 v)
		=> new short3((short)v.X, (short)v.Y, (short)v.Z);
	public static implicit operator short3(byte3 v)
		=> new short3(v.X, v.Y, v.Z);

	public override int GetHashCode()
		=> HashCode.Combine(X, Y, Z);

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is short3 other) return Equals(other);
		else return false;
	}

	public bool Equals(short3 other)
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
