namespace LiteBot.Services.CommandHandlers;

using Discord;
using LiteBot.DTOs;
using LiteBot.Exceptions;
using LiteBot.Interfaces;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public partial class RandomHandler(
	ISocketMessageAccessor socketMessageAccessor,
	ISingletonRandom singletonRandom
) : ICommandHandler {

	public Task HandleCommandAsync(CommandInfo commandInfo) {
		if (commandInfo.Argument == string.Empty) {
			SendMessage(singletonRandom.Next().ToString());
		}
		else if (commandInfo.Argument == "?") {
			HelpMessage();
		}
		else if (IsRandRange(commandInfo.Argument, out int first, out int second)) {
			if (first > second) {
				SendMessage("Некоректний діапазон");
				return Task.CompletedTask;
			}

			var number = singletonRandom.Next(first, second + 1);

			SendMessage(number.ToString());
		}
		else {
			throw new UnknownCommandException();
		}
		return Task.CompletedTask;
	}

	private void HelpMessage() {
		SendMessage(
			"Доступні команди:\n" +
				"	\"Немає аргументів\" - надсилає випадкове число\n" +
				"	\"число\"-\"число\" - надсилає число в заданому діапазоні"
		);
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

	private void SendMessage(string message, MessageReference? messageReference = null) {
		socketMessageAccessor.GetRequiredSocketMessage()
			.Channel
			.SendMessageAsync(message, messageReference: messageReference)
			.Wait();
	}

	[GeneratedRegex(@"^(-?\d+)-(-?\d+)$")]
	private static partial Regex NumbersRange();
}
