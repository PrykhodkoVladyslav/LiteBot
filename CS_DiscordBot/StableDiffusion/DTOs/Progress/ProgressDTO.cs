using Newtonsoft.Json;

namespace LiteBot.StableDiffusion.DTOs.Progress;

public class ProgressDTO {
	[JsonProperty("progress")]
	public float Progress { get; set; }
	[JsonProperty("eta_relative")]
	public float Eta_relative { get; set; }
	[JsonProperty("state")]
	public StateDTO State { get; set; }
	[JsonProperty("current_image")]
	public string? Current_image { get; set; }
	[JsonProperty("textinfo")]
	public string? Textinfo { get; set; }
}