using Discord.Rest;
using LiteBot.DTOs;
using LiteBot.Exceptions;
using LiteBot.Interfaces;
using System.Text.RegularExpressions;

namespace LiteBot.Services.CommandHandlers;

public partial class RandomHandler(
	ICurrentChannelMessageService messageService,
	ISingletonRandom singletonRandom
) : ICommandHandler {

	public async Task HandleCommandAsync(CommandInfo commandInfo) {
		if (commandInfo.Argument == string.Empty) {
			await messageService.SendMessageAsync(singletonRandom.Next().ToString());
		}
		else if (commandInfo.Argument == "?") {
			await HelpMessageAsync();
		}
		else if (IsRandRange(commandInfo.Argument, out int first, out int second)) {
			if (first > second) {
				await messageService.SendMessageAsync("Некоректний діапазон");
			}

			var number = singletonRandom.Next(first, second + 1);

			await messageService.SendMessageAsync(number.ToString());
		}
		else {
			throw new UnknownCommandException();
		}
	}

	private Task<RestUserMessage> HelpMessageAsync() {
		return messageService.SendMessageAsync("""
			Доступні команди:
				`"Немає аргументів"` - надсилає випадкове число
				`"число"-"число"` - надсилає число в заданому діапазоні
			""");
	}

	private static bool IsRandRange(string argument, out int first, out int second) {
		first = second = 0;

		if (!NumbersRange().IsMatch(argument)) {
			return false;
		}

		var groups = NumbersRange().Match(argument).Groups;

		if (!int.TryParse(groups[1].Value, out first) || !int.TryParse(groups[2].Value, out second))
			return false;

		return true;
	}

	[GeneratedRegex(@"^(-?\d+)-(-?\d+)$")]
	private static partial Regex NumbersRange();
}
