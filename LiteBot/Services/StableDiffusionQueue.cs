using LiteBot.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace LiteBot.Services;

public class StableDiffusionQueue(
	ILogger<StableDiffusionQueue> logger
) : IStableDiffusionQueue {

	private readonly ConcurrentQueue<IStableDiffusionUserRequest> queue = new();
	private readonly SemaphoreSlim semaphore = new(1, 1);

	public void Enqueue(IStableDiffusionUserRequest request) {
		queue.Enqueue(request);
		_ = ProcessQueueAsync();
	}

	private async Task ProcessQueueAsync() {
		if (!semaphore.Wait(0))
			return;

		try {
			while (queue.TryDequeue(out var request)) {
				try {
					await request.ExucuteAsync();
				}
				catch (Exception ex) {
					logger.LogError(ex, $"{nameof(StableDiffusionQueue)}.{nameof(IStableDiffusionUserRequest.ExucuteAsync)}");
				}
			}
		}
		finally {
			semaphore.Release();
		}
	}
}
