using LiteBot.DTOs.StableDiffusion.Requests;
using LiteBot.DTOs.StableDiffusion.Responses;
using LiteBot.Interfaces;
using LiteBot.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace LiteBot.Services;

public class StableDiffusionApi(
	IOptions<StableDiffusionOptions> stableDiffusionOptions
) : IStableDiffusionApi {

	private readonly StableDiffusionOptions _stableDiffusionOptions = stableDiffusionOptions.Value;

	public async Task<Txt2ImgResponseDto> GenerateImagesAsync(Txt2ImgRequestDto postDto, CancellationToken cancellationToken = default) {
		using HttpClient client = new();
		client.Timeout = TimeSpan.FromSeconds(_stableDiffusionOptions.ImageGenerationTimeoutInSeconds);
		var httpResponseMessage = await client.PostAsJsonAsync($"{_stableDiffusionOptions.ApiUrl}txt2img", postDto, cancellationToken);

		var stringResponse = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);

		return JsonConvert.DeserializeObject<Txt2ImgResponseDto>(stringResponse)
			?? throw new NullReferenceException($"{nameof(StableDiffusionApi)}.{nameof(GenerateImagesAsync)}");
	}

	public async Task<ProgressResponseDto> GetProgressAsync(CancellationToken cancellationToken = default) {
		using HttpClient client = new();
		string responseContent = await client.GetStringAsync($"{_stableDiffusionOptions.ApiUrl}progress", cancellationToken);

		return JsonConvert.DeserializeObject<ProgressResponseDto>(responseContent)
			?? throw new NullReferenceException($"{nameof(StableDiffusionApi)}.{nameof(GetProgressAsync)}");
	}
}
