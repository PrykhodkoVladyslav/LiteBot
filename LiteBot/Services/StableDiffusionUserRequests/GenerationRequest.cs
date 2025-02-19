using Discord;
using Discord.WebSocket;
using Discord.Rest;
using LiteBot.Interfaces;
using LiteBot.StableDiffusion;
using LiteBot.DTOs.StableDiffusion.Requests;
using LiteBot.DTOs.StableDiffusion.Responses;

namespace LiteBot.Services.StableDiffusionUserRequests;

public class GenerationRequest(
	ISocketMessageAccessor socketMessageAccessor,
	StableDiffusionApi api,
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

		IEnumerable<MemoryStream> imagesList = await imagesGenerationTask;

		cts.Cancel();
		try {
			await previewTask;
		}
		catch (TaskCanceledException) { }

		await restUserMessage.ModifyAsync(m => {
			m.Content = "";
			m.Attachments = ImagesToAttachments(imagesList).ToList();
			//m.Components = CreateComponentBuilder().Build();
		});

		imagesList.ToList().ForEach(image => image.Dispose());
	}

	protected async Task ShowPreviewImagesWhileNotCompletedAsync(Task<IEnumerable<MemoryStream>> imagesGenerationTask, RestUserMessage restUserMessage, CancellationToken cancellationToken = default) {
		while (!imagesGenerationTask.IsCompleted) {
			await Task.Delay(5000, cancellationToken);

			ProgressResponseDto progress = await api.GetProgressAsync();
			using MemoryStream? image = Base64ToMemoryStream(progress.CurrentImage);
			if (image == null)
				continue;

			await restUserMessage.ModifyAsync(m => {
				m.Content = $"Progress: {progress.State.SamplingStep}/{progress.State.SamplingSteps}";
				m.Attachments = new List<FileAttachment> { new FileAttachment(image, "image.png") };
			});
		}
	}

	protected MemoryStream? Base64ToMemoryStream(string? imageInBase64)
		=> string.IsNullOrEmpty(imageInBase64) ? null : new MemoryStream(Convert.FromBase64String(imageInBase64));

	protected IEnumerable<FileAttachment> ImagesToAttachments(IEnumerable<MemoryStream> images) {
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
