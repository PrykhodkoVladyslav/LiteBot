namespace LiteBot.Options;

public class WhiteListOptions {
	public bool Enabled { get; set; }
	public HashSet<ulong> AllowedChannelIds { get; set; } = null!;
}
