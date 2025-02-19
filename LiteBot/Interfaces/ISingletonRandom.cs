namespace LiteBot.Interfaces;

public interface ISingletonRandom {
	int Next();
	int Next(int min, int max);
}
