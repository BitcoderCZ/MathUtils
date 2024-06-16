using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathUtils.Equations.Parts.Operations
{
    public class MinusOp : BinaryOperation
    {
        public static int Priority => 4;

        public MinusOp(IPart _left, IPart _right) : base(_left, _right)
        {
        }

        public override double GetValue(CalculationContext context)
            => Left.GetValue(context) - Right.GetValue(context);

        public override string ToString(Equation.StringOptions options)
            => Left.ToString(options) + options.Separator + "-" + options.Separator + Right.ToString(options);
    }
}
