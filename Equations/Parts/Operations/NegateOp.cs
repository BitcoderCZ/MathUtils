namespace MathUtils.Equations.Parts.Operations
{
    public class NegateOp : UnaryOperation
    {
        public static int Priority => 1;

        public NegateOp(IPart _part) : base(_part)
        {
        }

        public override double GetValue(CalculationContext context)
            => -Part.GetValue(context);

        public override string ToString(Equation.StringOptions options)
            => "-" + Part.ToString(options);
    }
}
