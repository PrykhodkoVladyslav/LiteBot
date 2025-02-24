using LiteBot.DTOs;
using LiteBot.Interfaces;

namespace LiteBot.Services.CommandHandlers;

public class HelpHandler(
	ICurrentChannelMessageService messageService
) : ICommandHandler {

	public Task HandleCommandAsync(CommandInfo commandInfo) {
		return messageService.SendMessageAsync("""
			Доступні команди:
				Корисні:
					`sd`
					`art`/`арт`
					`rand`/`random`/`рандом`
				Менш корисні:
					`author`/`автор`
					`time`/`час`
			""");
	}
}
