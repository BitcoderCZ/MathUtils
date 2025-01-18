using System.Collections.Immutable;
using System.Diagnostics;

namespace MathUtils.Generators;

internal readonly struct VectorToGenerate
{
	public static readonly ImmutableArray<string> VectorAxisNames = ["X", "Y", "Z", "W"];
	public static readonly ImmutableArray<string> VectorAxisParamNames = ["x", "y", "z", "w"];

	public static readonly ImmutableArray<string> SizeAxisNames = ["Width", "Height"];
	public static readonly ImmutableArray<string> SizeAxisParamNames = ["width", "height"];

	public readonly int NumbDimensions;
	public readonly string ElementType;

	public readonly string Name;

	public readonly VectorType Type;

	public readonly ImmutableArray<string> AxisNames;
	public readonly ImmutableArray<string> AxisParamNames;

	public readonly bool IsFloatingPoint;
	public readonly bool IsSigned;
	public readonly bool Is64Bit;

	public VectorToGenerate(string name, string elementType, int numbDimensions, VectorType type)
	{
		Debug.Assert(numbDimensions >= 1);

		Name = name;
		NumbDimensions = numbDimensions;
		ElementType = elementType;

		Type = type;

		AxisNames = type switch
		{
			VectorType.Vector => VectorAxisNames,
			VectorType.Size => SizeAxisNames,
		};
		AxisParamNames = type switch
		{
			VectorType.Vector => VectorAxisParamNames,
			VectorType.Size => SizeAxisParamNames,
		};

		Debug.Assert(NumbDimensions <= AxisNames.Length);
		Debug.Assert(AxisNames.Length == AxisParamNames.Length);

		IsFloatingPoint = ElementType is "float" or "double" or "Half";
		IsSigned = ElementType[0] != 'u' && ElementType != "byte";
		Is64Bit = ElementType is "long" or "ulong" or "double";
	}

	public enum VectorType : byte
	{
		Vector,
		Size
	}
}
