using static MathUtils.Measures.Units.Angles;
using static MathUtils.Measures.Units.Storages;

namespace MathUtils.Measures
{
    public static class Units
    {
        static Units()
        {
            Measures.Init();
        }

        internal static void Init()
        {
            Degree = new AngleUnit(Measures.Angle, "Degree", "°", 1d, 360d);
            Radian = new AngleUnit(Measures.Angle, "Radian", "rad", Maths.RadToDeg, Maths.PI2);

            Storages.Byte = new Unit(Measures.Storage, "Byte", "B", 1d);
            KiloByte = new Unit(Measures.Storage, "KiloByte", "KB", 1024d);
            MegaByte = new Unit(Measures.Storage, "MegaByte", "MB", 1024d * 1024d);
            GigaByte = new Unit(Measures.Storage, "GigaByte", "GB", 1024d * 1024d * 1024d);
            TeraByte = new Unit(Measures.Storage, "TeraByte", "TB", 1024d * 1024d * 1024d * 1024d);
            PetaByte = new Unit(Measures.Storage, "PetaByte", "PB", 1024d * 1024d * 1024d * 1024d * 1024d);
        }
        internal static void Init2()
        {
            Degree.Measure = Measures.Angle;
            Radian.Measure = Measures.Angle;

            Storages.Byte.Measure = Measures.Storage;
            KiloByte.Measure = Measures.Storage;
            MegaByte.Measure = Measures.Storage;
            GigaByte.Measure = Measures.Storage;
            TeraByte.Measure = Measures.Storage;
            PetaByte.Measure = Measures.Storage;
        }

        public static class Angles
        {
            static Angles()
            {
                Measures.Init();
            }

            public sealed class AngleUnit : Unit
            {
                public readonly double FullAngleValue;

                public AngleUnit(Measure _measure, string _name, string _symbol, double _toBaseUnit, double _fullAngleValue) : base(_measure, _name, _symbol, _toBaseUnit)
                {
                    FullAngleValue = _fullAngleValue;
                }
            }

            public static AngleUnit Degree { get; internal set; } = null!;
            public static AngleUnit Radian { get; internal set; } = null!;
        }
        public static class Storages
        {
            static Storages()
            {
                Measures.Init();
            }

            public static Unit Byte { get; internal set; } = null!;
            public static Unit KiloByte { get; internal set; } = null!;
            public static Unit MegaByte { get; internal set; } = null!;
            public static Unit GigaByte { get; internal set; } = null!;
            public static Unit TeraByte { get; internal set; } = null!;
            public static Unit PetaByte { get; internal set; } = null!;
        }
    }
}
