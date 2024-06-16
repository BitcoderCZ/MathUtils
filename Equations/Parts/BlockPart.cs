using MathUtils.Equations.Parts.Operations;

namespace MathUtils.Equations.Parts
{
    // not really unary or operation, but kinda?
    public class BlockPart : UnaryOperation
    {
        public BlockPart(IPart _part) : base(_part)
        {
        }

        public override double GetValue(CalculationContext context)
            => Part.GetValue(context);

        public override string ToString(Equation.StringOptions options)
            => "(" + Part.ToString(options) + ")";
    }
}
