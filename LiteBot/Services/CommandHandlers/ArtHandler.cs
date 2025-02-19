using ArtApp.Web;
using Discord;
using LiteBot.DTOs;
using LiteBot.Exceptions;
using LiteBot.Interfaces;

namespace LiteBot.Services.CommandHandlers;

public class ArtHandler(
	ISocketMessageAccessor socketMessageAccessor,
	ICommandAnalizer commandAnalizer
) : ICommandHandler {

	protected readonly string apiFilePath = "api.txt";

	public Task HandleCommandAsync(CommandInfo commandInfo) {
		if (string.IsNullOrEmpty(commandInfo.Argument)) {
			DefaultAction();
		}
		else if (commandInfo.Argument == "?") {
			HelpMessage();
		}
		else if (commandInfo.Argument == "джерело") {
			SendMessage(
				"Джерела що підтримуються:\n" +
				"	https://api.waifu.pics/sfw/neko\n" +
				"	https://api.waifu.im/search/?included_tags=maid\n" +
				"	https://api.waifu.im/search/?included_tags=waifu\n" +
				"	https://api.waifu.im/search/?included_tags=marin-kitagawa\n" +
				"	https://api.waifu.im/search/?included_tags=mori-calliope\n" +
				"	https://api.waifu.im/search/?included_tags=raiden-shogun\n" +
				"	https://api.waifu.im/search/?included_tags=oppai\n" +
				"	https://api.waifu.im/search/?included_tags=selfies\n" +
				"	https://api.waifu.im/search/?included_tags=uniform\n" +
				"Можа спробувати ввести інше джерело, можливо воно буде працювати"
			);
		}
		else if (commandAnalizer.HasSubcommand(commandInfo, "джерело", out var subcommandInfo)) {
			File.WriteAllText(apiFilePath, subcommandInfo!.Argument.Trim());

			SendMessage("Джерело змінено");
		}
		else if (uint.TryParse(commandInfo.Argument, out uint numberOfPictures)) {
			if (numberOfPictures > 10) {
				SendMessage("Занадто багато зображень");
				return Task.CompletedTask;
			}

			for (uint i = 0; i < numberOfPictures; i++) {
				DefaultAction();
			}
		}
		else {
			throw new UnknownCommandException();
		}

		return Task.CompletedTask;
	}

	private void DefaultAction() {
		SendMessage(WebLoad.GetPictureUrlFromApi(File.ReadAllText(apiFilePath), "\"url\":\"([^\"]*)\""));
	}

	private void HelpMessage() {
		SendMessage(
			"Доступні команди:\n" +
				"	\"Немає аргументів\" - вивід одного арту\n" +
				"	\"число\" - для надсилання кількох артів\n" +
				"	джерело - список стандартних API\n" +
				"	джерело \"посилання\" - змінити джерело на нове"
		);
	}

	private void SendMessage(string message, MessageReference? messageReference = null) {
		socketMessageAccessor.GetRequiredSocketMessage()
			.Channel
			.SendMessageAsync(message, messageReference: messageReference)
			.Wait();
	}
}
