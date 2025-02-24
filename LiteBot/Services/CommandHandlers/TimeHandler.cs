using LiteBot.DTOs;
using LiteBot.Interfaces;

namespace LiteBot.Services.CommandHandlers;

public class TimeHandler(
	ICurrentChannelMessageService messageService
) : ICommandHandler {

	public async Task HandleCommandAsync(CommandInfo commandInfo) {
		await messageService.SendMessageAsync($"{DateTime.Now.ToLongTimeString()}\n{DateTime.Now.ToLongDateString()}");
		if (DateTime.Now.Hour < 8)
			await messageService.SendMessageAsync("Іншими словами час спати");
	}
}
