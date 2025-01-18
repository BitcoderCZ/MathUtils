using static MathUtils.Measures.Units;

namespace MathUtils.Measures;

public static class Measures
{
	private static bool initialized = false;

	public static Measure Angle { get; internal set; } = null!;
	public static Measure Storage { get; internal set; } = null!;

	static Measures()
	{
		Init();
	}

	internal static void Init()
	{
		if (initialized) return;

		Units.Init();

		Angle = new Measure("Angle", [Angles.Degree, Angles.Radian], Angles.Degree);
		Storage = new Measure("Storage", [Storages.Byte, Storages.KiloByte, Storages.MegaByte, Storages.GigaByte, Storages.TeraByte, Storages.PetaByte], Storages.Byte);

		Units.Init2();

		initialized = true;
	}
}
