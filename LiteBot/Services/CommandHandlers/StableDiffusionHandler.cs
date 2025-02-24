using Discord;
using Discord.Rest;
using LiteBot.DTOs;
using LiteBot.Exceptions;
using LiteBot.Interfaces;
using LiteBot.MathExtentions;
using LiteBot.Services.StableDiffusionUserRequests;
using Microsoft.Extensions.DependencyInjection;

namespace LiteBot.Services.CommandHandlers;

public class StableDiffusionHandler(
	ICommandAnalizer commandAnalizer,
	ICurrentChannelMessageService messageService,
	IStableDiffusionQueue stableDiffusionQueue,
	IServiceProvider serviceProvider
) : ICommandHandler {

	public async Task HandleCommandAsync(CommandInfo commandInfo) {
		var arguments = commandInfo.Argument;

		if (arguments == string.Empty) {
			stableDiffusionQueue.Enqueue(serviceProvider.GetRequiredService<GenerationRequest>());
		}
		else if (arguments == "?") {
			await HelpMessageAsync();
		}
		else if (commandAnalizer.HasSubcommand(commandInfo, "p", out SubcommandInfo? subcommandInfo) ||
			commandAnalizer.HasSubcommand(commandInfo, "prompt", out subcommandInfo)) {
			stableDiffusionQueue.Enqueue(BuildSetPropertyRequest("prompt", subcommandInfo!.Argument));
		}
		else if (commandAnalizer.HasSubcommand(commandInfo, "np", out subcommandInfo) ||
			commandAnalizer.HasSubcommand(commandInfo, "negative_prompt", out subcommandInfo)) {
			stableDiffusionQueue.Enqueue(BuildSetPropertyRequest("negative_prompt", subcommandInfo!.Argument));
		}
		else if (commandAnalizer.HasSubcommand(commandInfo, "s", out subcommandInfo) ||
			commandAnalizer.HasSubcommand(commandInfo, "steps", out subcommandInfo)) {
			if (!uint.TryParse(subcommandInfo!.Argument, out uint steps)) {
				await messageService.SendReplyMessageAsync("Вкажіть ціле додатне число");
				return;
			}

			if (!MathExpanded.Between<uint>(1, steps, 100)) {
				await messageService.SendReplyMessageAsync("Число має бути в діапазоні від 1 до 100");
				return;
			}

			stableDiffusionQueue.Enqueue(BuildSetPropertyRequest("steps", steps));
		}
		else if (commandAnalizer.HasSubcommand(commandInfo, "cfg", out subcommandInfo)) {
			if (!uint.TryParse(subcommandInfo!.Argument, out uint cfgScale)) {
				await messageService.SendReplyMessageAsync("Вкажіть ціле додатне число");
				return;
			}

			if (!MathExpanded.Between<uint>(1, cfgScale, 30)) {
				await messageService.SendReplyMessageAsync("Число має бути в діапазоні від 1 до 30");
				return;
			}

			stableDiffusionQueue.Enqueue(BuildSetPropertyRequest("cfg_scale", cfgScale));
		}
		else if (commandAnalizer.HasSubcommand(commandInfo, "w", out subcommandInfo) ||
			commandAnalizer.HasSubcommand(commandInfo, "width", out subcommandInfo)) {

			if (!uint.TryParse(subcommandInfo!.Argument, out uint width)) {
				await messageService.SendReplyMessageAsync("Вкажіть ціле додатне число");
				return;
			}

			if (!MathExpanded.Between<uint>(1, width, 1000)) {
				await messageService.SendReplyMessageAsync("Число має бути в діапазоні від 1 до 1000");
				return;
			}

			stableDiffusionQueue.Enqueue(BuildSetPropertyRequest("width", width));
		}
		else if (commandAnalizer.HasSubcommand(commandInfo, "h", out subcommandInfo) ||
			commandAnalizer.HasSubcommand(commandInfo, "height", out subcommandInfo)) {

			if (!uint.TryParse(subcommandInfo!.Argument, out uint height)) {
				await messageService.SendReplyMessageAsync("Вкажіть ціле додатне число");
				return;
			}

			if (!MathExpanded.Between<uint>(1, height, 1000)) {
				await messageService.SendReplyMessageAsync("Число має бути в діапазоні від 1 до 1000");
				return;
			}

			stableDiffusionQueue.Enqueue(BuildSetPropertyRequest("height", height));
		}
		else if (arguments == "default") {
			stableDiffusionQueue.Enqueue(serviceProvider.GetRequiredService<ResetPropertyRequest>());
		}
		else if (arguments != string.Empty) {
			stableDiffusionQueue.Enqueue(BuildSetPropertyRequest("prompt", arguments));
			stableDiffusionQueue.Enqueue(serviceProvider.GetRequiredService<GenerationRequest>());
		}
		else {
			throw new UnknownCommandException();
		}

		await messageService.AddReactionAsync(new Emoji("✅"));
	}

	private Task<RestUserMessage> HelpMessageAsync() {
		return messageService.SendMessageAsync("""
			Доступні команди:
				`"Немає аргументів"` - генерує зображення використовуючи встановлені параметри
				`"текст промпту"` - встановлює промпт та запускає генерацію
				`p "текст промпту"` або `prompt "текст промпту"` - встановлює промпт для генерації
				`np "текст анти-промпту"` або `negative_prompt "текст анти-промпту"` - встановлює анти-промпт для генерації
				`s "число"` або `steps "число"` - встановлює кількість ітерацій, яку виконує AI над зображенням. Стандартне значення 20
				`cfg "число"` - встановлює значення властивості cfg_scale. Дане число впливає на силу дії промптів та анти-промптів. Стандартне значення 7
				`w "число"` або `width "число"` - встановлює ширину зображення в пікселях
				`h "число"` або `height "число"` - встановлює висоту зображення в пікселях
				`default` - скидає всі налаштування до стандартних
			""");
	}

	private SetPropertyRequest BuildSetPropertyRequest(string property, object value) {
		var request = serviceProvider.GetRequiredService<SetPropertyRequest>();

		request.Property = property;
		request.Value = value;

		return request;
	}
}
