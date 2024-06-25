using System.Runtime.CompilerServices;

namespace MathUtils.Measures
{
    public class Unit
    {
        public Measure Measure { get; internal set; }

        public readonly string Name;
        public readonly string Symbol;
        public readonly double ToBaseUnit;

        public Unit(Measure _measure, string _name, string _symbol, double _toBaseUnit)
        {
            Measure = _measure;
            Name = _name;
            Symbol = _symbol;
            ToBaseUnit = _toBaseUnit;
        }

        public static bool operator ==(Unit a, Unit b)
            => a?.Equals(b) ?? b is null;
        public static bool operator !=(Unit a, Unit b)
            => !a?.Equals(b) ?? !(b is null);

        public override int GetHashCode()
            => Name.GetHashCode() ^ Measure.GetHashCode();

        public override bool Equals(object? obj)
        {
            if (obj is Unit other) return Equals(other);
            else return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Unit other)
            => Name == other.Name && Symbol == other.Symbol;
    }
}
