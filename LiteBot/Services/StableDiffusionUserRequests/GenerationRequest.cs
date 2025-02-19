using Discord;
using Discord.Rest;
using Discord.WebSocket;
using LiteBot.DTOs.StableDiffusion.Requests;
using LiteBot.DTOs.StableDiffusion.Responses;
using LiteBot.Interfaces;

namespace LiteBot.Services.StableDiffusionUserRequests;

public class GenerationRequest(
	ISocketMessageAccessor socketMessageAccessor,
	IStableDiffusionApi api,
	IStableDiffusionUserSettingsAccessor propertyAccessor
) : IStableDiffusionUserRequest {

	private readonly SocketMessage _socketMessage = socketMessageAccessor.GetRequiredSocketMessage();

	public async Task ExucuteAsync() {
		MessageReference messageReference = new MessageReference(_socketMessage.Id, _socketMessage.Channel.Id);
		RestUserMessage restUserMessage = await SendMessageAsync("Generation started...", messageReference);

		await GenerateAndShowImageAsync(restUserMessage);
	}

	private async Task GenerateAndShowImageAsync(RestUserMessage restUserMessage) {
		Txt2ImgRequestDto properties = propertyAccessor.GetSettings();

		var imagesGenerationTask = api.GenerateImagesAsync(properties);

		var cts = new CancellationTokenSource();
		var previewTask = ShowPreviewImagesWhileNotCompletedAsync(imagesGenerationTask, restUserMessage, cts.Token);

		var response = await imagesGenerationTask;

		cts.Cancel();
		try {
			await previewTask;
		}
		catch (TaskCanceledException) { }

		var streams = response.Images
			.Select(Base64ToMemoryStream)
			.ToArray();

		try {
			await restUserMessage.ModifyAsync(m => {
				m.Content = string.Empty;
				m.Attachments = ImagesToAttachments(streams).ToList();
				//m.Components = CreateComponentBuilder().Build();
			});
		}
		catch {
			foreach (var stream in streams) {
				stream.Dispose();
			}

			throw;
		}
	}

	private async Task ShowPreviewImagesWhileNotCompletedAsync(Task imagesGenerationTask, RestUserMessage restUserMessage, CancellationToken cancellationToken = default) {
		while (!imagesGenerationTask.IsCompleted) {
			await Task.Delay(5000, cancellationToken);

			ProgressResponseDto progress = await api.GetProgressAsync(cancellationToken);
			if (string.IsNullOrEmpty(progress.CurrentImage))
				continue;

			using MemoryStream image = Base64ToMemoryStream(progress.CurrentImage);

			await restUserMessage.ModifyAsync(m => {
				m.Content = $"Progress: {progress.State.SamplingStep}/{progress.State.SamplingSteps}";
				m.Attachments = new List<FileAttachment> { new FileAttachment(image, "image.png") };
			});
		}
	}

	private MemoryStream Base64ToMemoryStream(string imageInBase64)
		=> new MemoryStream(Convert.FromBase64String(imageInBase64));

	private IEnumerable<FileAttachment> ImagesToAttachments(IEnumerable<Stream> images) {
		return images.Select(stream => new FileAttachment(stream, "image.png"));
	}

	private ComponentBuilder CreateComponentBuilder() {
		ComponentBuilder builder = new ComponentBuilder()
			.WithButton("1", "sd 1")
			.WithButton("2", "sd 2");

		return builder;
	}

	private Task<RestUserMessage> SendMessageAsync(string message, MessageReference? messageReference = null) {
		return _socketMessage.Channel.SendMessageAsync(message, messageReference: messageReference);
	}
}
