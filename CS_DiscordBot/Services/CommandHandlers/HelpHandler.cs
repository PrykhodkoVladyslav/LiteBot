using Discord;
using LiteBot.DTOs;
using LiteBot.Interfaces;

namespace LiteBot.Services.CommandHandlers;

public class HelpHandler(
	ISocketMessageAccessor socketMessageAccessor
) : ICommandHandler {

	public Task HandleCommandAsync(CommandInfo commandInfo) {
		SendMessage("""
			Доступні команди:
				Корисні:
					`sd`
					`art`/`арт`
					`rand`/`random`/`рандом`
				Менш корисні:
					`author`/`автор`
					`time`/`час`
			""");

		return Task.CompletedTask;
	}

	private void SendMessage(string message, MessageReference? messageReference = null) {
		socketMessageAccessor.GetRequiredSocketMessage()
			.Channel
			.SendMessageAsync(message, messageReference: messageReference)
			.Wait();
	}
}
