using Discord;
using Discord.Rest;
using Discord.WebSocket;
using LiteBot.Interfaces;

namespace LiteBot.Services.StableDiffusionUserRequests;

public class ResetPropertyRequest(
	ISocketMessageAccessor socketMessageAccessor,
	IStableDiffusionUserSettingsAccessor propertyAccessor
) : IStableDiffusionUserRequest {

	private readonly SocketMessage _socketMessage = socketMessageAccessor.GetRequiredSocketMessage();

	public async Task ExucuteAsync() {
		propertyAccessor.SetDefaultValues();

		MessageReference messageReference = new MessageReference(_socketMessage.Id, _socketMessage.Channel.Id);
		await SendMessageAsync("Default values are set", messageReference);
	}

	private Task<RestUserMessage> SendMessageAsync(string message, MessageReference? messageReference = null) {
		return _socketMessage.Channel.SendMessageAsync(message, messageReference: messageReference);
	}
}
