using MathUtils.Generators.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text;

namespace MathUtils.Generators;

[Generator]
public class VectorGenerator : IIncrementalGenerator
{
	private static readonly ImmutableArray<string> AxisNames = ["X", "Y", "Z", "W"];
	private static readonly ImmutableArray<string> AxisParamNames = ["x", "y", "z", "w"];

	private static readonly ImmutableArray<string> VectorTypes = ["byte", "sbyte", "short", "ushort", "int", "uint", "long", "ulong", "float", "double"];
	private static readonly ImmutableArray<string> GroupTypes = ["byte", "sbyte", "short", "ushort", "int", "uint", "long", "ulong"];

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0022:Use expression body for method", Justification = "No")]
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		context.RegisterPostInitializationOutput(ctx =>
		{
			for (int i = 2; i <= 3; i++)
			{
				foreach (string elementType in VectorTypes)
				{
					VectorToGenerate value = new VectorToGenerate(elementType + i, i, elementType);

					string result = GenerateVectorClass(value);
					ctx.AddSource($"{value.Name}.g.cs", SourceText.From(result, Encoding.UTF8));
				}
			}
		});
	}

	private static string GenerateVectorClass(VectorToGenerate vectorToGenerate)
	{
		// TODO: add xml documentation
		IndentedStringBuilder builder = new IndentedStringBuilder(new ValueStringBuilder(1024 * 4));

		int indexOfElementType = VectorTypes.IndexOf(vectorToGenerate.ElementType);
		bool isGroupType = GroupTypes.Contains(vectorToGenerate.ElementType);
		bool isLesserThanInt = indexOfElementType < VectorTypes.IndexOf("int");

		// ********** struct start **********
		// ********** Zero **********
		builder.AppendLine($$"""
			using System;
			using System.Collections;
			using System.Collections.Generic;
			using System.Globalization;
			using System.Runtime.CompilerServices;
			using System.Text;
			using MathUtils.Utils;

			#nullable enable
			namespace MathUtils.Vectors
			{
				public struct {{vectorToGenerate.Name}} : IEnumerable<{{vectorToGenerate.ElementType}}>, IEquatable<{{vectorToGenerate.Name}}>
				{
					public static readonly {{vectorToGenerate.Name}} Zero = default;

			""");

		builder.Indent = 2;

		// ********** One **********
		builder.Append($"public static readonly {vectorToGenerate.Name} One = new {vectorToGenerate.Name}(");

		for (int i = 0; i < vectorToGenerate.NumbDimensions; i++)
		{
			if (i != 0)
			{
				builder.Append(", ");
			}

			builder.Append('1');
		}

		builder.AppendLine(");");
		builder.AppendLine();

		// ********** Unit{x} **********
		for (int i = 0; i < vectorToGenerate.NumbDimensions; i++)
		{
			builder.Append($"public static readonly {vectorToGenerate.Name} Unit{AxisNames[i]} = new {vectorToGenerate.Name}(");

			for (int j = 0; j < vectorToGenerate.NumbDimensions; j++)
			{
				if (j != 0)
				{
					builder.Append(", ");
				}

				builder.Append(j == i ? '1' : '0');
			}

			builder.AppendLine(");");
			builder.AppendLine();
		}

		// ********** X, Y, ... **********
		for (int i = 0; i < vectorToGenerate.NumbDimensions; i++)
		{
			builder.AppendLine($"public {vectorToGenerate.ElementType} {AxisNames[i]};");
			builder.AppendLine();
		}

		// ********** 1 element ctor **********
		builder.AppendLine($$"""
			public {{vectorToGenerate.Name}}({{vectorToGenerate.ElementType}} value)
			{
			""");

		builder.Indent++;
		for (int i = 0; i < vectorToGenerate.NumbDimensions; i++)
		{
			builder.AppendLine($"{AxisNames[i]} = value;");
		}

		builder.Indent--;

		// ********** x element ctor **********
		builder.Append($$"""
			}

			public {{vectorToGenerate.Name}}(
			""");

		for (int i = 0; i < vectorToGenerate.NumbDimensions; i++)
		{
			if (i != 0)
			{
				builder.Append(", ");
			}

			builder.Append($"{vectorToGenerate.ElementType} {AxisParamNames[i]}");
		}

		builder.AppendLine("""
			)
			{
			""");

		builder.Indent++;
		for (int i = 0; i < vectorToGenerate.NumbDimensions; i++)
		{
			builder.AppendLine($"{AxisNames[i]} = {AxisParamNames[i]};");
		}

		builder.Indent--;

		builder.AppendLine("""
			}

			""");

		// ********** x int element ctor **********
		if (isLesserThanInt)
		{
			builder.Append($$"""
			public {{vectorToGenerate.Name}}(
			""");

			for (int i = 0; i < vectorToGenerate.NumbDimensions; i++)
			{
				if (i != 0)
				{
					builder.Append(", ");
				}

				builder.Append($"int {AxisParamNames[i]}");
			}

			builder.AppendLine("""
			)
			{
			""");

			builder.Indent++;
			for (int i = 0; i < vectorToGenerate.NumbDimensions; i++)
			{
				builder.AppendLine($"{AxisNames[i]} = ({vectorToGenerate.ElementType}){AxisParamNames[i]};");
			}

			builder.Indent--;

			builder.AppendLine("""
			}

			""");
		}

		// ********** vec2 combinations **********
		for (int i = 0; i < vectorToGenerate.NumbDimensions; i++)
		{
			for (int j = 0; j < vectorToGenerate.NumbDimensions; j++)
			{
				builder.AppendLine($"public {vectorToGenerate.ElementType}2 {AxisNames[i]}{AxisNames[j]} => new {vectorToGenerate.ElementType}2({AxisNames[i]}, {AxisNames[j]});");
			}
		}

		builder.AppendLine();

		// ********** vec3 combinations **********
		for (int i = 0; i < vectorToGenerate.NumbDimensions; i++)
		{
			for (int j = 0; j < vectorToGenerate.NumbDimensions; j++)
			{
				for (int k = 0; k < vectorToGenerate.NumbDimensions; k++)
				{
					builder.AppendLine($"public {vectorToGenerate.ElementType}3 {AxisNames[i]}{AxisNames[j]}{AxisNames[k]} => new {vectorToGenerate.ElementType}3({AxisNames[i]}, {AxisNames[j]}, {AxisNames[k]});");
				}
			}
		}

		builder.AppendLine();

		// ********** length **********
		builder.Append($"public {(isLesserThanInt ? "int" : vectorToGenerate.ElementType)} LengthSquared => ");
		for (int i = 0; i < vectorToGenerate.NumbDimensions; i++)
		{
			if (i != 0)
			{
				builder.Append(" + ");
			}

			builder.Append($"{AxisNames[i]} * {AxisNames[i]}");
		}

		builder.AppendLine("""
			;
			
			public double Length => Math.Sqrt(LengthSquared);

			""");

		// ********** indexer **********
		builder.AppendLine($$"""
			/// <exception cref="IndexOutOfRangeException"></exception>
			public {{vectorToGenerate.ElementType}} this[int index]
			{
				get
				{
					switch (index)
					{
			""");

		builder.Indent += 3;
		for (int i = 0; i < vectorToGenerate.NumbDimensions; i++)
		{
			builder.AppendLine($"""
				case {i}:
					return {AxisNames[i]};
				""");
		}

		builder.Indent -= 2;

		builder.AppendLine("""
					default:
						throw new IndexOutOfRangeException();
				}
			}
			set
			{
				switch (index)
				{
			""");

		builder.Indent += 2;

		for (int i = 0; i < vectorToGenerate.NumbDimensions; i++)
		{
			builder.AppendLine($"""
				case {i}:
					{AxisNames[i]} = value;
					break;
				""");
		}

		builder.Indent -= 3;

		builder.AppendLine("""
						default:
							throw new IndexOutOfRangeException();
					}
				}
			}

			""");

		// ********** operators **********
		AppendBinaryOperator(ref builder, "+");
		AppendBinaryElementOperator(ref builder, "+");
		AppendBinaryOperator(ref builder, "-");
		AppendBinaryElementOperator(ref builder, "-");
		AppendBinaryOperator(ref builder, "*");
		AppendBinaryElementOperator(ref builder, "*");
		AppendBinaryOperator(ref builder, "/");
		AppendBinaryElementOperator(ref builder, "/");
		AppendBinaryOperator(ref builder, "%");
		AppendBinaryElementOperator(ref builder, "%");

		AppendUnaryOperator(ref builder, "+");
		if (vectorToGenerate.IsSigned)
		{
			AppendUnaryOperator(ref builder, "-");
		}

		// ********** equals operators **********
		AppendCombinedBinaryOperator(ref builder, "==", "&&", "bool");
		AppendCombinedBinaryOperator(ref builder, "!=", "||", "bool");

		// ********** conversions **********
		for (int i = 0; i < VectorTypes.Length; i++)
		{
			if (i == indexOfElementType)
			{
				continue;
			}

			bool isExplicit = i > indexOfElementType || (isGroupType && (indexOfElementType % 2 == 0 ? (i == indexOfElementType + 1) : (i == indexOfElementType - 1)));

			string otherType = VectorTypes[i];

			builder.Append($"""
				public static {(isExplicit ? "explicit" : "implicit")} operator {vectorToGenerate.Name}({otherType}{vectorToGenerate.NumbDimensions} v)
					=> new {vectorToGenerate.Name}(
				""");

			for (int j = 0; j < vectorToGenerate.NumbDimensions; j++)
			{
				if (j != 0)
				{
					builder.Append(", ");
				}

				builder.Append($"({vectorToGenerate.ElementType})v.{AxisNames[j]}");
			}

			builder.AppendLine("""
				);

				""");
		}

		// ********** 2D index functions **********
		if (vectorToGenerate.NumbDimensions == 2 && !vectorToGenerate.IsFloatingPoint)
		{
			string returnType = (!isLesserThanInt && !vectorToGenerate.IsSigned) ? vectorToGenerate.ElementType : vectorToGenerate.Is64Bit ? "long" : "int";
			
			builder.AppendLine($$"""
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public static {{vectorToGenerate.Name}} FromIndex({{vectorToGenerate.ElementType}} index, {{vectorToGenerate.ElementType}} width)
					=> new {{vectorToGenerate.Name}}(index % width, index / width);
				
				/// <exception cref="ArgumentOutOfRangeException"></exception>
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public static {{vectorToGenerate.Name}} FromIndexChecked({{vectorToGenerate.ElementType}} index, {{vectorToGenerate.ElementType}} width, {{vectorToGenerate.ElementType}} height)
				{
					{{vectorToGenerate.Name}} value = new {{vectorToGenerate.Name}}(index % width, index / width);
					return value.InBounds(width, height)
						? value
						: throw new ArgumentOutOfRangeException("index");
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public {{returnType}} ToIndex({{vectorToGenerate.ElementType}} width)
					=> X + Y * width;
				
				/// <exception cref="IndexOutOfRangeException"></exception>
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public {{returnType}} ToIndexChecked({{vectorToGenerate.ElementType}} width, {{vectorToGenerate.ElementType}} height)
					=> InBounds(width, height)
						? X + Y * width
						: throw new IndexOutOfRangeException();
				
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public bool InBounds({{vectorToGenerate.ElementType}} width, {{vectorToGenerate.ElementType}} height)
					=> X >= 0 && X < width && Y >= 0 && Y < height;
				
				""");
		}

		// ********** GetEnumerator **********
		builder.Append($"""
			public IEnumerator<{vectorToGenerate.ElementType}> GetEnumerator()
				=> new ArrayEnumerator<{vectorToGenerate.ElementType}>
			""");
		AppendCallWithValues(ref builder);
		builder.AppendLine("""
			;

			""");

		builder.Append($"""
			IEnumerator IEnumerable.GetEnumerator()
				=> new ArrayEnumerator<{vectorToGenerate.ElementType}>
			""");
		AppendCallWithValues(ref builder);
		builder.AppendLine("""
			;

			""");

		// ********** min/max **********
		builder.Append($"""
			public static {vectorToGenerate.Name} Min({vectorToGenerate.Name} a, {vectorToGenerate.Name} b)
				=> new {vectorToGenerate.Name}(
			""");

		for (int i = 0; i < vectorToGenerate.NumbDimensions; i++)
		{
			if (i != 0)
			{
				builder.Append(", ");
			}

			builder.Append("Math.Min");
			AppendCallWithAB(ref builder, i);
		}

		builder.Append($"""
			);

			public static {vectorToGenerate.Name} Max({vectorToGenerate.Name} a, {vectorToGenerate.Name} b)
				=> new {vectorToGenerate.Name}(
			""");

		for (int i = 0; i < vectorToGenerate.NumbDimensions; i++)
		{
			if (i != 0)
			{
				builder.Append(", ");
			}

			builder.Append("Math.Max");
			AppendCallWithAB(ref builder, i);
		}

		builder.AppendLine("""
			);

			""");

		// ********** Distance/Dot **********
		builder.Append($"""
			public static double Distance({vectorToGenerate.Name} a, {vectorToGenerate.Name} b)
				=> (a - b).Length;
			
			public static {(isLesserThanInt ? "int" : vectorToGenerate.ElementType)} Dot({vectorToGenerate.Name} a, {vectorToGenerate.Name} b)
				=> 
			""");

		for (int i = 0; i < vectorToGenerate.NumbDimensions; i++)
		{
			if (i != 0)
			{
				builder.Append(" + ");
			}

			builder.Append($"(a.{AxisNames[i]} * b.{AxisNames[i]})");
		}

		builder.AppendLine("""
			;

			""");

		// ********** Normalized **********
		if (vectorToGenerate.IsFloatingPoint)
		{
			builder.AppendLine($"""
				public {vectorToGenerate.Name} Normalized()
					=> this / ({vectorToGenerate.ElementType})Length;
				
				""");
		}

		// ********** Cross **********
		if (vectorToGenerate.NumbDimensions == 3)
		{
			builder.AppendLine($"""
				public static {vectorToGenerate.Name} Cross({vectorToGenerate.Name} a, {vectorToGenerate.Name} b)
					=> new {vectorToGenerate.Name}(
						a.Y * b.Z - a.Z * b.Y,
						a.Z * b.X - a.X * b.Z,
						a.X * b.Y - a.Y * b.X
					);
				
				""");
		}

		// ********** GetHashCode **********
		builder.Append("""
			public override int GetHashCode()
				=> HashCode.Combine
			""");
		AppendCallWithValues(ref builder);
		builder.AppendLine("""
			;

			""");

		// ********** Equals **********
		builder.AppendLine($"""
			public override bool Equals(object? obj)
				=> obj is {vectorToGenerate.Name} other && this == other;
			
			public bool Equals({vectorToGenerate.Name} other)
				=> this == other;
			
			""");

		// ********** ToString **********
		builder.AppendLine("""
			public override string ToString()
				=> ToString("G", CultureInfo.InvariantCulture);
			
			public string ToString(string format)
				=> ToString(format, CultureInfo.InvariantCulture);
			
			public string ToString(string format, IFormatProvider formatProvider)
			{
				StringBuilder sb = new StringBuilder();
				string separator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;
				sb.Append('<');
			""");

		builder.Indent++;
		for (int i = 0; i < vectorToGenerate.NumbDimensions; i++)
		{
			if (i != 0)
			{
				builder.AppendLine("""
					sb.Append(separator);
					sb.Append(' ');
					""");
			}

			builder.AppendLine($"sb.Append({AxisNames[i]}.ToString(format, formatProvider));");
		}

		builder.Indent--;

		builder.AppendLine("""
				sb.Append('>');
				return sb.ToString();
			}
			""");

		// ********** struct end **********
		builder.Indent -= 2;
		builder.AppendLine("""
				}
			}
			#nullable restore
			""");

		return builder.ToString(); // also disposes

		void AppendUnaryOperator(ref IndentedStringBuilder builder, string opSymbol)
		{
			builder.Append($"""
				public static {vectorToGenerate.Name} operator {opSymbol}({vectorToGenerate.Name} value)
					=> new {vectorToGenerate.Name}(
				""");

			for (int i = 0; i < vectorToGenerate.NumbDimensions; i++)
			{
				if (i != 0)
				{
					builder.Append(", ");
				}

				builder.Append($"{opSymbol}value.{AxisNames[i]}");
			}

			builder.AppendLine("""
				);

				""");
		}

		void AppendBinaryOperator(ref IndentedStringBuilder builder, string opSymbol)
		{
			builder.Append($"""
				public static {vectorToGenerate.Name} operator {opSymbol}({vectorToGenerate.Name} a, {vectorToGenerate.Name} b)
					=> new {vectorToGenerate.Name}(
				""");

			for (int i = 0; i < vectorToGenerate.NumbDimensions; i++)
			{
				if (i != 0)
				{
					builder.Append(", ");
				}

				builder.Append($"a.{AxisNames[i]} {opSymbol} b.{AxisNames[i]}");
			}

			builder.AppendLine("""
				);

				""");
		}

		void AppendBinaryElementOperator(ref IndentedStringBuilder builder, string opSymbol)
		{
			builder.Append($"""
				public static {vectorToGenerate.Name} operator {opSymbol}({vectorToGenerate.Name} a, {vectorToGenerate.ElementType} b)
					=> new {vectorToGenerate.Name}(
				""");

			for (int i = 0; i < vectorToGenerate.NumbDimensions; i++)
			{
				if (i != 0)
				{
					builder.Append(", ");
				}

				builder.Append($"a.{AxisNames[i]} {opSymbol} b");
			}

			builder.AppendLine("""
				);

				""");
		}

		void AppendCombinedBinaryOperator(ref IndentedStringBuilder builder, string opSymbol, string combinationSymbol, string resultType)
		{
			builder.Append($"""
				public static {resultType} operator {opSymbol}({vectorToGenerate.Name} a, {vectorToGenerate.Name} b)
					=> 
				""");

			for (int i = 0; i < vectorToGenerate.NumbDimensions; i++)
			{
				if (i != 0)
				{
					builder.Append($" {combinationSymbol} ");
				}

				builder.Append($"a.{AxisNames[i]} {opSymbol} b.{AxisNames[i]}");
			}

			builder.AppendLine("""
				;

				""");
		}

		// (X, Y)
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void AppendCallWithValues(ref IndentedStringBuilder builder)
		{
			builder.Append('(');

			for (int i = 0; i < vectorToGenerate.NumbDimensions; i++)
			{
				if (i != 0)
				{
					builder.Append(", ");
				}

				builder.Append(AxisNames[i]);
			}

			builder.Append(')');
		}

		// (a.X, b.X)
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void AppendCallWithAB(ref IndentedStringBuilder builder, int axisIndex)
		{
			builder.Append($"(a.{AxisNames[axisIndex]}, b.{AxisNames[axisIndex]})");
		}
	}
}
