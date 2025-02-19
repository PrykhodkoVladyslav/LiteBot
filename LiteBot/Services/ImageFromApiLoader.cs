using LiteBot.Interfaces;
using System.Text.RegularExpressions;

namespace LiteBot.Services;

public class ImageFromApiLoader(
	IHtmlLoader htmlLoader
) : IImageFromApiLoader {

	public async Task<string> GetImageUrlByRegexAsync(string apiUrl, string regexPattern) {
		string htmlText = await htmlLoader.LoadAsStringAsync(apiUrl);

		Regex regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
		Match match = regex.Match(htmlText);

		return match.Groups[1].Value;
	}
}
