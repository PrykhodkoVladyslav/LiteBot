namespace LiteBot.Interfaces;

public interface IHtmlLoader {
	Task<string> LoadAsStringAsync(string url);
}
