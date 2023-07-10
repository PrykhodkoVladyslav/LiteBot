using System.Numerics;

namespace LiteBot.MathExtentions;

public static class MathExpanded {
	public static bool Between<T>(T start, T value, T end) where T : INumber<T> {
		return start <= value && value <= end;
	}
}
