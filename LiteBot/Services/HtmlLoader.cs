using LiteBot.Interfaces;

namespace LiteBot.Services;

public class HtmlLoader : IHtmlLoader {
	public async Task<string> LoadAsStringAsync(string url) {
		using var client = new HttpClient();
		return await client.GetStringAsync(url);
	}
}
