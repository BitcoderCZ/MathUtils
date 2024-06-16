namespace MathUtils.Equations.Parts
{
    public interface IPart
    {
        double GetValue(CalculationContext context);

        string ToString(Equation.StringOptions options);
    }
}
