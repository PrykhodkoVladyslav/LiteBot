using Newtonsoft.Json;

namespace LiteBot.StableDiffusion.DTOs.Progress;

public class StateDTO {
	[JsonProperty("skipped")]
	public bool skipped { get; set; }
	[JsonProperty("interrupted")]
	public bool interrupted { get; set; }
	[JsonProperty("job")]
	public string job { get; set; }
	[JsonProperty("job_count")]
	public int job_count { get; set; }
	[JsonProperty("job_timestamp")]
	public string job_timestamp { get; set; }
	[JsonProperty("job_no")]
	public int job_no { get; set; }
	[JsonProperty("sampling_step")]
	public int sampling_step { get; set; }
	[JsonProperty("sampling_steps")]
	public int sampling_steps { get; set; }
}