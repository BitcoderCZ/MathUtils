using MathUtils.Equations.Parts.Functions;
using MathUtils.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace MathUtils.Equations;

public class ParseSettings
{
	public static ParseSettings Default => new ParseSettings();

	/// <summary>
	/// If <see langword="true"/> 2X; 3(2+2) will become 2 * X; 3 * (2 + 2)
	/// </summary>
	public readonly bool ImplicitMultiplication;

	public readonly IEnumerable<FunctionDefinition> Functions;

	public ParseSettings(bool _implicitMultiplication = true, IEnumerable<FunctionDefinition>? _functions = null)
	{
		ImplicitMultiplication = _implicitMultiplication;
		Functions = _functions is null ? new List<FunctionDefinition>() : new List<FunctionDefinition>(_functions);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="paramCount"></param>
	/// <returns></returns>
	/// <exception cref="ExecutionException"></exception>
	public FunctionDefinition GetFunction(string name, int paramCount)
	{
		FunctionDefinition? definition = Functions.Where(definition => definition.Name == name && definition.ParamCount == paramCount).FirstOrDefault();

		return definition ?? throw new ParseException(-1, $"Function '{name}' with '{paramCount}' parameters isn't defined");
	}
}
