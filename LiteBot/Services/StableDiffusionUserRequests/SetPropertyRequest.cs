using Discord;
using Discord.Rest;
using Discord.WebSocket;
using LiteBot.Interfaces;
using LiteBot.StableDiffusion.DTOAccessors;

namespace LiteBot.Services.StableDiffusionUserRequests;

public class SetPropertyRequest(
	ISocketMessageAccessor socketMessageAccessor,
	Txt2imgAccessor propertyAccessor
) : IStableDiffusionUserRequest {

	private readonly SocketMessage _socketMessage = socketMessageAccessor.GetRequiredSocketMessage();

	public string? Property { private get; set; }
	public object? Value { private get; set; }

	public async Task ExucuteAsync() {
		ArgumentNullException.ThrowIfNull(Property);
		ArgumentNullException.ThrowIfNull(Value);

		propertyAccessor.SetProperty(_socketMessage.Author.Id, Property, Value);

		MessageReference messageReference = new MessageReference(_socketMessage.Id, _socketMessage.Channel.Id);
		await SendMessageAsync($"A new property value set to: {Property}", messageReference);
	}

	private Task<RestUserMessage> SendMessageAsync(string message, MessageReference? messageReference = null) {
		return _socketMessage.Channel.SendMessageAsync(message, messageReference: messageReference);
	}
}
