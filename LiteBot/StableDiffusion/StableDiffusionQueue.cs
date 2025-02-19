using LiteBot.StableDiffusion.UserRequests;
using Microsoft.Extensions.Logging;

namespace LiteBot.StableDiffusion;

public class StableDiffusionQueue(
	ILogger<StableDiffusionQueue> logger
) {

	private Queue<Task> queue = new();
	private object queueLocker = new();

	public void Enqueue(IStableDiffusionUserRequest request) {
		Task task = new Task(() => _ = ExecuteActionAsync(request.ExucuteAsync));

		lock (queueLocker) {
			queue.Enqueue(task);

			if (queue.Count == 1) {
				queue.Peek().Start();
			}
		}
	}

	private async Task ExecuteActionAsync(Func<Task> action) {
		try {
			await action();
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
