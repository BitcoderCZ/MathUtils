using System;
using System.Collections.Generic;
using static MathUtils.Measures.Measures;
using static MathUtils.Measures.Units.Angles;

namespace MathUtils.Equations.Parts.Functions
{
    public static class Funs
    {
        public static IEnumerable<FunctionDefinition> AllFunctions = new List<FunctionDefinition>()
        {
            Abs,
            Min,
            Max,
            Clamp,
            Ceiling,
            Round,
            Floor,
            Sin,
            Cos,
            Tan,
        };

        public static FunctionDefinition Abs => new FunctionDefinition("abs", 1, (parts, content) => Math.Abs(parts[0].GetValue(content)));

        public static FunctionDefinition Min => new FunctionDefinition("min", 2, (parts, content) => Math.Min(parts[0].GetValue(content), parts[1].GetValue(content)));
        public static FunctionDefinition Max => new FunctionDefinition("max", 2, (parts, content) => Math.Max(parts[0].GetValue(content), parts[1].GetValue(content)));
        public static FunctionDefinition Clamp => new FunctionDefinition("clamp", 3, (parts, content) => Math.Clamp(parts[0].GetValue(content), parts[1].GetValue(content), parts[2].GetValue(content)));

        public static FunctionDefinition Ceiling => new FunctionDefinition("ceiling", 1, (parts, content) => Math.Ceiling(parts[0].GetValue(content)));
        public static FunctionDefinition Round => new FunctionDefinition("round", 1, (parts, content) => Math.Round(parts[0].GetValue(content)));
        public static FunctionDefinition Floor => new FunctionDefinition("floor", 1, (parts, content) => Math.Floor(parts[0].GetValue(content)));

        public static FunctionDefinition Sin => new FunctionDefinition("sin", 1, (parts, content) => Math.Sin(Angle.Convert(parts[0].GetValue(content), content.AngleUnit, Radian)));
        public static FunctionDefinition Cos => new FunctionDefinition("cos", 1, (parts, content) => Math.Cos(Angle.Convert(parts[0].GetValue(content), content.AngleUnit, Radian)));
        public static FunctionDefinition Tan => new FunctionDefinition("tan", 1, (parts, content) => Math.Tan(Angle.Convert(parts[0].GetValue(content), content.AngleUnit, Radian)));
    }
}
