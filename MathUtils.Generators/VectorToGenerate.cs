namespace MathUtils.Generators;

internal readonly record struct VectorToGenerate
{
	public readonly string Name;
	public readonly int NumbDimensions;
	public readonly string ElementType;

	public readonly bool IsFloatingPoint;

	public VectorToGenerate(string name, int numbDimensions, string elementType)
	{
		Name = name;
		NumbDimensions = numbDimensions;
		ElementType = elementType;

		IsFloatingPoint = ElementType is "float" or "double" or "Half";
	}
}
