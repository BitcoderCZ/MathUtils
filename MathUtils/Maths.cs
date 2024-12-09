using System;

namespace MathUtils;

public static class Maths
{
	public const double PI2 = Math.PI * 2d;
	public const double DegToRad = Math.PI / 180d;
	public const double RadToDeg = 180d / Math.PI;

	public static float Round(float value, int decimalPlaces = 0)
	{
		if (decimalPlaces == 0) return MathF.Round(value);
		else
		{
			float multiplier = MathF.Pow(10f, decimalPlaces);
			return MathF.Round(value * multiplier) / multiplier;
		}
	}

	public static double Round(double value, int decimalPlaces = 0)
	{
		if (decimalPlaces == 0) return Math.Round(value);
		else
		{
			double multiplier = Math.Pow(10.0, decimalPlaces);
			return Math.Round(value * multiplier) / multiplier;
		}
	}
}
