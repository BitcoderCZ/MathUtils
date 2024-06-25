using System;
using System.Collections.Generic;

namespace MathUtils.Equations.Parts.Functions
{
    public class FunctionDefinition
    {
        public readonly string Name;
        public readonly int ParamCount;
        public readonly Func<IList<IPart>, CalculationContext, double> Func;

        public FunctionDefinition(string _name, int _paramCount, Func<IList<IPart>, CalculationContext, double> _func)
        {
            Name = _name;
            ParamCount = _paramCount;
            Func = _func;
        }

        public FunctionPart CreatePart(IList<IPart> @params)
            => new FunctionPart(Name, @params, Func);
    }
}
