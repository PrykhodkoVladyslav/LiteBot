using Discord;
using Discord.Rest;
using Discord.WebSocket;
using LiteBot.Interfaces;
using LiteBot.StableDiffusion.DTOAccessors;

namespace LiteBot.StableDiffusion.UserRequests;

public class ResetPropertyRequest(
	ISocketMessageAccessor socketMessageAccessor,
	Txt2imgAccessor propertyAccessor
) : IStableDiffusionUserRequest {

	private readonly SocketMessage _socketMessage = socketMessageAccessor.GetRequiredSocketMessage();

	public async Task ExucuteAsync() {
		propertyAccessor.SetDefaultValues(_socketMessage.Author.Id);

		MessageReference messageReference = new MessageReference(_socketMessage.Id, _socketMessage.Channel.Id);
		await SendMessageAsync("Default values are set", messageReference);
	}

	private Task<RestUserMessage> SendMessageAsync(string message, MessageReference? messageReference = null) {
		return _socketMessage.Channel.SendMessageAsync(message, messageReference: messageReference);
	}
}
