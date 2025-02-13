using LiteBot.DTOs;
using LiteBot.Exceptions;
using LiteBot.Interfaces;
using LiteBot.Options;
using LiteBot.Services.CommandHandlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LiteBot;

public class CommandController(
	IServiceProvider serviceProvider,
	IOptions<BotOptions> botOptions,
	ISocketMessageAccessor socketMessageAccessor
) {

	private readonly BotOptions _botOptions = botOptions.Value;

	public async Task HandleCommandAsync(CommandInfo command) {
		try {
			var commandHandler = GetCommandHandlerByCommandName(command.Name);

			await commandHandler.HandleCommandAsync(command);
		}
		catch (UnknownCommandException) {
			await socketMessageAccessor.GetRequiredSocketMessage()
				.Channel
				.SendMessageAsync($"Невідома команда, для детальнішої інформації про команди спробуйте \"{_botOptions.Prefix}?\"");
		}
	}

	private ICommandHandler GetCommandHandlerByCommandName(string commandName) {
		if (commandName == "?" || commandName == "help") {
			return serviceProvider.GetRequiredService<HelpHandler>();
		}
		else if (commandName == "sd") {
			return serviceProvider.GetRequiredService<StableDiffusionHandler>();
		}
		else if (commandName == "time" || commandName == "час") {
			return serviceProvider.GetRequiredService<TimeHandler>();
		}
		else if (commandName == "rand" || commandName == "random" || commandName == "рандом") {
			return serviceProvider.GetRequiredService<RandomHandler>();
		}
		else if (commandName == "author" || commandName == "автор") {
			return serviceProvider.GetRequiredService<AuthorHandler>();
		}
		else if (commandName == "art" || commandName == "арт") {
			return serviceProvider.GetRequiredService<ArtHandler>();
		}

		throw new UnknownCommandException();
	}
}
