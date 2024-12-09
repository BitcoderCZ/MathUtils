using MathUtils.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace MathUtils.Vectors;

public struct byte2 : IEnumerable<byte>
{
	// vec 1
	public byte X;
	public byte Y;

	// vec 2
	public byte2 XX => new byte2(X, X);
	public byte2 XY => new byte2(X, Y);
	public byte2 YX => new byte2(Y, X);
	public byte2 YY => new byte2(Y, Y);

	// vec 3
	public byte3 XXX => new byte3(X, X, X);
	public byte3 XXY => new byte3(X, X, Y);
	public byte3 XYX => new byte3(X, Y, X);
	public byte3 XYY => new byte3(X, Y, Y);
	public byte3 YXX => new byte3(Y, X, X);
	public byte3 YXY => new byte3(Y, X, Y);
	public byte3 YYX => new byte3(Y, Y, X);
	public byte3 YYY => new byte3(Y, Y, Y);

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

	public static readonly byte2 Zero = default;

	public static readonly byte2 One = new byte2(1, 1);

	public static readonly byte2 UnitX = new byte2(1, 0);
	public static readonly byte2 UnitY = new byte2(0, 1);

	public byte2(int _x, int _y)
		: this((byte)_x, (byte)_y)
	{
	}
	public byte2(byte _x, byte _y)
	{
		X = _x;
		Y = _y;
	}

	public IEnumerator<byte> GetEnumerator()
		=> new ArrayEnumerator<byte>(X, Y);

	IEnumerator IEnumerable.GetEnumerator()
		=> new ArrayEnumerator<byte>(X, Y);

	public static byte2 Min(byte2 a, byte2 b)
		=> new byte2(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
	public static byte2 Max(byte2 a, byte2 b)
		=> new byte2(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));

	public static double Distance(byte2 a, byte2 b)
		=> (a - b).Length;

	public static int Dot(byte2 a, byte2 b)
		=> a.X * b.X + a.Y * b.Y;

	public static byte2 operator +(byte2 a, byte2 b)
		=> new byte2(a.X + b.X, a.Y + b.Y);
	public static byte2 operator -(byte2 a, byte2 b)
		=> new byte2(a.X - b.X, a.Y - b.Y);
	public static byte2 operator *(byte2 a, byte2 b)
		=> new byte2(a.X * b.X, a.Y * b.Y);
	public static byte2 operator /(byte2 a, byte2 b)
		=> new byte2(a.X / b.X, a.Y / b.Y);
	public static byte2 operator %(byte2 a, byte2 b)
		=> new byte2(a.X % b.X, a.Y % b.Y);

	public static byte2 operator *(byte2 a, byte b)
		=> new byte2(a.X * b, a.Y * b);
	public static byte2 operator /(byte2 a, byte b)
		=> new byte2(a.X / b, a.Y / b);
	public static byte2 operator %(byte2 a, byte b)
		=> new byte2(a.X % b, a.Y % b);

	public static bool operator ==(byte2 a, byte2 b)
		=> a.Equals(b);
	public static bool operator !=(byte2 a, byte2 b)
		=> !a.Equals(b);

	public static explicit operator byte2(float2 v)
		=> new byte2((byte)v.X, (byte)v.Y);
	public static explicit operator byte2(int2 v)
		=> new byte2((byte)v.X, (byte)v.Y);
	public static explicit operator byte2(short2 v)
		=> new byte2((byte)v.X, (byte)v.Y);
	public static explicit operator byte2(ushort2 v)
		=> new byte2((byte)v.X, (byte)v.Y);

	public override int GetHashCode()
		=> HashCode.Combine(X, Y);

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is byte2 other) return Equals(other);
		else return false;
	}

	public bool Equals(byte2 other)
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
