using MathUtils.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace MathUtils.Vectors;

public struct short2 : IEnumerable<short>
{
	// vec 1
	public short X;
	public short Y;

	// vec 2
	public short2 XX => new short2(X, X);
	public short2 XY => new short2(X, Y);
	public short2 YX => new short2(Y, X);
	public short2 YY => new short2(Y, Y);

	// vec 3
	public short3 XXX => new short3(X, X, X);
	public short3 XXY => new short3(X, X, Y);
	public short3 XYX => new short3(X, Y, X);
	public short3 XYY => new short3(X, Y, Y);
	public short3 YXX => new short3(Y, X, X);
	public short3 YXY => new short3(Y, X, Y);
	public short3 YYX => new short3(Y, Y, X);
	public short3 YYY => new short3(Y, Y, Y);

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

	public static readonly short2 Zero = default;

	public static readonly short2 One = new short2(1, 1);

	public static readonly short2 UnitX = new short2(1, 0);
	public static readonly short2 UnitY = new short2(0, 1);

	public short2(int _x, int _y)
		: this((short)_x, (short)_y)
	{
	}
	public short2(short _x, short _y)
	{
		X = _x;
		Y = _y;
	}

	public IEnumerator<short> GetEnumerator()
		=> new ArrayEnumerator<short>(X, Y);

	IEnumerator IEnumerable.GetEnumerator()
		=> new ArrayEnumerator<short>(X, Y);

	public static short2 Min(short2 a, short2 b)
		=> new short2(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
	public static short2 Max(short2 a, short2 b)
		=> new short2(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));

	public static double Distance(short2 a, short2 b)
		=> (a - b).Length;

	public static int Dot(short2 a, short2 b)
		=> a.X * b.X + a.Y * b.Y;

	public static short2 operator +(short2 a, short2 b)
		=> new short2(a.X + b.X, a.Y + b.Y);
	public static short2 operator -(short2 a, short2 b)
		=> new short2(a.X - b.X, a.Y - b.Y);
	public static short2 operator *(short2 a, short2 b)
		=> new short2(a.X * b.X, a.Y * b.Y);
	public static short2 operator /(short2 a, short2 b)
		=> new short2(a.X / b.X, a.Y / b.Y);
	public static short2 operator %(short2 a, short2 b)
		=> new short2(a.X % b.X, a.Y % b.Y);

	public static short2 operator -(short2 a)
		=> new short2(-a.X, -a.Y);

	public static short2 operator *(short2 a, short b)
		=> new short2(a.X * b, a.Y * b);
	public static short2 operator /(short2 a, short b)
		=> new short2(a.X / b, a.Y / b);
	public static short2 operator %(short2 a, short b)
		=> new short2(a.X % b, a.Y % b);

	public static bool operator ==(short2 a, short2 b)
		=> a.Equals(b);
	public static bool operator !=(short2 a, short2 b)
		=> !a.Equals(b);

	public static explicit operator short2(float2 v)
		=> new short2((short)v.X, (short)v.Y);
	public static explicit operator short2(int2 v)
		=> new short2((short)v.X, (short)v.Y);
	public static explicit operator short2(ushort2 v)
		=> new short2((short)v.X, (short)v.Y);
	public static implicit operator short2(byte2 v)
		=> new short2(v.X, v.Y);

	public override int GetHashCode()
		=> HashCode.Combine(X, Y);

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is short2 other) return Equals(other);
		else return false;
	}

	public bool Equals(short2 other)
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
