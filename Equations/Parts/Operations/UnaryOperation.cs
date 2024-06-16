namespace MathUtils.Equations.Parts.Operations
{
    public abstract class UnaryOperation : IOperation
    {
        public IPart Part { get; protected set; }

        protected UnaryOperation(IPart _part)
        {
            Part = _part;
        }

        public abstract double GetValue(CalculationContext context);

        public override string ToString()
            => ToString(Equation.StringOptions.Default);
        public abstract string ToString(Equation.StringOptions options);
    }
}
