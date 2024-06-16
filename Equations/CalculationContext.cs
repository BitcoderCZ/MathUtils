using MathUtils.Exceptions;
using MathUtils.Measures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathUtils.Equations
{
    public class CalculationContext
    {
        public static CalculationContext Default => new CalculationContext(new Dictionary<string, double>(), Units.Angles.Degree);

        public readonly IReadOnlyDictionary<string, double> Variables;
        public readonly Unit AngleUnit;

        public CalculationContext(IDictionary<string, double> _variables, Unit _angleUnit)
        {
            Variables = new ReadOnlyDictionary<string, double>(_variables);
            AngleUnit = _angleUnit;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ExecutionException"></exception>
        public double GetVariable(string name)
        {
            if (Variables.TryGetValue(name, out double value)) return value;
            else throw new ExecutionException($"Variable '{name}' isn't defined");
        }
    }
}
