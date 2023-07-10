using Discord;
using Discord.WebSocket;
using Discord.Rest;
using LiteBot.StableDiffusion.DTOAccessors;
using LiteBot.StableDiffusion.DTOs.Requests;
using LiteBot.StableDiffusion.DTOs.Progress;

namespace LiteBot.StableDiffusion.UserRequests;

public class GenerationRequest : UserRequest {
	protected StableDiffusionApi api;
	protected Txt2imgAccessor propertyAccessor;

	public GenerationRequest(SocketMessage socketMessage, ApiWithProperties apiWithProperties) : base(socketMessage) {
		api = apiWithProperties.StableDiffusionApi;
		propertyAccessor = apiWithProperties.PropertyAccessor;
	}

	public override void Exucute() {
		MessageReference messageReference = new MessageReference(socketMessage.Id, socketMessage.Channel.Id);
		RestUserMessage restUserMessage = SendMessage("Generation started...", messageReference);

		try {
			GenerateAndShowImageAsync(restUserMessage).Wait();
		}
		catch (Exception ex) {
			restUserMessage.ModifyAsync(m => m.Content = $"Unexpected exception: {ex}");
		}
	}

	private async Task GenerateAndShowImageAsync(RestUserMessage restUserMessage) {
		Txt2imgRequestDTO properties = propertyAccessor.GetDTO(socketMessage.Author.Id);

		var imagesGenerationTask = api.GenerateImagesAsync(properties);

		var cts = new CancellationTokenSource();
		var previewTask = ShowPreviewImagesWhileNotCompletedAsync(cts.Token, imagesGenerationTask, restUserMessage);

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

	protected async Task ShowPreviewImagesWhileNotCompletedAsync(CancellationToken token, Task<IEnumerable<MemoryStream>> imagesGenerationTask, RestUserMessage restUserMessage) {
		while (!imagesGenerationTask.IsCompleted) {
			await Task.Delay(5000, token);

			ProgressDTO progress = await api.GetProgressAsync();
			using MemoryStream? image = Base64ToMemoryStream(progress.Current_image);
			if (image == null)
				continue;

			await restUserMessage.ModifyAsync(m => {
				m.Content = $"Progress: {progress.State.sampling_step}/{progress.State.sampling_steps}";
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
}
