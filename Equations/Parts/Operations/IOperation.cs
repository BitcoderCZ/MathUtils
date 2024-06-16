namespace MathUtils.Equations.Parts.Operations
{
    public interface IOperation : IPart
    {
        static int Priority { get; }
    }
}
