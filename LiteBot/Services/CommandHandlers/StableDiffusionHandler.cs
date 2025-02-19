using Discord;
using Discord.WebSocket;
using LiteBot.DTOs;
using LiteBot.Exceptions;
using LiteBot.Interfaces;
using LiteBot.MathExtentions;
using LiteBot.Services.StableDiffusionUserRequests;
using Microsoft.Extensions.DependencyInjection;

namespace LiteBot.Services.CommandHandlers;

public class StableDiffusionHandler(
	ICommandAnalizer commandAnalizer,
	ISocketMessageAccessor socketMessageAccessor,
	IStableDiffusionQueue stableDiffusionQueue,
	IServiceProvider serviceProvider
) : ICommandHandler {

	private readonly SocketMessage _socketMessage = socketMessageAccessor.GetRequiredSocketMessage();

	public async Task HandleCommandAsync(CommandInfo commandInfo) {
		var arguments = commandInfo.Argument;

		if (arguments == string.Empty) {
			stableDiffusionQueue.Enqueue(serviceProvider.GetRequiredService<GenerationRequest>());
		}
		else if (arguments == "?") {
			HelpMessage();
		}
		else if (commandAnalizer.HasSubcommand(commandInfo, "p", out SubcommandInfo? subcommandInfo)) {
			stableDiffusionQueue.Enqueue(BuildSetPropertyRequest("prompt", subcommandInfo!.Argument));
		}
		else if (commandAnalizer.HasSubcommand(commandInfo, "np", out subcommandInfo)) {
			stableDiffusionQueue.Enqueue(BuildSetPropertyRequest("negative_prompt", subcommandInfo!.Argument));
		}
		else if (commandAnalizer.HasSubcommand(commandInfo, "s", out subcommandInfo)) {
			if (!uint.TryParse(subcommandInfo!.Argument, out uint steps)) {
				SendMessage("Uncorrect value type", new MessageReference(_socketMessage.Id, _socketMessage.Channel.Id));
				return;
			}

			if (!MathExpanded.Between<uint>(1, steps, 100)) {
				SendMessage("The value of the property must be between 1 and 100", new MessageReference(_socketMessage.Id, _socketMessage.Channel.Id));
				return;
			}

			stableDiffusionQueue.Enqueue(BuildSetPropertyRequest("steps", steps));
		}
		else if (commandAnalizer.HasSubcommand(commandInfo, "cfg", out subcommandInfo)) {
			if (!uint.TryParse(subcommandInfo!.Argument, out uint cfgScale)) {
				SendMessage("Uncorrect value type", new MessageReference(_socketMessage.Id, _socketMessage.Channel.Id));
				return;
			}

			if (!MathExpanded.Between<uint>(1, cfgScale, 30)) {
				SendMessage("The value of the property must be between 1 and 30", new MessageReference(_socketMessage.Id, _socketMessage.Channel.Id));
				return;
			}

			stableDiffusionQueue.Enqueue(BuildSetPropertyRequest("cfg_scale", cfgScale));
		}
		else if (commandAnalizer.HasSubcommand(commandInfo, "w", out subcommandInfo) ||
			commandAnalizer.HasSubcommand(commandInfo, "width", out subcommandInfo)) {

			if (!uint.TryParse(subcommandInfo!.Argument, out uint width)) {
				SendMessage("Uncorrect value type", new MessageReference(_socketMessage.Id, _socketMessage.Channel.Id));
				return;
			}

			if (!MathExpanded.Between<uint>(1, width, 1000)) {
				SendMessage("The value of the property must be between 1 and 1000", new MessageReference(_socketMessage.Id, _socketMessage.Channel.Id));
				return;
			}

			stableDiffusionQueue.Enqueue(BuildSetPropertyRequest("width", width));
		}
		else if (commandAnalizer.HasSubcommand(commandInfo, "h", out subcommandInfo) ||
			commandAnalizer.HasSubcommand(commandInfo, "height", out subcommandInfo)) {

			if (!uint.TryParse(subcommandInfo!.Argument, out uint height)) {
				SendMessage("Uncorrect value type", new MessageReference(_socketMessage.Id, _socketMessage.Channel.Id));
				return;
			}

			if (!MathExpanded.Between<uint>(1, height, 1000)) {
				SendMessage("The value of the property must be between 1 and 1000", new MessageReference(_socketMessage.Id, _socketMessage.Channel.Id));
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

		await _socketMessage.AddReactionAsync(new Emoji("✅"));
	}

	private void HelpMessage() {
		SendMessage("""
			Доступні команди:
				`"Немає аргументів"` - генерує зображення по раніше заданим параметрах
				`"текст промпту"` - встановлює промпт та запускає генерацію
				`p "текст промпту"` або `prompt "текст промпту"` - встановлює промпт для генерації
				`np "текст анти-промпту"` або `negative prompt "текст анти-промпту"` - встановлює анти-промпт для генерації
				`s "число" або steps "число"` - встановлює кількість ітерацій яку виконує AI над зображенням. Стандартне значення 20
				`cfg "число"` - встановлює значення властивості cfg_scale. Вона вплиає на силу дії промптів та анти-промптів. Стандартне значення 7
				`w "число"` або `width "число"` - встановлює ширину зображення в пікселях
				`h "число"` або `height "число"` - встановлює висоту зображення в пікселях
				`default` - встановлює стандартне значення властивостей
			""");
	}

	private void SendMessage(string message, MessageReference? messageReference = null) {
		_socketMessage.Channel.SendMessageAsync(message, messageReference: messageReference).Wait();
	}

	private SetPropertyRequest BuildSetPropertyRequest(string property, object value) {
		var request = serviceProvider.GetRequiredService<SetPropertyRequest>();

		request.Property = property;
		request.Value = value;

		return request;
	}
}
