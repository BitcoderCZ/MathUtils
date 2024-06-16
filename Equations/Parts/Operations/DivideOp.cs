﻿namespace MathUtils.Equations.Parts.Operations
{
    public class DivideOp : BinaryOperation
    {
        public static int Priority => 5;

        public DivideOp(IPart _left, IPart _right) : base(_left, _right)
        {
        }

        public override double GetValue(CalculationContext context)
            => Left.GetValue(context) / Right.GetValue(context);

        public override string ToString(Equation.StringOptions options)
            => Left.ToString(options) + options.Separator + "/" + options.Separator + Right.ToString(options);
    }
}
