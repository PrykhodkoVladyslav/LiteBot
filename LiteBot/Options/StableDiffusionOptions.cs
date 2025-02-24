namespace LiteBot.Options;

public class StableDiffusionOptions {
	public string ApiUrl { get; set; } = null!;

	public int ImageGenerationTimeoutInSeconds { get; set; }
}
