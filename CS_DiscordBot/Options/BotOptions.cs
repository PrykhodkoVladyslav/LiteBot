namespace LiteBot.Options;

public class BotOptions {
	public string Token { get; set; } = null!;

	public string Prefix { get; set; } = null!;

	public WhiteListOptions WhiteList { get; set; } = null!;

	public bool IgnoreMessagesFromBots { get; set; }

	public bool LogReceivedMessages { get; set; }
}
