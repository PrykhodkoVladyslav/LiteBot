using Newtonsoft.Json;

namespace LiteBot.DTOs.StableDiffusion.Responses;

public class StateResponseDto {
	[JsonProperty("skipped")]
	public bool Skipped { get; set; }
	[JsonProperty("interrupted")]
	public bool Interrupted { get; set; }
	[JsonProperty("job")]
	public string Job { get; set; } = default!;
	[JsonProperty("job_count")]
	public int JobCount { get; set; }
	[JsonProperty("job_timestamp")]
	public string JobTimestamp { get; set; } = default!;
	[JsonProperty("job_no")]
	public int JobNo { get; set; }
	[JsonProperty("sampling_step")]
	public int SamplingStep { get; set; }
	[JsonProperty("sampling_steps")]
	public int SamplingSteps { get; set; }
}
