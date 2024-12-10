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
	public static explicit operator int2(float2 v)
		=> new int2((int)v.X, (int)v.Y);

	public static implicit operator int2(short2 v)
		=> new int2(v.X, v.Y);

	public static implicit operator int2(ushort2 v)
		=> new int2(v.X, v.Y);

	public static implicit operator int2(byte2 v)
		=> new int2(v.X, v.Y);
}
