using System;

namespace MathUtils.Equations.Parts.Operations
{
    public class PowerOp : BinaryOperation
    {
        public static int Priority => 6;

        public PowerOp(IPart _left, IPart _right) : base(_left, _right)
        {
        }

        public override double GetValue(CalculationContext context)
        {
            double numb = Left.GetValue(context);
            double exponent = Right.GetValue(context);
            double res = Math.Pow(Math.Abs(numb), exponent);
            if (numb < 0d && Math.Abs(exponent) % 2d == 1d)
                res = -res;

            return res;
        }

        public override string ToString(Equation.StringOptions options)
            => Left.ToString(options) + options.Separator + "^" + options.Separator + Right.ToString(options);
    }
}
