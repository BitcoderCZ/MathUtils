namespace MathUtils.Equations.Parts
{
    public class NumberPart : IPart
    {
        public readonly double Value;

        public NumberPart(double _value)
        {
            Value = _value;
        }

        public double GetValue(CalculationContext context)
            => Value;

        public override string ToString()
            => ToString(Equation.StringOptions.Default);
        public string ToString(Equation.StringOptions options)
            => Value.ToString(options.NumberFormat);
    }
}
