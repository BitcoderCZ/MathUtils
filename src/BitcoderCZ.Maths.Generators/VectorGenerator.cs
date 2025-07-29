using BitcoderCZ.Maths.Generators.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;

namespace BitcoderCZ.Maths.Generators;

[Generator]
public class VectorGenerator : IIncrementalGenerator
{
	private static readonly ImmutableArray<string> VectorTypes = ["byte", "sbyte", "short", "ushort", "int", "uint", "long", "ulong", "float", "double"];
	private static readonly ImmutableArray<string> GroupTypes = ["byte", "sbyte", "short", "ushort", "int", "uint", "long", "ulong"];

	private static readonly ImmutableArray<(string, string)> SizeTypes = [(string.Empty, "int"), ("L", "long"), ("F", "float")];

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0022:Use expression body for method", Justification = "No")]
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		// TODO: Size (int), SizeL (long) SizeF (float)
		// consider using unsigned types, with signed ctors and ArgumentOutOfRangeException
		// Use GenerateVectorClass methos, add AxisNames and AxisParamNames as parameters (ROS?)
		// Add Type enum or IsSize bool to VectorToGenerate, only add some methods for each (don't want stuff like Dot and the combinations properties on Size)

		context.RegisterPostInitializationOutput(ctx =>
		{
			foreach (var (suffix, type) in SizeTypes)
			{
				VectorToGenerate value = new VectorToGenerate("Size" + suffix, type, 2, VectorToGenerate.VectorType.Size);
				string result = GenerateVectorClass(value);
				ctx.AddSource($"{value.Name}.g.cs", SourceText.From(result, Encoding.UTF8));
			}

			for (int i = 2; i <= 3; i++)
			{
				foreach (string elementType in VectorTypes)
				{
					VectorToGenerate value = new VectorToGenerate(elementType + i, elementType, i, VectorToGenerate.VectorType.Vector);

					string result = GenerateVectorClass(value);
					ctx.AddSource($"{value.Name}.g.cs", SourceText.From(result, Encoding.UTF8));
				}
			}
		});
	}

	private static string GenerateVectorClass(VectorToGenerate vec)
	{
		// TODO: add xml documentation
		IndentedStringBuilder builder = new IndentedStringBuilder(new ValueStringBuilder(1024 * 4));

		int indexOfElementType = VectorTypes.IndexOf(vec.ElementType);
		bool isGroupType = GroupTypes.Contains(vec.ElementType);
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
			using BitcoderCZ.Maths.Utils;

			#nullable enable
			namespace BitcoderCZ.Maths.Vectors
			{
				public struct {{vec.Name}} : IEnumerable<{{vec.ElementType}}>, IEquatable<{{vec.Name}}>
				{
					public static readonly {{vec.Name}} Zero = default;

			""");

		builder.Indent = 2;

		// ********** One **********
		builder.Append($"public static readonly {vec.Name} One = new {vec.Name}(");

		for (int i = 0; i < vec.NumbDimensions; i++)
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
		for (int i = 0; i < vec.NumbDimensions; i++)
		{
			builder.Append($"public static readonly {vec.Name} Unit{vec.AxisNames[i]} = new {vec.Name}(");

			for (int j = 0; j < vec.NumbDimensions; j++)
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
		for (int i = 0; i < vec.NumbDimensions; i++)
		{
			builder.AppendLine($"public {vec.ElementType} {vec.AxisNames[i]};");
			builder.AppendLine();
		}

		// ********** 1 element ctor **********
		builder.AppendLine($$"""
			public {{vec.Name}}({{vec.ElementType}} value)
			{
			""");

		builder.Indent++;
		for (int i = 0; i < vec.NumbDimensions; i++)
		{
			builder.AppendLine($"{vec.AxisNames[i]} = value;");
		}

		builder.Indent--;

		// ********** x element ctor **********
		builder.Append($$"""
			}

			public {{vec.Name}}(
			""");

		for (int i = 0; i < vec.NumbDimensions; i++)
		{
			if (i != 0)
			{
				builder.Append(", ");
			}

			builder.Append($"{vec.ElementType} {vec.AxisParamNames[i]}");
		}

		builder.AppendLine("""
			)
			{
			""");

		builder.Indent++;
		for (int i = 0; i < vec.NumbDimensions; i++)
		{
			builder.AppendLine($"{vec.AxisNames[i]} = {vec.AxisParamNames[i]};");
		}

		builder.Indent--;

		builder.AppendLine("""
			}

			""");

		// ********** x int element ctor **********
		if (isLesserThanInt)
		{
			builder.Append($$"""
			public {{vec.Name}}(
			""");

			for (int i = 0; i < vec.NumbDimensions; i++)
			{
				if (i != 0)
				{
					builder.Append(", ");
				}

				builder.Append($"int {vec.AxisParamNames[i]}");
			}

			builder.AppendLine("""
			)
			{
			""");

			builder.Indent++;
			for (int i = 0; i < vec.NumbDimensions; i++)
			{
				builder.AppendLine($"{vec.AxisNames[i]} = ({vec.ElementType}){vec.AxisParamNames[i]};");
			}

			builder.Indent--;

			builder.AppendLine("""
			}

			""");
		}

		// ********** vec2 combinations **********
		if (vec.Type == VectorToGenerate.VectorType.Vector)
		{
			for (int i = 0; i < vec.NumbDimensions; i++)
			{
				for (int j = 0; j < vec.NumbDimensions; j++)
				{
					builder.AppendLine($"public readonly {vec.ElementType}2 {vec.AxisNames[i]}{vec.AxisNames[j]} => new {vec.ElementType}2({vec.AxisNames[i]}, {vec.AxisNames[j]});");
				}
			}

			builder.AppendLine();
		}

		// ********** vec3 combinations **********
		if (vec.Type == VectorToGenerate.VectorType.Vector)
		{
			for (int i = 0; i < vec.NumbDimensions; i++)
			{
				for (int j = 0; j < vec.NumbDimensions; j++)
				{
					for (int k = 0; k < vec.NumbDimensions; k++)
					{
						builder.AppendLine($"public readonly {vec.ElementType}3 {vec.AxisNames[i]}{vec.AxisNames[j]}{vec.AxisNames[k]} => new {vec.ElementType}3({vec.AxisNames[i]}, {vec.AxisNames[j]}, {vec.AxisNames[k]});");
					}
				}
			}

			builder.AppendLine();
		}

		// ********** length **********
		builder.Append($"public readonly {(isLesserThanInt ? "int" : vec.ElementType)} LengthSquared => ");
		for (int i = 0; i < vec.NumbDimensions; i++)
		{
			if (i != 0)
			{
				builder.Append(" + ");
			}

			builder.Append($"{vec.AxisNames[i]} * {vec.AxisNames[i]}");
		}

		builder.AppendLine($"""
			;
			
			public readonly {(vec.ElementType == "float" ? "float" : "double")} Length => {(vec.ElementType == "float" ? "MathF" : "Math")}.Sqrt(LengthSquared);

			""");

		// ********** indexer **********
		builder.AppendLine($$"""
			/// <exception cref="IndexOutOfRangeException"></exception>
			public {{vec.ElementType}} this[int index]
			{
				readonly get
				{
					switch (index)
					{
			""");

		builder.Indent += 3;
		for (int i = 0; i < vec.NumbDimensions; i++)
		{
			builder.AppendLine($"""
				case {i}:
					return {vec.AxisNames[i]};
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

		for (int i = 0; i < vec.NumbDimensions; i++)
		{
			builder.AppendLine($"""
				case {i}:
					{vec.AxisNames[i]} = value;
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
		builder.AppendBinaryOperator(vec, "+");
		builder.AppendBinaryElementOperator(vec, "+");
		builder.AppendBinaryOperator(vec, "-");
		builder.AppendBinaryElementOperator(vec, "-");
		builder.AppendBinaryOperator(vec, "*");
		builder.AppendBinaryElementOperator(vec, "*");
		builder.AppendBinaryOperator(vec, "/");
		builder.AppendBinaryElementOperator(vec, "/");
		builder.AppendBinaryOperator(vec, "%");
		builder.AppendBinaryElementOperator(vec, "%");

		builder.AppendUnaryOperator(vec, "+");
		if (vec.IsSigned)
		{
			builder.AppendUnaryOperator(vec, "-");
		}

		// ********** equals operators **********
		builder.AppendCombinedBinaryOperator(vec, "==", "&&", "bool");
		builder.AppendCombinedBinaryOperator(vec, "!=", "||", "bool");

		// ********** conversions **********
		if (vec.Type == VectorToGenerate.VectorType.Vector)
		{
			for (int i = 0; i < VectorTypes.Length; i++)
			{
				if (i == indexOfElementType)
				{
					continue;
				}

				bool isExplicit = i > indexOfElementType || (isGroupType && (indexOfElementType % 2 == 0 ? (i == indexOfElementType + 1) : (i == indexOfElementType - 1)));

				string otherType = VectorTypes[i];

				builder.Append($"""
				public static {(isExplicit ? "explicit" : "implicit")} operator {vec.Name}({otherType}{vec.NumbDimensions} v)
					=> new {vec.Name}(
				""");

				for (int j = 0; j < vec.NumbDimensions; j++)
				{
					if (j != 0)
					{
						builder.Append(", ");
					}

					builder.Append($"({vec.ElementType})v.{vec.AxisNames[j]}");
				}

				builder.AppendLine("""
				);

				""");
			}
		}
		else if (vec.Type == VectorToGenerate.VectorType.Size)
		{
			// convert to/from the vector type
			builder.Append($"""
				public static explicit operator {vec.Name}({vec.ElementType}{vec.NumbDimensions} v)
					=> new {vec.Name}(
				""");

			for (int j = 0; j < vec.NumbDimensions; j++)
			{
				if (j != 0)
				{
					builder.Append(", ");
				}

				builder.Append($"v.{VectorToGenerate.VectorAxisNames[j]}");
			}

			builder.AppendLine("""
				);

				""");

			builder.Append($"""
				public static explicit operator {vec.ElementType}{vec.NumbDimensions}({vec.Name} v)
					=> new {vec.ElementType}{vec.NumbDimensions}(
				""");

			for (int j = 0; j < vec.NumbDimensions; j++)
			{
				if (j != 0)
				{
					builder.Append(", ");
				}

				builder.Append($"v.{vec.AxisNames[j]}");
			}

			builder.AppendLine("""
				);

				""");
		}

		// ********** 2D index functions **********
		if (vec.Type == VectorToGenerate.VectorType.Vector && vec.NumbDimensions == 2 && !vec.IsFloatingPoint)
		{
			string returnType = (!isLesserThanInt && !vec.IsSigned) ? vec.ElementType : vec.Is64Bit ? "long" : "int";

			builder.AppendLine($$"""
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public static {{vec.Name}} FromIndex({{vec.ElementType}} index, {{vec.ElementType}} width)
					=> new {{vec.Name}}(index % width, index / width);
				
				/// <exception cref="ArgumentOutOfRangeException"></exception>
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public static {{vec.Name}} FromIndexChecked({{vec.ElementType}} index, {{vec.ElementType}} width, {{vec.ElementType}} height)
				{
					{{vec.Name}} value = new {{vec.Name}}(index % width, index / width);
					return value.InBounds(width, height)
						? value
						: throw new ArgumentOutOfRangeException("index");
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public {{returnType}} ToIndex({{vec.ElementType}} width)
					=> X + (Y * width);
				
				/// <exception cref="IndexOutOfRangeException"></exception>
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public {{returnType}} ToIndexChecked({{vec.ElementType}} width, {{vec.ElementType}} height)
					=> InBounds(width, height)
						? X + (Y * width)
						: throw new IndexOutOfRangeException();
				
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public bool InBounds({{vec.ElementType}} width, {{vec.ElementType}} height)
					=> X >= 0 && X < width && Y >= 0 && Y < height;
				
				""");
		}

		// ********** 3D index functions **********
		if (vec.Type == VectorToGenerate.VectorType.Vector && vec.NumbDimensions == 3 && !vec.IsFloatingPoint)
		{
			string returnType = (!isLesserThanInt && !vec.IsSigned) ? vec.ElementType : vec.Is64Bit ? "long" : "int";

			// optimize using Math.DivRem(), not for ulong, https://stackoverflow.com/a/11317776
			builder.AppendLine($$"""
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public static {{vec.Name}} FromIndex({{vec.ElementType}} index, {{vec.ElementType}} width, {{vec.ElementType}} height)
					=> new {{vec.Name}}(index % width, (index / width) % height, index / (width * height));
				
				/// <exception cref="ArgumentOutOfRangeException"></exception>
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public static {{vec.Name}} FromIndexChecked({{vec.ElementType}} index, {{vec.ElementType}} width, {{vec.ElementType}} height, {{vec.ElementType}} depth)
				{
					{{vec.Name}} value = new {{vec.Name}}(index % width, (index / width) % height, index / (width * height));
					return value.InBounds(width, height, depth)
						? value
						: throw new ArgumentOutOfRangeException("index");
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public {{returnType}} ToIndex({{vec.ElementType}} width, {{vec.ElementType}} height)
					=> X + (Y * width) + (Z * width * height);
				
				/// <exception cref="IndexOutOfRangeException"></exception>
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public {{returnType}} ToIndexChecked({{vec.ElementType}} width, {{vec.ElementType}} height, {{vec.ElementType}} depth)
					=> InBounds(width, height, depth)
						? X + (Y * width) + (Z * width * height)
						: throw new IndexOutOfRangeException();
				
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public bool InBounds({{vec.ElementType}} width, {{vec.ElementType}} height, {{vec.ElementType}} depth)
					=> X >= 0 && X < width && Y >= 0 && Y < height && Z >= 0 && Z < depth;
				
				""");
		}

		// ********** GetEnumerator **********
		builder.Append($"""
			public readonly IEnumerator<{vec.ElementType}> GetEnumerator()
				=> new ArrayEnumerator<{vec.ElementType}>
			""");
		builder.AppendCallWithValues(vec);
		builder.AppendLine("""
			;

			""");

		builder.Append($"""
			readonly IEnumerator IEnumerable.GetEnumerator()
				=> new ArrayEnumerator<{vec.ElementType}>
			""");
		builder.AppendCallWithValues(vec);
		builder.AppendLine("""
			;

			""");

		// ********** min/max **********
		builder.Append($"""
			public static {vec.Name} Min({vec.Name} a, {vec.Name} b)
				=> new {vec.Name}(
			""");

		for (int i = 0; i < vec.NumbDimensions; i++)
		{
			if (i != 0)
			{
				builder.Append(", ");
			}

			builder.Append("Math.Min");
			builder.AppendCallWithAB(vec, i);
		}

		builder.Append($"""
			);

			public static {vec.Name} Max({vec.Name} a, {vec.Name} b)
				=> new {vec.Name}(
			""");

		for (int i = 0; i < vec.NumbDimensions; i++)
		{
			if (i != 0)
			{
				builder.Append(", ");
			}

			builder.Append("Math.Max");
			builder.AppendCallWithAB(vec, i);
		}

		builder.AppendLine("""
			);

			""");

		// ********** Distance/Dot **********
		if (vec.Type == VectorToGenerate.VectorType.Vector)
		{
			builder.Append($"""
			public static double Distance({vec.Name} a, {vec.Name} b)
				=> (a - b).Length;
			
			public static {(isLesserThanInt ? "int" : vec.ElementType)} Dot({vec.Name} a, {vec.Name} b)
				=> 
			""");

			for (int i = 0; i < vec.NumbDimensions; i++)
			{
				if (i != 0)
				{
					builder.Append(" + ");
				}

				builder.Append($"(a.{vec.AxisNames[i]} * b.{vec.AxisNames[i]})");
			}

			builder.AppendLine("""
			;

			""");
		}

		// ********** Normalized **********
		if (vec.Type == VectorToGenerate.VectorType.Vector && vec.IsFloatingPoint)
		{
			builder.AppendLine($"""
				public readonly {vec.Name} Normalized()
					=> this / ({vec.ElementType})Length;
				
				""");
		}

		// ********** Cross **********
		if (vec.Type == VectorToGenerate.VectorType.Vector && vec.NumbDimensions == 3)
		{
			builder.AppendLine($"""
				public static {vec.Name} Cross({vec.Name} a, {vec.Name} b)
					=> new {vec.Name}(
						a.Y * b.Z - a.Z * b.Y,
						a.Z * b.X - a.X * b.Z,
						a.X * b.Y - a.Y * b.X
					);
				
				""");
		}

		// ********** GetHashCode **********
		builder.Append("""
			public readonly override int GetHashCode()
				=> HashCode.Combine
			""");
		builder.AppendCallWithValues(vec);
		builder.AppendLine("""
			;

			""");

		// ********** Equals **********
		builder.AppendLine($"""
			public readonly override bool Equals(object? obj)
				=> obj is {vec.Name} other && this == other;
			
			public readonly bool Equals({vec.Name} other)
				=> this == other;
			
			""");

		// ********** ToString **********
		builder.AppendLine("""
			public readonly override string ToString()
				=> ToString("G", CultureInfo.InvariantCulture);
			
			public readonly string ToString(string format)
				=> ToString(format, CultureInfo.InvariantCulture);
			
			public readonly string ToString(string format, IFormatProvider formatProvider)
			{
				StringBuilder sb = new StringBuilder();
				string separator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;
			""");

		builder.Indent++;

		if (vec.Type == VectorToGenerate.VectorType.Vector)
		{
			builder.AppendLine("sb.Append('<');");

			for (int i = 0; i < vec.NumbDimensions; i++)
			{
				if (i != 0)
				{
					builder.AppendLine("""
					sb.Append(separator);
					sb.Append(' ');
					""");
				}

				builder.AppendLine($"sb.Append({vec.AxisNames[i]}.ToString(format, formatProvider));");
			}

			builder.AppendLine("sb.Append('>');");
		}
		else if (vec.Type == VectorToGenerate.VectorType.Size)
		{
			for (int i = 0; i < vec.NumbDimensions; i++)
			{
				if (i != 0)
				{
					builder.AppendLine("""
					sb.Append(separator);
					sb.Append(' ');
					""");
				}

				builder.AppendLine($"sb.Append({vec.AxisNames[i]}.ToString(format, formatProvider));");
			}
		}
		else
		{
			Debug.Fail("Unknown vector type.");
		}

		builder.Indent--;

		builder.AppendLine("""
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
	}
}
