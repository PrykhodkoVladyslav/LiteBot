using Discord;
using Discord.Rest;
using Discord.WebSocket;
using LiteBot.Interfaces;

namespace LiteBot.Services;

public class CurrentChannelMessageService(
	ISocketMessageAccessor socketMessageAccessor
) : ICurrentChannelMessageService {

	private readonly SocketMessage _socketMessage = socketMessageAccessor.GetRequiredSocketMessage();

	public Task<RestUserMessage> SendMessageAsync(string message, ulong? messageId = null) {
		return _socketMessage.Channel.SendMessageAsync(message, messageReference: TryBuildMessageReference(messageId));
	}

	public Task<RestUserMessage> SendReplyMessageAsync(string message) {
		return SendMessageAsync(message, _socketMessage.Id);
	}

	public Task AddReactionAsync(Emoji emoji) {
		return _socketMessage.AddReactionAsync(emoji);
	}

	private MessageReference? TryBuildMessageReference(ulong? messageId) {
		if (!messageId.HasValue)
			return null;

		return new MessageReference(messageId.Value, _socketMessage.Channel.Id);
	}
}
