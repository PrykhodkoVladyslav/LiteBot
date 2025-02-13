using LiteBot.StableDiffusion.UserRequests;
using Microsoft.Extensions.Logging;

namespace LiteBot.StableDiffusion;

public class StableDiffusionQueue(
	ILogger<StableDiffusionQueue> logger
) {

	private Queue<Task> queue = new();
	private object queueLocker = new();

	public void Enqueue(UserRequest request) {
		Task task = new Task(() => ExecuteAction(request.Exucute));

		lock (queueLocker) {
			queue.Enqueue(task);

			if (queue.Count == 1) {
				queue.Peek().Start();
			}
		}
	}

	private void ExecuteAction(Action action) {
		try {
			action();
		}
		catch (Exception ex) {
			logger.LogError(ex, "StableDiffusionQueue.ExecuteAction");
		}
		finally {
			DequeueAndStartNext();
		}
	}

	private void DequeueAndStartNext() {
		lock (queueLocker) {
			queue.Dequeue();
			if (queue.Count > 0)
				queue.Peek().Start();
		}
	}
}
