namespace LiteBot.Interfaces;

public interface IStableDiffusionQueue {
	void Enqueue(IStableDiffusionUserRequest request);
}
