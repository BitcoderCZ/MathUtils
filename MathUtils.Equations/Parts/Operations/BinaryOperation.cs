namespace MathUtils.Equations.Parts.Operations;

public abstract class BinaryOperation : IOperation
{
	public IPart Left { get; protected set; }
	public IPart Right { get; protected set; }

	protected BinaryOperation(IPart _left, IPart _right)
	{
		Left = _left;
		Right = _right;
	}

	public abstract double GetValue(CalculationSettings context);

	public override string ToString()
		=> ToString(Equation.StringOptions.Default);
	public abstract string ToString(Equation.StringOptions options);
}
