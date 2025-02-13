using LiteBot.StableDiffusion.DTOs.Requests;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LiteBot.StableDiffusion.DTOAccessors;

public class Txt2imgAccessor {
	protected string folderPath = "UsersProperties";
	protected object jsonLocker = new();
	private readonly string defaultFilePath = "default.json";

	public Txt2imgAccessor() {
		if (!Directory.Exists(folderPath))
			Directory.CreateDirectory(folderPath);
	}

	public Txt2imgRequestDTO GetDTO(ulong authorId) {
		string filePath = CreatePathToFile(authorId);

		lock (jsonLocker) {
			if (!File.Exists(filePath))
				filePath = GetDefaultPath();

			return JsonConvert.DeserializeObject<Txt2imgRequestDTO>(File.ReadAllText(filePath))
				?? throw new NullReferenceException("Txt2imgAccessor.GetDTO");
		}
	}

	public void SaveDTO(ulong authorId, Txt2imgRequestDTO dto) {
		lock (jsonLocker) {
			File.WriteAllText(CreatePathToFile(authorId), JsonConvert.SerializeObject(dto));
		}
	}

	public void SetProperty(ulong authorId, string property, object value) {
		string filePath = CreatePathToFile(authorId);

		if (!File.Exists(filePath))
			SetDefaultValues(authorId);

		JObject obj = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(filePath))
			?? throw new NullReferenceException("Txt2imgAccessor.SetProperty");

		obj[property] = JToken.FromObject(value);

		lock (jsonLocker) {
			File.WriteAllText(filePath, JsonConvert.SerializeObject(obj));
		}
	}

	public void SetDefaultValues(ulong authorId) {
		lock (jsonLocker) {
			File.WriteAllText(CreatePathToFile(authorId), File.ReadAllText(GetDefaultPath()));
		}
	}

	protected string CreatePathToFile(ulong authorId) {
		return $"{folderPath}\\{authorId}.json";
	}
	protected string GetDefaultPath() {
		return $"{folderPath}\\{defaultFilePath}";
	}
}
