using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
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
			string result = GenerateVectorClass(value);

			context.AddSource($"{value.Name}.g.cs", SourceText.From(result, Encoding.UTF8));
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
		var builder = new StringBuilder();

		// ********** struct start **********
		// ********** Zero **********
		builder.AppendLine($$"""
			namespace MathUtils.Vectors
			{
				public partial struct {{vectorToGenerate.Name}} : IEnumerable<int>, IEquatable<int2>
				{
					public static readonly {{vectorToGenerate.Name}} Zero = default;

			""");

		// ********** One **********
		builder.Append($"		public static readonly {vectorToGenerate.Name} One = new {vectorToGenerate.Name}(");

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
			builder.Append($"		public static readonly {vectorToGenerate.Name} Unit{AxisNames[i]} = new {vectorToGenerate.Name}(");

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
			builder.AppendLine($"		public {vectorToGenerate.ElementType} {AxisNames[i]};");
			builder.AppendLine();
		}

		// ********** 1 element ctor **********
		builder.AppendLine($$"""
					public {{vectorToGenerate.Name}}({{vectorToGenerate.ElementType}} value)
					{
			""");

		for (int i = 0; i < vectorToGenerate.NumbDimensions; i++)
		{
			builder.AppendLine($"			{AxisNames[i]} = value;");
		}

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

		for (int i = 0; i < vectorToGenerate.NumbDimensions; i++)
		{
			builder.AppendLine($"			{AxisNames[i]} = {AxisParamNames[i]};");
		}

		builder.AppendLine("""
					}

			""");

		// TODO {x}{y} combinations

		// ********** length **********
		builder.Append("		public double LengthSquared => ");
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

		for (int i = 0; i < vectorToGenerate.NumbDimensions; i++)
		{
			builder.AppendLine($"""
									case {i}:
										return {AxisNames[i]};
				""");
		}

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

		for (int i = 0; i < vectorToGenerate.NumbDimensions; i++)
		{
			builder.AppendLine($"""
									case {i}:
										{AxisNames[i]} = value;
										break;
				""");
		}

		builder.AppendLine("""
								default:
									throw new IndexOutOfRangeException();
							}
						}
					}

			""");

		// ********** operators **********
		AppendBinaryOperator("+");
		AppendBinaryElementOperator("+");
		AppendBinaryOperator("-");
		AppendBinaryElementOperator("-");
		AppendBinaryOperator("*");
		AppendBinaryElementOperator("*");
		AppendBinaryOperator("/");
		AppendBinaryElementOperator("/");
		AppendBinaryOperator("%");
		AppendBinaryElementOperator("5");

		AppendUnaryOperator("+");
		AppendUnaryOperator("-");

		// ********** equals operators **********
		AppendCombinedBinaryOperator("==", "&&", "bool");
		AppendCombinedBinaryOperator("!=", "||", "bool");

		// ********** GetEnumerator **********
		/*
		 public IEnumerator<int> GetEnumerator()
			=> new ArrayEnumerator<int>(X, Y);

		IEnumerator IEnumerable.GetEnumerator()
			=> new ArrayEnumerator<int>(X, Y);
		 */

		// ********** struct end **********
		builder.AppendLine("""
				}
			}
			""");

		return builder.ToString();

		void AppendUnaryOperator(string opSymbol)
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

		void AppendBinaryOperator(string opSymbol)
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

		void AppendBinaryElementOperator(string opSymbol)
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

		void AppendCombinedBinaryOperator(string opSymbol, string combinationSymbol, string resultType)
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
		void AppendCall()
		{

		}

		// (a.X, b.X)
		void AppendABCall(int axisIndex)
		{

		}
	}
}
