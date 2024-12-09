using MathUtils.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace MathUtils.Vectors;

public struct float2 : IEnumerable<float>
{
	// vec 1
	public float X;
	public float Y;

	// vec 2
	public float2 XX => new float2(X, X);
	public float2 XY => new float2(X, Y);
	public float2 YX => new float2(Y, X);
	public float2 YY => new float2(Y, Y);

	// vec 3
	public float3 XXX => new float3(X, X, X);
	public float3 XXY => new float3(X, X, Y);
	public float3 XYX => new float3(X, Y, X);
	public float3 XYY => new float3(X, Y, Y);
	public float3 YXX => new float3(Y, X, X);
	public float3 YXY => new float3(Y, X, Y);
	public float3 YYX => new float3(Y, Y, X);
	public float3 YYY => new float3(Y, Y, Y);

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

	public static readonly float2 Zero = new float2(0f, 0f);

	public static readonly float2 One = new float2(1f, 1f);

	public static readonly float2 UnitX = new float2(1f, 0f);
	public static readonly float2 UnitY = new float2(0f, 1f);

	public float2(float _x, float _y)
	{
		X = _x;
		Y = _y;
	}

	public float2 Normalized()
		=> this / (float)Length;

	public IEnumerator<float> GetEnumerator()
		=> new ArrayEnumerator<float>(X, Y);

	IEnumerator IEnumerable.GetEnumerator()
		=> new ArrayEnumerator<float>(X, Y);

	public static float2 Min(float2 a, float2 b)
		=> new float2(MathF.Min(a.X, b.X), MathF.Min(a.Y, b.Y));
	public static float2 Max(float2 a, float2 b)
		=> new float2(MathF.Max(a.X, b.X), MathF.Max(a.Y, b.Y));

	public static double Distance(float2 a, float2 b)
		=> (a - b).Length;

	public static float Dot(float2 a, float2 b)
		=> a.X * b.X + a.Y * b.Y;

	public static float2 operator +(float2 a, float2 b)
		=> new float2(a.X + b.X, a.Y + b.Y);
	public static float2 operator -(float2 a, float2 b)
		=> new float2(a.X - b.X, a.Y - b.Y);
	public static float2 operator *(float2 a, float2 b)
		=> new float2(a.X * b.X, a.Y * b.Y);
	public static float2 operator /(float2 a, float2 b)
		=> new float2(a.X / b.X, a.Y / b.Y);
	public static float2 operator %(float2 a, float2 b)
		=> new float2(a.X % b.X, a.Y % b.Y);

	public static float2 operator -(float2 a)
		=> new float2(-a.X, -a.Y);

	public static float2 operator *(float2 a, float b)
		=> new float2(a.X * b, a.Y * b);
	public static float2 operator /(float2 a, float b)
		=> new float2(a.X / b, a.Y / b);
	public static float2 operator %(float2 a, float b)
		=> new float2(a.X % b, a.Y % b);

	public static bool operator ==(float2 a, float2 b)
		=> a.Equals(b);
	public static bool operator !=(float2 a, float2 b)
		=> !a.Equals(b);

	public static implicit operator float2(int2 v)
		=> new float2(v.X, v.Y);
	public static implicit operator float2(short2 v)
		=> new float2(v.X, v.Y);
	public static implicit operator float2(ushort2 v)
		=> new float2(v.X, v.Y);
	public static implicit operator float2(byte2 v)
		=> new float2(v.X, v.Y);

	public override int GetHashCode()
		=> HashCode.Combine(X, Y);

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is int2 other) return Equals(other);
		else return false;
	}

	public bool Equals(int2 other)
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
