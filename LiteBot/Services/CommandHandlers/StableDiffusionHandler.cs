using Discord;
using Discord.WebSocket;
using LiteBot.DTOs;
using LiteBot.Exceptions;
using LiteBot.Interfaces;
using LiteBot.MathExtentions;
using LiteBot.StableDiffusion;
using LiteBot.StableDiffusion.DTOAccessors;
using LiteBot.StableDiffusion.UserRequests;

namespace LiteBot.Services.CommandHandlers;

public class StableDiffusionHandler : ICommandHandler {
	private ICommandAnalizer _commandAnalizer;
	private SocketMessage _socketMessage;
	private StableDiffusionApi _api;
	private StableDiffusionQueue _sdQueue;
	private Txt2imgAccessor _propertyAccessor;

	public StableDiffusionHandler(ICommandAnalizer commandAnalizer, ISocketMessageAccessor socketMessageAccessor, StableDiffusionApi stableDiffusionApi, Txt2imgAccessor txt2imgAccessor, StableDiffusionQueue stableDiffusionQueue) {
		_commandAnalizer = commandAnalizer;
		_socketMessage = socketMessageAccessor.GetRequiredSocketMessage();
		_api = stableDiffusionApi;
		_propertyAccessor = txt2imgAccessor;
		_sdQueue = stableDiffusionQueue;
	}

	public async Task HandleCommandAsync(CommandInfo commandInfo) {
		var arguments = commandInfo.Argument;

		if (arguments == string.Empty) {
			_sdQueue.Enqueue(new GenerationRequest(_socketMessage, new ApiWithProperties(_api, _propertyAccessor)));
		}
		else if (arguments == "?") {
			HelpMessage();
		}
		else if (_commandAnalizer.HasSubcommand(commandInfo, "p", out SubcommandInfo? subcommandInfo)) {
			_sdQueue.Enqueue(new SetPropertyRequest(_socketMessage, _propertyAccessor, "prompt", subcommandInfo!.Argument));
		}
		else if (_commandAnalizer.HasSubcommand(commandInfo, "np", out subcommandInfo)) {
			_sdQueue.Enqueue(new SetPropertyRequest(_socketMessage, _propertyAccessor, "negative_prompt", subcommandInfo!.Argument));
		}
		else if (_commandAnalizer.HasSubcommand(commandInfo, "s", out subcommandInfo)) {
			if (!TypeChecker.IsUInt32(subcommandInfo!.Argument)) {
				SendMessage("Uncorrect value type", new MessageReference(_socketMessage.Id, _socketMessage.Channel.Id));
				return;
			}

			int steps = Convert.ToInt32(subcommandInfo!.Argument);

			if (!MathExpanded.Between(1, steps, 100)) {
				SendMessage("The value of the property must be between 1 and 100", new MessageReference(_socketMessage.Id, _socketMessage.Channel.Id));
				return;
			}

			_sdQueue.Enqueue(new SetPropertyRequest(_socketMessage, _propertyAccessor, "steps", steps));
		}
		else if (_commandAnalizer.HasSubcommand(commandInfo, "cfg", out subcommandInfo)) {
			if (!TypeChecker.IsUInt32(subcommandInfo!.Argument)) {
				SendMessage("Uncorrect value type", new MessageReference(_socketMessage.Id, _socketMessage.Channel.Id));
				return;
			}

			int cfgScale = Convert.ToInt32(subcommandInfo!.Argument);

			if (!MathExpanded.Between(1, cfgScale, 30)) {
				SendMessage("The value of the property must be between 1 and 30", new MessageReference(_socketMessage.Id, _socketMessage.Channel.Id));
				return;
			}

			_sdQueue.Enqueue(new SetPropertyRequest(_socketMessage, _propertyAccessor, "cfg_scale", cfgScale));
		}
		else if (_commandAnalizer.HasSubcommand(commandInfo, "w", out subcommandInfo) ||
			_commandAnalizer.HasSubcommand(commandInfo, "width", out subcommandInfo)) {

			if (!TypeChecker.IsUInt32(subcommandInfo!.Argument)) {
				SendMessage("Uncorrect value type", new MessageReference(_socketMessage.Id, _socketMessage.Channel.Id));
				return;
			}

			int width = Convert.ToInt32(subcommandInfo!.Argument);

			if (!MathExpanded.Between(1, width, 1000)) {
				SendMessage("The value of the property must be between 1 and 1000", new MessageReference(_socketMessage.Id, _socketMessage.Channel.Id));
				return;
			}

			_sdQueue.Enqueue(new SetPropertyRequest(_socketMessage, _propertyAccessor, "width", width));
		}
		else if (_commandAnalizer.HasSubcommand(commandInfo, "h", out subcommandInfo) ||
			_commandAnalizer.HasSubcommand(commandInfo, "height", out subcommandInfo)) {

			if (!TypeChecker.IsUInt32(subcommandInfo!.Argument)) {
				SendMessage("Uncorrect value type", new MessageReference(_socketMessage.Id, _socketMessage.Channel.Id));
				return;
			}

			int height = Convert.ToInt32(subcommandInfo!.Argument);

			if (!MathExpanded.Between(1, height, 1000)) {
				SendMessage("The value of the property must be between 1 and 1000", new MessageReference(_socketMessage.Id, _socketMessage.Channel.Id));
				return;
			}

			_sdQueue.Enqueue(new SetPropertyRequest(_socketMessage, _propertyAccessor, "height", height));
		}
		else if (arguments == "default") {
			_sdQueue.Enqueue(new ResetPropertyRequest(_socketMessage, _propertyAccessor));
		}
		else if (arguments != string.Empty) {
			_sdQueue.Enqueue(new SetPropertyRequest(_socketMessage, _propertyAccessor, "prompt", arguments));
			_sdQueue.Enqueue(new GenerationRequest(_socketMessage, new ApiWithProperties(_api, _propertyAccessor)));
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
}
