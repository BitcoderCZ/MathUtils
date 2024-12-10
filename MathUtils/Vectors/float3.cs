using MathUtils.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using MathUtils.Generators;

namespace MathUtils.Vectors;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Same name case as float.")]
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Same name case as int.")]
[Vector(3, "float")]
public partial struct float3
{
	public static implicit operator float3(int3 v)
		=> new float3(v.X, v.Y, v.Z);
	public static implicit operator float3(short3 v)
		=> new float3(v.X, v.Y, v.Z);
	public static implicit operator float3(ushort3 v)
		=> new float3(v.X, v.Y, v.Z);
	public static implicit operator float3(byte3 v)
		=> new float3(v.X, v.Y, v.Z);
}
