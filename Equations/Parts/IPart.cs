using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathUtils.Equations.Parts
{
    public interface IPart
    {
        double GetValue(CalculationContext context);

        string ToString(Equation.StringOptions options);
    }
}
