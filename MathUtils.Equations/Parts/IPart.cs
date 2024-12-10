namespace MathUtils.Equations.Parts;

public interface IPart
{
	double GetValue(CalculationSettings context);

	string ToString(Equation.StringOptions options);
}
