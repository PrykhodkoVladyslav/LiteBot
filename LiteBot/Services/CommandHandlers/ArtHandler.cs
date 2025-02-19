using Discord;
using LiteBot.DTOs;
using LiteBot.Exceptions;
using LiteBot.Interfaces;

namespace LiteBot.Services.CommandHandlers;

public class ArtHandler(
	ISocketMessageAccessor socketMessageAccessor,
	ICommandAnalizer commandAnalizer,
	IImageFromApiLoader imageFromApiLoader
) : ICommandHandler {

	protected readonly string apiFilePath = "api.txt";

	public async Task HandleCommandAsync(CommandInfo commandInfo) {
		if (string.IsNullOrEmpty(commandInfo.Argument)) {
			await DefaultActionAsync();
		}
		else if (commandInfo.Argument == "?") {
			await HelpMessageAsync();
		}
		else if (commandInfo.Argument == "джерело") {
			await SendMessageAsync(
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
			await File.WriteAllTextAsync(apiFilePath, subcommandInfo!.Argument.Trim());

			await SendMessageAsync("Джерело змінено");
		}
		else if (uint.TryParse(commandInfo.Argument, out uint numberOfPictures)) {
			if (numberOfPictures > 20) {
				await SendMessageAsync("Надто багато зображень");
			}

			for (uint i = 0; i < numberOfPictures; i++) {
				await DefaultActionAsync();
			}
		}
		else {
			throw new UnknownCommandException();
		}
	}

	private async Task DefaultActionAsync() {
		var imageUrl = await imageFromApiLoader.GetImageUrlByRegexAsync(File.ReadAllText(apiFilePath), "\"url\":\"([^\"]*)\"");
		await SendMessageAsync(imageUrl);
	}

	private Task HelpMessageAsync() {
		return SendMessageAsync(
			"Доступні команди:\n" +
				"	\"Немає аргументів\" - вивід одного арту\n" +
				"	\"число\" - для надсилання кількох артів\n" +
				"	джерело - список стандартних API\n" +
				"	джерело \"посилання\" - змінити джерело на нове"
		);
	}

	private Task SendMessageAsync(string message, MessageReference? messageReference = null) {
		return socketMessageAccessor.GetRequiredSocketMessage()
			.Channel
			.SendMessageAsync(message, messageReference: messageReference);
	}
}
