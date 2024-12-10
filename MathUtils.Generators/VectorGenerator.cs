﻿using MathUtils.Generators.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace MathUtils.Generators;

[Generator]
public class VectorGenerator : IIncrementalGenerator
{
	private const string VectorAttribute = """
		namespace MathUtils.Generators
		{
			[System.AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
			public sealed class VectorAttribute : System.Attribute
			{
				public VectorAttribute(int numbDimensions, string elementType)
				{
					NumbDimensions = numbDimensions;
					ElementType = elementType;
				}

				public int NumbDimensions { get; }
				public string ElementType { get; }
			}
		}
		""";

	private static readonly ImmutableArray<string> AxisNames = ["X", "Y", "Z", "W"];
	private static readonly ImmutableArray<string> AxisParamNames = ["x", "y", "z", "w"];

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		context.RegisterPostInitializationOutput(ctx => ctx.AddSource("VectorAttribute.g.cs", SourceText.From(VectorAttribute, Encoding.UTF8)));

		IncrementalValuesProvider<VectorToGenerate?> vectorsToGenerate = context.SyntaxProvider
			.ForAttributeWithMetadataName(
				"MathUtils.Generators.VectorAttribute",
				predicate: static (s, _) => true,
				transform: static (ctx, _) => GetVectorToGenerate(ctx.SemanticModel, ctx.TargetNode))
			.Where(static m => m is not null);

		context.RegisterSourceOutput(vectorsToGenerate, static (spc, source) => Execute(source, spc));
	}

	private static void Execute(VectorToGenerate? vectorToGenerate, SourceProductionContext context)
	{
		if (vectorToGenerate is { } value)
		{
			try
			{
				string result = GenerateVectorClass(value);
				context.AddSource($"{value.Name}.g.cs", SourceText.From(result, Encoding.UTF8));
			}
			catch (Exception ex)
			{
				string s = ex.ToString();
			}
		}
	}

	private static VectorToGenerate? GetVectorToGenerate(SemanticModel semanticModel, SyntaxNode vectorDeclarationSyntax)
	{
		if (semanticModel.GetDeclaredSymbol(vectorDeclarationSyntax) is not INamedTypeSymbol vectorSymbol)
		{
			return null;
		}

		foreach (var attrib in vectorSymbol.GetAttributes())
		{
			if (attrib.AttributeClass?.Name == "VectorAttribute")
			{
				int numbDimensions = (int)attrib.ConstructorArguments[0].Value!;
				string elementType = (string)attrib.ConstructorArguments[1].Value!;

				if (numbDimensions < 1 || numbDimensions > 4)
				{
					throw new IndexOutOfRangeException("numbDimensions must be between 1 and 4.");
				}
				else if (string.IsNullOrEmpty(elementType))
				{
					throw new Exception("elementType cannot be null or empty.");
				}

				return new VectorToGenerate(
					vectorSymbol.Name,
					numbDimensions,
					elementType
				);
			}
		}

		Debug.Fail("The vector must have the VectorAttribute");
		return null;
	}

	private static string GenerateVectorClass(VectorToGenerate vectorToGenerate)
	{
		// TODO: add xml documentation
		IndentedStringBuilder builder = new IndentedStringBuilder(new ValueStringBuilder(1024 * 4));
		
		// ********** struct start **********
		// ********** Zero **********
		builder.AppendLine($$"""
			using System;
			using System.Collections;
			using System.Collections.Generic;
			using System.Globalization;
			using System.Text;
			using MathUtils.Utils;

			namespace MathUtils.Vectors
			{
				public partial struct {{vectorToGenerate.Name}} : IEnumerable<int>, IEquatable<int2>
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

		builder.AppendLine(")");
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

			builder.AppendLine(")");
			builder.AppendLine();
		}

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

		// ********** vec2 combinations **********
		for (int i = 0; i < vectorToGenerate.NumbDimensions; i++)
		{
			for (int j = 0; j < vectorToGenerate.NumbDimensions; j++)
			{
				builder.AppendLine($"public {vectorToGenerate.Name} {AxisNames[i]}{AxisNames[j]} = new {vectorToGenerate.Name}({AxisNames[i]}, {AxisNames[j]});");
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
					builder.AppendLine($"public {vectorToGenerate.Name} {AxisNames[i]}{AxisNames[j]}{AxisNames[k]} = new {vectorToGenerate.Name}({AxisNames[i]}, {AxisNames[j]}, {AxisNames[k]});");
				}
			}
		}

		builder.AppendLine();

		// ********** length **********
		builder.Append($"public {vectorToGenerate.ElementType} LengthSquared => ");
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
		AppendUnaryOperator(ref builder, "-");

		// ********** equals operators **********
		AppendCombinedBinaryOperator(ref builder, "==", "&&", "bool");
		AppendCombinedBinaryOperator(ref builder, "!=", "||", "bool");

		// ********** GetEnumerator **********
		builder.Append($"""
			public IEnumerator<int> GetEnumerator()
				=> new ArrayEnumerator<int>
			""");
		AppendCallWithValues(ref builder);
		builder.AppendLine("""
			;

			""");

		builder.Append($"""
			IEnumerator IEnumerator.GetEnumerator()
				=> new ArrayEnumerator<int>
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
			
			public static {vectorToGenerate.ElementType} Dot({vectorToGenerate.Name} a, {vectorToGenerate.Name} b)
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
			);

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
			);

			""");

		// ********** Equals **********
		builder.AppendLine($"""
			public override bool Equals([NotNullWhen(true)] object? obj)
				=> obj is {vectorToGenerate.Name} other && this == other;
			
			public bool Equals(int2 other)
				=> this == {vectorToGenerate.Name};
			
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

				builder.Append($"{opSymbol}a.{AxisNames[i]}");
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
