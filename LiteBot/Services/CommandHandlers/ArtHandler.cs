using Discord.Rest;
using LiteBot.DTOs;
using LiteBot.Exceptions;
using LiteBot.Interfaces;

namespace LiteBot.Services.CommandHandlers;

public class ArtHandler(
	ICurrentChannelMessageService messageService,
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
			await messageService.SendMessageAsync("""
				Джерела, що підтримуються:
					https://api.waifu.pics/sfw/neko
					https://api.waifu.im/search/?included_tags=maid
					https://api.waifu.im/search/?included_tags=waifu
					https://api.waifu.im/search/?included_tags=marin-kitagawa
					https://api.waifu.im/search/?included_tags=mori-calliope
					https://api.waifu.im/search/?included_tags=raiden-shogun
					https://api.waifu.im/search/?included_tags=oppai
					https://api.waifu.im/search/?included_tags=selfies
					https://api.waifu.im/search/?included_tags=uniform
				Можна спробувати ввести інше джерело, можливо воно буде працювати
				""");
		}
		else if (commandAnalizer.HasSubcommand(commandInfo, "джерело", out var subcommandInfo)) {
			await File.WriteAllTextAsync(apiFilePath, subcommandInfo!.Argument.Trim());

			await messageService.SendMessageAsync("Джерело змінено");
		}
		else if (uint.TryParse(commandInfo.Argument, out uint numberOfPictures)) {
			if (numberOfPictures > 20) {
				await messageService.SendMessageAsync("Надто багато зображень");
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
		await messageService.SendMessageAsync(imageUrl);
	}

	private Task<RestUserMessage> HelpMessageAsync() {
		return messageService.SendMessageAsync("""
			Доступні команди:
				`"Немає аргументів"` - вивід одного арту
				`"число"` - для надсилання кількох артів
				`джерело` - список стандартних API
				`джерело "посилання"` - встановити задане джерело
			""");
	}
}
