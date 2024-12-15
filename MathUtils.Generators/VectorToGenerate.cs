using System.Collections.Immutable;

namespace MathUtils.Generators;

internal readonly struct VectorToGenerate
{
	private static readonly ImmutableArray<string> VectorAxisNames = ["X", "Y", "Z", "W"];
	private static readonly ImmutableArray<string> VectorAxisParamNames = ["x", "y", "z", "w"];

	private static readonly ImmutableArray<string> SizeAxisNames = ["Width", "Height"];
	private static readonly ImmutableArray<string> SizeAxisParamNames = ["width", "Height"];

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
