using Discord;
using Discord.Rest;
using LiteBot.DTOs.StableDiffusion.Requests;
using LiteBot.DTOs.StableDiffusion.Responses;
using LiteBot.Interfaces;

namespace LiteBot.Services.StableDiffusionUserRequests;

public class GenerationRequest(
	ICurrentChannelMessageService messageService,
	IStableDiffusionApi api,
	IStableDiffusionUserSettingsAccessor propertyAccessor
) : IStableDiffusionUserRequest {

	public async Task ExucuteAsync() {
		RestUserMessage restUserMessage = await messageService.SendReplyMessageAsync("Генерація в процесі...");

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

		await ShowResultAsync(restUserMessage, response.Images);
	}

	private async Task ShowPreviewImagesWhileNotCompletedAsync(Task imagesGenerationTask, RestUserMessage restUserMessage, CancellationToken cancellationToken = default) {
		while (!imagesGenerationTask.IsCompleted) {
			await Task.Delay(5000, cancellationToken);

			ProgressResponseDto progress = await api.GetProgressAsync(cancellationToken);
			if (string.IsNullOrEmpty(progress.CurrentImage))
				continue;

			using MemoryStream image = Base64ToMemoryStream(progress.CurrentImage);

			await restUserMessage.ModifyAsync(m => {
				m.Content = $"Прогрес: {progress.State.SamplingStep}/{progress.State.SamplingSteps}";
				m.Attachments = new FileAttachment[] { new(image, "image.png") };
			});
		}
	}

	private MemoryStream Base64ToMemoryStream(string imageInBase64)
		=> new MemoryStream(Convert.FromBase64String(imageInBase64));

	private async Task ShowResultAsync(RestUserMessage restUserMessage, IEnumerable<string> base64images) {
		var streams = base64images
			.Select(Base64ToMemoryStream)
			.ToArray();

		try {
			await restUserMessage.ModifyAsync(m => {
				m.Content = string.Empty;
				m.Attachments = ImagesToAttachments(streams).ToArray();
				//m.Components = CreateComponentBuilder().Build();
			});
		}
		finally {
			foreach (var stream in streams) {
				stream.Dispose();
			}
		}
	}

	private IEnumerable<FileAttachment> ImagesToAttachments(IEnumerable<Stream> images) {
		return images.Select(stream => new FileAttachment(stream, "image.png"));
	}

	private ComponentBuilder CreateComponentBuilder() {
		ComponentBuilder builder = new ComponentBuilder()
			.WithButton("1", "sd 1")
			.WithButton("2", "sd 2");

		return builder;
	}
}
