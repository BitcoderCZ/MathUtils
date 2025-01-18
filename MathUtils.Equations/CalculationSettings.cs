using MathUtils.Exceptions;
using MathUtils.Measures;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MathUtils.Equations;

public class CalculationSettings
{
	public static CalculationSettings Default => new CalculationSettings(new Dictionary<string, double>(), Units.Angles.Degree);

	public readonly IReadOnlyDictionary<string, double> Variables;
	public readonly Unit AngleUnit;

	public CalculationSettings(IDictionary<string, double> _variables, Unit _angleUnit)
	{
		Variables = new ReadOnlyDictionary<string, double>(_variables);
		AngleUnit = _angleUnit;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	/// <exception cref="ExecutionException"></exception>
	public double GetVariable(string name)
		=> Variables.TryGetValue(name, out double value)
			? value
			: throw new ExecutionException($"Variable '{name}' isn't defined");
}
