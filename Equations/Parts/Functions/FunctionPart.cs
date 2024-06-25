using System;
using System.Collections.Generic;
using System.Linq;

namespace MathUtils.Equations.Parts.Functions
{
    public sealed class FunctionPart : IPart
    {
        public readonly string Name;
        public readonly IList<IPart> Params;
        private readonly Func<IList<IPart>, CalculationContext, double> func;

        public FunctionPart(string _name, IList<IPart> _params, Func<IList<IPart>, CalculationContext, double> _func)
        {
            Name = _name;
            Params = _params;
            func = _func;
        }

        public double GetValue(CalculationContext context)
            => func(Params, context);

        public override string ToString()
            => ToString(Equation.StringOptions.Default);
        public string ToString(Equation.StringOptions options)
            => Name + "(" + string.Join("," + options.Separator, Params.Select(part => part.ToString(options))) + ")";
    }
}
