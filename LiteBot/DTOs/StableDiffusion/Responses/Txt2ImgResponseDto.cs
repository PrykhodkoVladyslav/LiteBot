using Newtonsoft.Json;

namespace LiteBot.DTOs.StableDiffusion.Responses;

public class Txt2ImgResponseDto {
	[JsonProperty("images")]
	public IEnumerable<string> Images { get; set; } = default!;
}
