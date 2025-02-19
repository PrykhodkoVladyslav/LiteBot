using LiteBot.Interfaces;

namespace LiteBot.Services;

public class SingletonRandom : ISingletonRandom {
	private readonly Random _random = new();

	public int Next() => _random.Next();
	public int Next(int min, int max) => _random.Next(min, max);
}
