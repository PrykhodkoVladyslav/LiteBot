using LiteBot.DTOs.StableDiffusion.Requests;
using LiteBot.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LiteBot.Services;

public class StableDiffusionUserSettingsAccessor(
	ICurrentUserService currentUser
) : IStableDiffusionUserSettingsAccessor {

	private readonly string folderPath = "UsersProperties";
	private static readonly object jsonLocker = new();
	private readonly string defaultFilePath = "default.json";
	private readonly ulong _userId = currentUser.GetId();

	public Txt2ImgRequestDto GetSettings() {
		string filePath = CreatePathToFile(_userId);

		lock (jsonLocker) {
			if (!File.Exists(filePath))
				filePath = GetDefaultPath();

			return JsonConvert.DeserializeObject<Txt2ImgRequestDto>(File.ReadAllText(filePath))
				?? throw new NullReferenceException($"{nameof(StableDiffusionUserSettingsAccessor)}.{nameof(GetSettings)}");
		}
	}

	public void SetSettings(Txt2ImgRequestDto dto) {
		lock (jsonLocker) {
			CreateFolder();

			File.WriteAllText(CreatePathToFile(_userId), JsonConvert.SerializeObject(dto));
		}
	}

	public void SetProperty(string property, object value) {
		string filePath = CreatePathToFile(_userId);

		if (!File.Exists(filePath))
			SetDefaultValues();

		JObject obj = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(filePath))
			?? throw new NullReferenceException($"{nameof(StableDiffusionUserSettingsAccessor)}.{nameof(SetProperty)}");

		obj[property] = JToken.FromObject(value);

		lock (jsonLocker) {
			File.WriteAllText(filePath, JsonConvert.SerializeObject(obj, Formatting.Indented));
		}
	}

	public void SetDefaultValues() {
		lock (jsonLocker) {
			CreateFolder();

			File.WriteAllText(CreatePathToFile(_userId), File.ReadAllText(GetDefaultPath()));
		}
	}

	private string CreatePathToFile(ulong userId) {
		return $"{folderPath}\\{userId}.json";
	}
	private string GetDefaultPath() {
		return $"{folderPath}\\{defaultFilePath}";
	}

	private void CreateFolder() {
		if (!Directory.Exists(folderPath))
			Directory.CreateDirectory(folderPath);
	}
}
