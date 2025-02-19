using Newtonsoft.Json;

namespace LiteBot.DTOs.StableDiffusion.Responses;

public class ProgressResponseDto {
	[JsonProperty("progress")]
	public float Progress { get; set; }
	[JsonProperty("eta_relative")]
	public float EtaRelative { get; set; }
	[JsonProperty("state")]
	public StateResponseDto State { get; set; } = default!;
	[JsonProperty("current_image")]
	public string? CurrentImage { get; set; }
	[JsonProperty("textinfo")]
	public string? Textinfo { get; set; }
}
