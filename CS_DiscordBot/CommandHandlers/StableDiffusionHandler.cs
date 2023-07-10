using Discord;
using LiteBot.CommandHandlersBase;
using LiteBot.StableDiffusion;
using LiteBot.Exceptions;
using LiteBot.StableDiffusion.UserRequests;
using LiteBot.MathExtentions;

namespace LiteBot.CommandHandlers;

public class StableDiffusionHandler : CommandHandler {
	protected StableDiffusionApi api = new();
	protected StableDiffusionQueue sdQueue = new();
	protected PropertyAccessor propertyAccessor = new();

	public StableDiffusionHandler(string commandIdentifier) : base(commandIdentifier) {
		sdQueue.ExceptionCatched += (sender, e) => {
			File.AppendAllText("Exceptions.log", $"""
				{DateTime.Now.ToLongTimeString()}
				{e}

				""");
		};
	}

	protected override void ExecuteCommand(string arguments) {
		if (arguments == string.Empty) {
			sdQueue.Enqueue(new GenerationRequest(socketMessage, new ApiWithProperties(api, propertyAccessor)));
		}
		else if (arguments == "?") {
			HelpMessage();
		}
		else if (IsSubcommand(arguments, "p", out string value) || IsSubcommand(arguments, "prompt", out value)) {
			sdQueue.Enqueue(new SetPropertyRequest(socketMessage, propertyAccessor, "prompt", value));
		}
		else if (IsSubcommand(arguments, "np", out value) || IsSubcommand(arguments, "negative prompt", out value)) {
			sdQueue.Enqueue(new SetPropertyRequest(socketMessage, propertyAccessor, "negative_prompt", value));
		}
		else if (IsSubcommand(arguments, "s", out value) || IsSubcommand(arguments, "steps", out value)) {
			if (!TypeChecker.IsUInt32(value)) {
				SendMessage("Uncorrect value type", new MessageReference(socketMessage.Id, socketMessage.Channel.Id));
				return;
			}

			int steps = Convert.ToInt32(value);

			if (!MathExpanded.Between(1, steps, 100)) {
				SendMessage("The value of the property must be between 1 and 100", new MessageReference(socketMessage.Id, socketMessage.Channel.Id));
				return;
			}

			sdQueue.Enqueue(new SetPropertyRequest(socketMessage, propertyAccessor, "steps", steps));
		}
		else if (IsSubcommand(arguments, "cfg", out value)) {
			if (!TypeChecker.IsUInt32(value)) {
				SendMessage("Uncorrect value type", new MessageReference(socketMessage.Id, socketMessage.Channel.Id));
				return;
			}

			int cfgScale = Convert.ToInt32(value);

			if (!MathExpanded.Between(1, cfgScale, 30)) {
				SendMessage("The value of the property must be between 1 and 30", new MessageReference(socketMessage.Id, socketMessage.Channel.Id));
				return;
			}

			sdQueue.Enqueue(new SetPropertyRequest(socketMessage, propertyAccessor, "cfg_scale", cfgScale));
		}
		else if (IsSubcommand(arguments, "w", out value) || IsSubcommand(arguments, "width", out value)) {
			if (!TypeChecker.IsUInt32(value)) {
				SendMessage("Uncorrect value type", new MessageReference(socketMessage.Id, socketMessage.Channel.Id));
				return;
			}

			int width = Convert.ToInt32(value);

			if (!MathExpanded.Between(1, width, 1000)) {
				SendMessage("The value of the property must be between 1 and 1000", new MessageReference(socketMessage.Id, socketMessage.Channel.Id));
				return;
			}

			sdQueue.Enqueue(new SetPropertyRequest(socketMessage, propertyAccessor, "width", width));
		}
		else if (IsSubcommand(arguments, "h", out value) || IsSubcommand(arguments, "height", out value)) {
			if (!TypeChecker.IsUInt32(value)) {
				SendMessage("Uncorrect value type", new MessageReference(socketMessage.Id, socketMessage.Channel.Id));
				return;
			}

			int height = Convert.ToInt32(value);

			if (!MathExpanded.Between(1, height, 1000)) {
				SendMessage("The value of the property must be between 1 and 1000", new MessageReference(socketMessage.Id, socketMessage.Channel.Id));
				return;
			}

			sdQueue.Enqueue(new SetPropertyRequest(socketMessage, propertyAccessor, "height", height));
		}
		else if (arguments == "default") {
			sdQueue.Enqueue(new ResetPropertyRequest(socketMessage, propertyAccessor));
		}
		else if (arguments != string.Empty) {
			sdQueue.Enqueue(new SetPropertyRequest(socketMessage, propertyAccessor, "prompt", arguments));
			sdQueue.Enqueue(new GenerationRequest(socketMessage, new ApiWithProperties(api, propertyAccessor)));
		}
		else {
			throw new UnknownCommandException();
		}

		socketMessage.AddReactionAsync(new Emoji("✅"));
	}

	//protected override void DefaultAction() {
	//
	//}

	protected override void HelpMessage() {
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
}