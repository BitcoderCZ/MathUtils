using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathUtils.Equations.Parts
{
    public class VariablePart : IPart
    {
        public readonly string Name;

        public VariablePart(string _name)
        {
            Name = _name;
        }

        public double GetValue(CalculationContext context)
            => context.GetVariable(Name);

        public override string ToString()
            => ToString(Equation.StringOptions.Default);
        public string ToString(Equation.StringOptions options)
            => Name;
    }
}
