using MathUtils.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace MathUtils.Vectors;

public struct float3 : IEnumerable<float>
{
	// vec 1
	public float X;
	public float Y;
	public float Z;

	// vec2
	public float2 XX => new float2(X, X);
	public float2 XY => new float2(X, Y);
	public float2 XZ => new float2(X, Z);
	public float2 YX => new float2(Y, X);
	public float2 YY => new float2(Y, Y);
	public float2 YZ => new float2(Y, Z);
	public float2 ZX => new float2(Z, X);
	public float2 ZY => new float2(Z, Y);
	public float2 ZZ => new float2(Z, Z);

	// vec 3
	public float3 XXX => new float3(X, X, X);
	public float3 XXY => new float3(X, X, Y);
	public float3 XXZ => new float3(X, X, Z);
	public float3 XYX => new float3(X, Y, X);
	public float3 XYY => new float3(X, Y, Y);
	public float3 XYZ => new float3(X, Y, Z);
	public float3 XZX => new float3(X, Z, X);
	public float3 XZY => new float3(X, Z, Y);
	public float3 XZZ => new float3(X, Z, Z);
	public float3 YXX => new float3(Y, X, X);
	public float3 YXY => new float3(Y, X, Y);
	public float3 YXZ => new float3(Y, X, Z);
	public float3 YYX => new float3(Y, Y, X);
	public float3 YYY => new float3(Y, Y, Y);
	public float3 YYZ => new float3(Y, Y, Z);
	public float3 YZX => new float3(Y, Z, X);
	public float3 YZY => new float3(Y, Z, Y);
	public float3 YZZ => new float3(Y, Z, Z);
	public float3 ZXX => new float3(Z, X, X);
	public float3 ZXY => new float3(Z, X, Y);
	public float3 ZXZ => new float3(Z, X, Z);
	public float3 ZYX => new float3(Z, Y, X);
	public float3 ZYY => new float3(Z, Y, Y);
	public float3 ZYZ => new float3(Z, Y, Z);
	public float3 ZZX => new float3(Z, Z, X);
	public float3 ZZY => new float3(Z, Z, Y);
	public float3 ZZZ => new float3(Z, Z, Z);

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

	public double LengthSquared => X * X + Y * Y + Z * Z;
	public double Length => Math.Sqrt(LengthSquared);

	public static readonly float3 Zero = new float3(0f, 0f, 0f);
	public static readonly float3 One = new float3(1f, 1f, 1f);

	public static readonly float3 UnitX = new float3(1f, 0f, 0f);
	public static readonly float3 UnitY = new float3(0f, 1f, 0f);
	public static readonly float3 UnitZ = new float3(0f, 0f, 1f);

	public float3(float _x, float _y, float _z)
	{
		X = _x;
		Y = _y;
		Z = _z;
	}

	public float3 Normalized()
		=> this / (float)Length;

	public IEnumerator<float> GetEnumerator()
		=> new ArrayEnumerator<float>(X, Y, Z);

	IEnumerator IEnumerable.GetEnumerator()
		=> new ArrayEnumerator<float>(X, Y, Z);

	public static float3 Min(float3 a, float3 b)
		=> new float3(MathF.Min(a.X, b.X), MathF.Min(a.Y, b.Y), MathF.Min(a.Z, b.Z));
	public static float3 Max(float3 a, float3 b)
		=> new float3(MathF.Max(a.X, b.X), MathF.Max(a.Y, b.Y), MathF.Max(a.Z, b.Z));

	public static double Distance(float3 a, float3 b)
		=> (a - b).Length;

	public static float Dot(float3 a, float3 b)
		=> a.X * b.X + a.Y * b.Y + a.Z;
	public static float3 Cross(float3 a, float3 b)
		=> new float3(
			a.Y * b.Z - a.Z * b.Y,
			a.Z * b.X - a.X * b.Z,
			a.X * b.Y - a.Y * b.X
		);

	public static float3 operator +(float3 a, float3 b)
		=> new float3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
	public static float3 operator -(float3 a, float3 b)
		=> new float3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
	public static float3 operator *(float3 a, float3 b)
		=> new float3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
	public static float3 operator /(float3 a, float3 b)
		=> new float3(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
	public static float3 operator %(float3 a, float3 b)
		=> new float3(a.X % b.X, a.Y % b.Y, a.Z % b.Z);

	public static float3 operator -(float3 a)
		=> new float3(-a.X, -a.Y, -a.Z);

	public static float3 operator *(float3 a, float b)
		=> new float3(a.X * b, a.Y * b, a.Z * b);
	public static float3 operator /(float3 a, float b)
		=> new float3(a.X / b, a.Y / b, a.Z / b);
	public static float3 operator %(float3 a, float b)
		=> new float3(a.X % b, a.Y % b, a.Z % b);

	public static bool operator ==(float3 a, float3 b)
		=> a.Equals(b);
	public static bool operator !=(float3 a, float3 b)
		=> !a.Equals(b);

	public static implicit operator float3(int3 v)
		=> new float3(v.X, v.Y, v.Z);
	public static implicit operator float3(short3 v)
		=> new float3(v.X, v.Y, v.Z);
	public static implicit operator float3(ushort3 v)
		=> new float3(v.X, v.Y, v.Z);
	public static implicit operator float3(byte3 v)
		=> new float3(v.X, v.Y, v.Z);

	public override int GetHashCode()
		=> HashCode.Combine(X, Y, Z);

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is float3 other) return Equals(other);
		else return false;
	}

	public bool Equals(float3 other)
		=> X == other.X && Y == other.Y && Z == other.Z;

	/// <summary>
	/// Returns a String representing this Vector3 instance.
	/// </summary>
	/// <returns>The string representation.</returns>
	public override string ToString()
		=> ToString("G", CultureInfo.InvariantCulture);

	/// <summary>
	/// Returns a String representing this Vector3 instance, using the specified format to format individual elements.
	/// </summary>
	/// <param name="format">The format of individual elements.</param>
	/// <returns>The string representation.</returns>
	public string ToString(string format)
		=> ToString(format, CultureInfo.InvariantCulture);

	/// <summary>
	/// Returns a String representing this Vector3 instance, using the specified format to format individual elements 
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
