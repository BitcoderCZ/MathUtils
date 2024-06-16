using System.Runtime.CompilerServices;

namespace MathUtils.Measures
{
    public class Value
    {
        public double Val;
        public Unit Unit;

        public Value(double _val, Unit _unit)
        {
            Val = _val;
            Unit = _unit;
        }

        public static bool operator ==(Value a, Value b)
            => a?.Equals(b) ?? b is null;
        public static bool operator !=(Value a, Value b)
            => !a?.Equals(b) ?? b is not null;

        public override string ToString()
            => Val + " " + Unit.Symbol;

        public override int GetHashCode()
            => Val.GetHashCode() ^ Unit.GetHashCode();

        public override bool Equals(object? obj)
        {
            if (obj is Value other) return Equals(other);
            else return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Value other)
            => Val == other.Val && Unit == other.Unit;
    }
}
