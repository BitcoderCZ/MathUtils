using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using MathUtils.Generators;
using MathUtils.Utils;

namespace MathUtils.Vectors;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Same name case as int.")]
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Same name case as int.")]
[Vector(2, "int")]
public partial struct int2
{
	// vec 2
	public int2 XX => new int2(X, X);
	public int2 XY => new int2(X, Y);
	public int2 YX => new int2(Y, X);
	public int2 YY => new int2(Y, Y);

	// vec 3
	public int3 XXX => new int3(X, X, X);
	public int3 XXY => new int3(X, X, Y);
	public int3 XYX => new int3(X, Y, X);
	public int3 XYY => new int3(X, Y, Y);
	public int3 YXX => new int3(Y, X, X);
	public int3 YXY => new int3(Y, X, Y);
	public int3 YYX => new int3(Y, Y, X);
	public int3 YYY => new int3(Y, Y, Y);

	public static explicit operator int2(float2 v)
		=> new int2((int)v.X, (int)v.Y);

	public static implicit operator int2(short2 v)
		=> new int2(v.X, v.Y);

	public static implicit operator int2(ushort2 v)
		=> new int2(v.X, v.Y);

	public static implicit operator int2(byte2 v)
		=> new int2(v.X, v.Y);

	public static int2 Min(int2 a, int2 b)
		=> new int2(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));

	public static int2 Max(int2 a, int2 b)
		=> new int2(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));

	public static double Distance(int2 a, int2 b)
		=> (a - b).Length;

	public static int Dot(int2 a, int2 b)
		=> (a.X * b.X) + (a.Y * b.Y);

	/// <inheritdoc/>
	public override int GetHashCode()
		=> HashCode.Combine(X, Y);

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] object? obj)
		=> obj is int2 other && this == other;

	/// <inheritdoc/>
	public bool Equals(int2 other)
		=> this == other;

	/// <inheritdoc/>
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
