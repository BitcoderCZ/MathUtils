using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathUtils.Equations.Parts.Operations
{
    public class IdentityOp : UnaryOperation
    {
        public static int Priority => 1;

        public IdentityOp(IPart _part) : base(_part)
        {
        }

        public override double GetValue(CalculationContext context)
            => Part.GetValue(context);

        public override string ToString(Equation.StringOptions options)
            => "+" + Part.ToString(options);
    }
}
