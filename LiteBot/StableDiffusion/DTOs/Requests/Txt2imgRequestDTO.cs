using Newtonsoft.Json;

namespace LiteBot.StableDiffusion.DTOs.Requests;

public class Txt2imgRequestDTO {
	[JsonProperty("enable_hr")]
	public bool Enable_hr { get; set; } = false;
	[JsonProperty("denoising_strength")]
	public float Denoising_strength { get; set; } = 0;
	[JsonProperty("firstphase_width")]
	public float Firstphase_width { get; set; } = 0;
	[JsonProperty("firstphase_height")]
	public float Firstphase_height { get; set; } = 0;
	[JsonProperty("hr_scale")]
	public float Hr_scale { get; set; } = 2;
	[JsonProperty("hr_upscaler")]
	public string Hr_upscaler { get; set; } = "";
	[JsonProperty("hr_second_pass_steps")]
	public float Hr_second_pass_steps { get; set; } = 0;
	[JsonProperty("hr_resize_x")]
	public float Hr_resize_x { get; set; } = 0;
	[JsonProperty("hr_resize_y")]
	public float Hr_resize_y { get; set; } = 0;
	[JsonProperty("hr_sampler_name")]
	public string Hr_sampler_name { get; set; } = "";
	[JsonProperty("hr_prompt")]
	public string Hr_prompt { get; set; } = "";
	[JsonProperty("hr_negative_prompt")]
	public string Hr_negative_prompt { get; set; } = "";
	[JsonProperty("prompt")]
	public string Prompt { get; set; } = "";
	[JsonProperty("styles")]
	public List<string> Styles { get; set; } = new();
	[JsonProperty("seed")]
	public int Seed { get; set; } = -1;
	[JsonProperty("subseed")]
	public int Subseed { get; set; } = -1;
	[JsonProperty("subseed_strength")]
	public float Subseed_strength { get; set; } = 0;
	[JsonProperty("seed_resize_from_h")]
	public int Seed_resize_from_h { get; set; } = -1;
	[JsonProperty("seed_resize_from_w")]
	public int Seed_resize_from_w { get; set; } = -1;
	[JsonProperty("sampler_name")]
	public string Sampler_name { get; set; } = "";
	[JsonProperty("batch_size")]
	public int Batch_size { get; set; } = 1;
	[JsonProperty("n_iter")]
	public float N_iter { get; set; } = 1;
	[JsonProperty("steps")]
	public int Steps { get; set; } = 20;
	[JsonProperty("cfg_scale")]
	public float Cfg_scale { get; set; } = 7;
	[JsonProperty("width")]
	public int Width { get; set; } = 512;
	[JsonProperty("height")]
	public int Height { get; set; } = 512;
	[JsonProperty("restore_faces")]
	public bool Restore_faces { get; set; } = false;
	[JsonProperty("tiling")]
	public bool Tiling { get; set; } = false;
	[JsonProperty("do_not_save_samples")]
	public bool Do_not_save_samples { get; set; } = false;
	[JsonProperty("do_not_save_grid")]
	public bool Do_not_save_grid { get; set; } = false;
	[JsonProperty("negative_prompt")]
	public string Negative_prompt { get; set; } = "";
	[JsonProperty("eta")]
	public float Eta { get; set; } = 0;
	[JsonProperty("s_min_uncond")]
	public float S_min_uncond { get; set; } = 0;
	[JsonProperty("s_churn")]
	public float S_churn { get; set; } = 0;
	[JsonProperty("s_tmax")]
	public float S_tmax { get; set; } = 0;
	[JsonProperty("s_tmin")]
	public float S_tmin { get; set; } = 0;
	[JsonProperty("s_noise")]
	public float S_noise { get; set; } = 1;
	[JsonProperty("script_args")]
	public List<string> Script_args { get; set; } = new();
	[JsonProperty("sampler_index")]
	public string Sampler_index { get; set; } = "Euler";
	[JsonProperty("script_name")]
	public string Script_name { get; set; } = "";
	[JsonProperty("send_images")]
	public bool Send_images { get; set; } = true;
	[JsonProperty("save_images")]
	public bool Save_images { get; set; } = false;
}
