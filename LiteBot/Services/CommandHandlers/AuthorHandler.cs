using Discord;
using LiteBot.DTOs;
using LiteBot.Interfaces;

namespace LiteBot.Services.CommandHandlers;

public class AuthorHandler(
	ISocketMessageAccessor socketMessageAccessor
) : ICommandHandler {

	public Task HandleCommandAsync(CommandInfo commandInfo) {
		SendMessage("<@!883836608963555339> Lite#5625");

		return Task.CompletedTask;
	}

	private void SendMessage(string message, MessageReference? messageReference = null) {
		socketMessageAccessor.GetRequiredSocketMessage()
			.Channel
			.SendMessageAsync(message, messageReference: messageReference)
			.Wait();
	}
}
