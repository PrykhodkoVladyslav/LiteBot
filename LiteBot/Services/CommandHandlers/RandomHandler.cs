namespace LiteBot.Services.CommandHandlers;

using Discord;
using LiteBot.DTOs;
using LiteBot.Exceptions;
using LiteBot.Interfaces;
using System.Threading.Tasks;

public class RandomHandler(
	ISocketMessageAccessor socketMessageAccessor
) : ICommandHandler {

	private Random random = new Random(DateTime.Now.Millisecond);

	public Task HandleCommandAsync(CommandInfo commandInfo) {
		if (commandInfo.Argument == string.Empty) {
			SendMessage(random.Next().ToString());
		}
		else if (commandInfo.Argument == "?") {
			HelpMessage();
		}
		else if (IsRandRange(commandInfo.Argument, out uint first, out uint second)) {
			if (first > second) {
				SendMessage("Некоректний діапазон");
				return Task.CompletedTask;
			}

			SendMessage(new Random(DateTime.Now.Millisecond).Next((int)first, (int)second + 1).ToString());
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

	private bool IsRandRange(string argument, out uint first, out uint second) {
		first = second = 0;

		string[] arguments = argument.Split("-");
		if (arguments.Length != 2)
			return false;

		if (!TypeChecker.IsUInt32(arguments[0]) && TypeChecker.IsUInt32(arguments[1]))
			return false;

		first = Convert.ToUInt32(arguments[0]);
		second = Convert.ToUInt32(arguments[1]);
		return true;
	}

	private void SendMessage(string message, MessageReference? messageReference = null) {
		socketMessageAccessor.GetRequiredSocketMessage()
			.Channel
			.SendMessageAsync(message, messageReference: messageReference)
			.Wait();
	}
}
