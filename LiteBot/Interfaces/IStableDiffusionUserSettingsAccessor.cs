using LiteBot.DTOs.StableDiffusion.Requests;

namespace LiteBot.Interfaces;

public interface IStableDiffusionUserSettingsAccessor {
	Txt2ImgRequestDto GetSettings();

	void SetSettings(Txt2ImgRequestDto dto);

	void SetDefaultValues();

	void SetProperty(string property, object value);
}
