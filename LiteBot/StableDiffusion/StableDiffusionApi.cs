using System.Net;
using System.Text.Json;
using System.Text;
using Newtonsoft.Json;
using LiteBot.DTOs.StableDiffusion.Requests;
using LiteBot.DTOs.StableDiffusion.Responses;

namespace LiteBot.StableDiffusion;

public class StableDiffusionApi {
	private string url = "http://127.0.0.1:7860/sdapi/v1/";
	public StableDiffusionApi() { }

	public async Task<IEnumerable<MemoryStream>> GenerateImagesAsync(Txt2ImgRequestDto postDTO) {
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "txt2img");
		request.Timeout = 3_600_000; // Timeout.Infinite
		request.Method = "POST";
		request.ContentType = "application/json";

		byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(postDTO));
		request.ContentLength = data.Length;
		using (Stream stream = await request.GetRequestStreamAsync()) {
			stream.Write(data, 0, data.Length);
		}

		using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync()) {
			JsonElement images = System.Text.Json.JsonSerializer
				.Deserialize<JsonElement>(await GetJsonFromHttpWebResponseAsync(response))
				.GetProperty("images");

			return images.EnumerateArray()
				.Select(item => Convert.FromBase64String(
					item.GetString() ?? throw new NullReferenceException("item.GetString() is null"))
				)
				.Select(bytes => {
					MemoryStream stream = new();
					stream.Write(bytes, 0, bytes.Length);
					return stream;
				});
		}
	}

	public async Task<ProgressResponseDto> GetProgressAsync() {
		using HttpClient client = new HttpClient();
		HttpResponseMessage response = await client.GetAsync(url + "progress");

		if (!response.IsSuccessStatusCode)
			throw new Exception("Response with not success status code");

		string responseContent = await response.Content.ReadAsStringAsync();

		return JsonConvert.DeserializeObject<ProgressResponseDto>(responseContent)
			?? throw new NullReferenceException("StableDiffusionApi.GetProgressAsync");
	}

	private async Task<string> GetJsonFromHttpWebResponseAsync(HttpWebResponse response) {
		using (Stream responseStream = response.GetResponseStream())
		using (StreamReader reader = new StreamReader(responseStream)) {
			return await reader.ReadToEndAsync();
		}
	}
}