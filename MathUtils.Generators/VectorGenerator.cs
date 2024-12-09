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
					System.ArgumentOutOfRangeException.ThrowIfLessThan(numbDimensions, 1);
					System.ArgumentOutOfRangeException.ThrowIfGreaterThan(numbDimensions, 4);
					System.ArgumentNullException.ThrowIfNullOrEmpty(elementType);

					NumbDimensions = numbDimensions;
					ElementType = elementType;
				}

				public int NumbDimensions { get; }
				public string ElementType { get; }
			}
		}
		""";

	private static readonly ImmutableArray<string> AxisNames = ["X", "Y", "Z", "W"];

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
				return new VectorToGenerate(
					vectorSymbol.Name,
					(int)attrib.ConstructorArguments[0].Value!,
					(string)attrib.ConstructorArguments[1].Value!
				);
			}
		}

		Debug.Fail("The vector must have the VectorAttribute");
		return null;
	}

	private static string GenerateVectorClass(VectorToGenerate vectorToGenerate)
	{
		var builder = new StringBuilder();

		builder.AppendLine($$"""
			namespace MathUtils.Vectors
			{
				public partial struct {{vectorToGenerate.Name}}
				{
			""");

		for (int i = 0; i < vectorToGenerate.NumbDimensions; i++)
		{
			builder.AppendLine($"		public {vectorToGenerate.ElementType} {AxisNames[i]};");
		}

		builder.AppendLine("""
				}
			}
			""");

		return builder.ToString();
	}
}
