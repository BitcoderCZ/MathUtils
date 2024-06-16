using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MathUtils.Measures
{
    public class Measure
    {
        public readonly string Name;

        public readonly IEnumerable<Unit> Units;
        public readonly Unit BaseUnit;

        public Measure(string _name, IEnumerable<Unit> _units, Unit _baseUnit)
        {
            Name = _name;
            Units = _units;
            BaseUnit = _baseUnit;
        }

        public double Convert(Value value, Unit to)
            => Convert(value.Val, value.Unit, to);
        public double Convert(double value, Unit from, Unit to)
        {
            if (from.Measure != this) throw new InvalidDataException($"{nameof(from)} must be a unit of {Name}");
            if (to.Measure != this) throw new InvalidDataException($"{nameof(to)} must be a unit of {Name}");

            if (from == to) return value;

            return (value * from.ToBaseUnit) / to.ToBaseUnit;
        }

        /// <summary>
        /// Chooses a <see cref="Unit"/> so that <paramref name="value"/> is in 1-10 range (or the <see cref="Unit"/> the comes closest to it)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Value ChooseUnit(Value value)
            => ChooseUnit(value.Val, value.Unit);
        /// <summary>
        /// Chooses a <see cref="Unit"/> so that <paramref name="value"/> is in 1-10 range (or the <see cref="Unit"/> the comes closest to it)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"><see cref="Unit"/> of the <paramref name="value"/></param>
        /// <returns></returns>
        public Value ChooseUnit(double value, Unit unit)
        {
            List<int> decimalSepPlace = new();

            foreach (Unit u in Units)
            {
                double val = Convert(value, unit, u);
                string str = val.ToString("0.###################################################################################################################################################################################################################################################################################################################################################", NumberFormatInfo.InvariantInfo);

                int i = str.IndexOf('.');

                if (i == -1) decimalSepPlace.Add(str.Length - 1);
                else if (i == 1 && str[0] != '0') decimalSepPlace.Add(0);
                else if (i != 1) decimalSepPlace.Add(i - 1);
                else
                { // 0.xxx
                    int j;
                    for (j = 2; j < str.Length; j++)
                        if (str[j] != '0')
                            break;

                    decimalSepPlace.Add(-j);
                }
            }

            int lowest = int.MaxValue;
            int lowestIndex = 0;
            for (int i = 0; i < decimalSepPlace.Count; i++)
                if (Math.Abs(decimalSepPlace[i]) < lowest)
                {
                    lowest = decimalSepPlace[i];
                    lowestIndex = i;
                }

            Unit newUnit = Units.ElementAt(lowestIndex);

            return new Value(Convert(value, unit, newUnit), newUnit);
        }

        public static bool operator ==(Measure a, Measure b)
            => a?.Equals(b) ?? b is null;
        public static bool operator !=(Measure a, Measure b)
            => !a?.Equals(b) ?? b is not null;

        public override int GetHashCode()
            => Name.GetHashCode();

        public override bool Equals(object? obj)
        {
            if (obj is Measure other) return Equals(other);
            else return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Measure other)
            => Name == other.Name;
    }
}
