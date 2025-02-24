using Discord;
using Discord.Rest;

namespace LiteBot.Interfaces;

public interface ICurrentChannelMessageService {
	Task<RestUserMessage> SendMessageAsync(string message, ulong? messageId = null);

	Task<RestUserMessage> SendReplyMessageAsync(string message);

	Task AddReactionAsync(Emoji emoji);
}
