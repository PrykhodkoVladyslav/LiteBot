using LiteBot.DTOs.StableDiffusion.Requests;
using LiteBot.DTOs.StableDiffusion.Responses;
using LiteBot.Interfaces;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace LiteBot.Services;

public class StableDiffusionApi : IStableDiffusionApi {
	private string url = "http://127.0.0.1:7860/sdapi/v1/";

	public async Task<Txt2ImgResponseDto> GenerateImagesAsync(Txt2ImgRequestDto postDto, CancellationToken cancellationToken = default) {
		using HttpClient client = new();
		client.Timeout = TimeSpan.FromSeconds(900);
		var httpResponseMessage = await client.PostAsJsonAsync($"{url}txt2img", postDto, cancellationToken);

		var stringResponse = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);

		return JsonConvert.DeserializeObject<Txt2ImgResponseDto>(stringResponse)
			?? throw new NullReferenceException($"{nameof(StableDiffusionApi)}.{nameof(GenerateImagesAsync)}");
	}

	public async Task<ProgressResponseDto> GetProgressAsync(CancellationToken cancellationToken = default) {
		using HttpClient client = new();
		string responseContent = await client.GetStringAsync($"{url}progress", cancellationToken);

		return JsonConvert.DeserializeObject<ProgressResponseDto>(responseContent)
			?? throw new NullReferenceException($"{nameof(StableDiffusionApi)}.{nameof(GetProgressAsync)}");
	}
}
