using MathUtils.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace MathUtils.Vectors;

public struct ushort2 : IEnumerable<ushort>
{
	// vec 1
	public ushort X;
	public ushort Y;

	// vec 2
	public ushort2 XX => new ushort2(X, X);
	public ushort2 XY => new ushort2(X, Y);
	public ushort2 YX => new ushort2(Y, X);
	public ushort2 YY => new ushort2(Y, Y);

	// vec 3
	public ushort3 XXX => new ushort3(X, X, X);
	public ushort3 XXY => new ushort3(X, X, Y);
	public ushort3 XYX => new ushort3(X, Y, X);
	public ushort3 XYY => new ushort3(X, Y, Y);
	public ushort3 YXX => new ushort3(Y, X, X);
	public ushort3 YXY => new ushort3(Y, X, Y);
	public ushort3 YYX => new ushort3(Y, Y, X);
	public ushort3 YYY => new ushort3(Y, Y, Y);

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

	public static readonly ushort2 Zero = default;

	public static readonly ushort2 One = new ushort2(1, 1);

	public static readonly ushort2 UnitX = new ushort2(1, 0);
	public static readonly ushort2 UnitY = new ushort2(0, 1);

	public ushort2(int _x, int _y)
		: this((ushort)_x, (ushort)_y)
	{
	}
	public ushort2(ushort _x, ushort _y)
	{
		X = _x;
		Y = _y;
	}

	public IEnumerator<ushort> GetEnumerator()
		=> new ArrayEnumerator<ushort>(X, Y);

	IEnumerator IEnumerable.GetEnumerator()
		=> new ArrayEnumerator<ushort>(X, Y);

	public static ushort2 Min(ushort2 a, ushort2 b)
		=> new ushort2(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
	public static ushort2 Max(ushort2 a, ushort2 b)
		=> new ushort2(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));

	public static double Distance(ushort2 a, ushort2 b)
		=> (a - b).Length;

	public static int Dot(ushort2 a, ushort2 b)
		=> a.X * b.X + a.Y * b.Y;

	public static ushort2 operator +(ushort2 a, ushort2 b)
		=> new ushort2(a.X + b.X, a.Y + b.Y);
	public static ushort2 operator -(ushort2 a, ushort2 b)
		=> new ushort2(a.X - b.X, a.Y - b.Y);
	public static ushort2 operator *(ushort2 a, ushort2 b)
		=> new ushort2(a.X * b.X, a.Y * b.Y);
	public static ushort2 operator /(ushort2 a, ushort2 b)
		=> new ushort2(a.X / b.X, a.Y / b.Y);
	public static ushort2 operator %(ushort2 a, ushort2 b)
		=> new ushort2(a.X % b.X, a.Y % b.Y);

	public static ushort2 operator *(ushort2 a, ushort b)
		=> new ushort2(a.X * b, a.Y * b);
	public static ushort2 operator /(ushort2 a, ushort b)
		=> new ushort2(a.X / b, a.Y / b);
	public static ushort2 operator %(ushort2 a, ushort b)
		=> new ushort2(a.X % b, a.Y % b);

	public static bool operator ==(ushort2 a, ushort2 b)
		=> a.Equals(b);
	public static bool operator !=(ushort2 a, ushort2 b)
		=> !a.Equals(b);

	public static explicit operator ushort2(float2 v)
		=> new ushort2((ushort)v.X, (ushort)v.Y);
	public static explicit operator ushort2(int2 v)
		=> new ushort2((ushort)v.X, (ushort)v.Y);
	public static explicit operator ushort2(short2 v)
		=> new ushort2((ushort)v.X, (ushort)v.Y);
	public static implicit operator ushort2(byte2 v)
		=> new ushort2(v.X, v.Y);

	public override int GetHashCode()
		=> HashCode.Combine(X, Y);

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is ushort2 other) return Equals(other);
		else return false;
	}

	public bool Equals(ushort2 other)
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
