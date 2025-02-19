namespace LiteBot.Interfaces;

public interface IImageFromApiLoader {
	Task<string> GetImageUrlByRegexAsync(string apiUrl, string regexPattern);
}
