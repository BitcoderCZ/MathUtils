using MathUtils.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace MathUtils.Vectors;

public struct byte3 : IEnumerable<byte>
{
	// vec 1
	public byte X;
	public byte Y;
	public byte Z;

	// vec2
	public byte2 XX => new byte2(X, X);
	public byte2 XY => new byte2(X, Y);
	public byte2 XZ => new byte2(X, Z);
	public byte2 YX => new byte2(Y, X);
	public byte2 YY => new byte2(Y, Y);
	public byte2 YZ => new byte2(Y, Z);
	public byte2 ZX => new byte2(Z, X);
	public byte2 ZY => new byte2(Z, Y);
	public byte2 ZZ => new byte2(Z, Z);

	// vec 3
	public byte3 XXX => new byte3(X, X, X);
	public byte3 XXY => new byte3(X, X, Y);
	public byte3 XXZ => new byte3(X, X, Z);
	public byte3 XYX => new byte3(X, Y, X);
	public byte3 XYY => new byte3(X, Y, Y);
	public byte3 XYZ => new byte3(X, Y, Z);
	public byte3 XZX => new byte3(X, Z, X);
	public byte3 XZY => new byte3(X, Z, Y);
	public byte3 XZZ => new byte3(X, Z, Z);
	public byte3 YXX => new byte3(Y, X, X);
	public byte3 YXY => new byte3(Y, X, Y);
	public byte3 YXZ => new byte3(Y, X, Z);
	public byte3 YYX => new byte3(Y, Y, X);
	public byte3 YYY => new byte3(Y, Y, Y);
	public byte3 YYZ => new byte3(Y, Y, Z);
	public byte3 YZX => new byte3(Y, Z, X);
	public byte3 YZY => new byte3(Y, Z, Y);
	public byte3 YZZ => new byte3(Y, Z, Z);
	public byte3 ZXX => new byte3(Z, X, X);
	public byte3 ZXY => new byte3(Z, X, Y);
	public byte3 ZXZ => new byte3(Z, X, Z);
	public byte3 ZYX => new byte3(Z, Y, X);
	public byte3 ZYY => new byte3(Z, Y, Y);
	public byte3 ZYZ => new byte3(Z, Y, Z);
	public byte3 ZZX => new byte3(Z, Z, X);
	public byte3 ZZY => new byte3(Z, Z, Y);
	public byte3 ZZZ => new byte3(Z, Z, Z);

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

	public static readonly byte3 Zero = new byte3(0, 0, 0);
	public static readonly byte3 One = new byte3(1, 1, 1);

	public static readonly byte3 UnitX = new byte3(1, 0, 0);
	public static readonly byte3 UnitY = new byte3(0, 1, 0);
	public static readonly byte3 UnitZ = new byte3(0, 0, 1);

	public byte3(int _x, int _y, int _z)
		: this((byte)_x, (byte)_y, (byte)_z)
	{
	}
	public byte3(byte _x, byte _y, byte _z)
	{
		X = _x;
		Y = _y;
		Z = _z;
	}

	public IEnumerator<byte> GetEnumerator()
		=> new ArrayEnumerator<byte>(X, Y, Z);

	IEnumerator IEnumerable.GetEnumerator()
		=> new ArrayEnumerator<byte>(X, Y, Z);

	public static byte3 Min(byte3 a, byte3 b)
		=> new byte3(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
	public static byte3 Max(byte3 a, byte3 b)
		=> new byte3(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));

	public static double Distance(byte3 a, byte3 b)
		=> (a - b).Length;

	public static int Dot(byte3 a, byte3 b)
		=> a.X * b.X + a.Y * b.Y + a.Z;
	public static byte3 Cross(byte3 a, byte3 b)
		=> new byte3(
			a.Y * b.Z - a.Z * b.Y,
			a.Z * b.X - a.X * b.Z,
			a.X * b.Y - a.Y * b.X
		);

	public static byte3 operator +(byte3 a, byte3 b)
		=> new byte3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
	public static byte3 operator -(byte3 a, byte3 b)
		=> new byte3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
	public static byte3 operator *(byte3 a, byte3 b)
		=> new byte3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
	public static byte3 operator /(byte3 a, byte3 b)
		=> new byte3(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
	public static byte3 operator %(byte3 a, byte3 b)
		=> new byte3(a.X % b.X, a.Y % b.Y, a.Z % b.Z);

	public static byte3 operator *(byte3 a, int b)
		=> new byte3(a.X * b, a.Y * b, a.Z * b);
	public static byte3 operator /(byte3 a, int b)
		=> new byte3(a.X / b, a.Y / b, a.Z / b);
	public static byte3 operator %(byte3 a, int b)
		=> new byte3(a.X % b, a.Y % b, a.Z % b);

	public static bool operator ==(byte3 a, byte3 b)
		=> a.Equals(b);
	public static bool operator !=(byte3 a, byte3 b)
		=> !a.Equals(b);

	public static explicit operator byte3(float3 v)
		=> new byte3((byte)v.X, (byte)v.Y, (byte)v.Z);
	public static explicit operator byte3(int3 v)
		=> new byte3((byte)v.X, (byte)v.Y, (byte)v.Z);
	public static explicit operator byte3(short3 v)
		=> new byte3((byte)v.X, (byte)v.Y, (byte)v.Z);
	public static explicit operator byte3(ushort3 v)
		=> new byte3((byte)v.X, (byte)v.Y, (byte)v.Z);

	public override int GetHashCode()
		=> HashCode.Combine(X, Y, Z);

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is byte3 other) return Equals(other);
		else return false;
	}

	public bool Equals(byte3 other)
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
