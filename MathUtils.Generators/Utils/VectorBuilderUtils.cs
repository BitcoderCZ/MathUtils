using System.Runtime.CompilerServices;

namespace MathUtils.Generators.Utils;

internal static class VectorBuilderUtils
{
	public static void AppendUnaryOperator(this ref IndentedStringBuilder builder, VectorToGenerate vec, string opSymbol)
	{
		builder.Append($"""
				public static {vec.Name} operator {opSymbol}({vec.Name} value)
					=> new {vec.Name}(
				""");

		for (int i = 0; i < vec.NumbDimensions; i++)
		{
			if (i != 0)
			{
				builder.Append(", ");
			}

			builder.Append($"{opSymbol}value.{vec.AxisNames[i]}");
		}

		builder.AppendLine("""
				);

				""");
	}

	public static void AppendBinaryOperator(this ref IndentedStringBuilder builder, VectorToGenerate vec, string opSymbol)
	{
		builder.Append($"""
				public static {vec.Name} operator {opSymbol}({vec.Name} a, {vec.Name} b)
					=> new {vec.Name}(
				""");

		for (int i = 0; i < vec.NumbDimensions; i++)
		{
			if (i != 0)
			{
				builder.Append(", ");
			}

			builder.Append($"a.{vec.AxisNames[i]} {opSymbol} b.{vec.AxisNames[i]}");
		}

		builder.AppendLine("""
				);

				""");
	}

	public static void AppendBinaryElementOperator(this ref IndentedStringBuilder builder, VectorToGenerate vec, string opSymbol)
	{
		builder.Append($"""
				public static {vec.Name} operator {opSymbol}({vec.Name} a, {vec.ElementType} b)
					=> new {vec.Name}(
				""");

		for (int i = 0; i < vec.NumbDimensions; i++)
		{
			if (i != 0)
			{
				builder.Append(", ");
			}

			builder.Append($"a.{vec.AxisNames[i]} {opSymbol} b");
		}

		builder.AppendLine("""
				);

				""");
	}

	public static void AppendCombinedBinaryOperator(this ref IndentedStringBuilder builder, VectorToGenerate vec, string opSymbol, string combinationSymbol, string resultType)
	{
		builder.Append($"""
				public static {resultType} operator {opSymbol}({vec.Name} a, {vec.Name} b)
					=> 
				""");

		for (int i = 0; i < vec.NumbDimensions; i++)
		{
			if (i != 0)
			{
				builder.Append($" {combinationSymbol} ");
			}

			builder.Append($"a.{vec.AxisNames[i]} {opSymbol} b.{vec.AxisNames[i]}");
		}

		builder.AppendLine("""
				;

				""");
	}

	// (X, Y)
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AppendCallWithValues(this ref IndentedStringBuilder builder, VectorToGenerate vec)
	{
		builder.Append('(');

		for (int i = 0; i < vec.NumbDimensions; i++)
		{
			if (i != 0)
			{
				builder.Append(", ");
			}

			builder.Append(vec.AxisNames[i]);
		}

		builder.Append(')');
	}

	// (a.X, b.X)
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AppendCallWithAB(this ref IndentedStringBuilder builder, VectorToGenerate vec, int axisIndex)
		=> builder.Append($"(a.{vec.AxisNames[axisIndex]}, b.{vec.AxisNames[axisIndex]})");
}
