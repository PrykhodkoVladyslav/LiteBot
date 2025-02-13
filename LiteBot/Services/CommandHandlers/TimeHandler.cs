using Discord;
using LiteBot.DTOs;
using LiteBot.Interfaces;

namespace LiteBot.Services.CommandHandlers;

public class TimeHandler(
	ISocketMessageAccessor socketMessageAccessor
) : ICommandHandler {

	public Task HandleCommandAsync(CommandInfo commandInfo) {
		SendMessage($"{DateTime.Now.ToLongTimeString()}\n{DateTime.Now.ToLongDateString()}");
		if (DateTime.Now.Hour < 8)
			SendMessage("Іншими словами час спати");

		return Task.CompletedTask;
	}

	private void SendMessage(string message, MessageReference? messageReference = null) {
		socketMessageAccessor.GetRequiredSocketMessage()
			.Channel
			.SendMessageAsync(message, messageReference: messageReference)
			.Wait();
	}
}
