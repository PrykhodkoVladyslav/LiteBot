using LiteBot.DTOs;
using LiteBot.Interfaces;

namespace LiteBot.Services.CommandHandlers;

public class AuthorHandler(
	ICurrentChannelMessageService messageService
) : ICommandHandler {

	public async Task HandleCommandAsync(CommandInfo commandInfo) {
		await messageService.SendReplyMessageAsync("<@!883836608963555339> light2night");
	}
}
