using System;
using System.Collections.Generic;
using System.Text;

namespace MathUtils.Generators;

internal readonly record struct VectorToGenerate
{
	public readonly string Name;
	public readonly int NumbDimensions;
	public readonly string ElementType;

	public VectorToGenerate(string name, int numbDimensions, string elementType)
	{
		Name = name;
		NumbDimensions = numbDimensions;
		ElementType = elementType;
	}
}
