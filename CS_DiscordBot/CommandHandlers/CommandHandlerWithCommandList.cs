using Discord.WebSocket;
using LiteBot.Exceptions;

namespace LiteBot.CommandHandlers;

public abstract class CommandHandlerWithCommandList : CommandHandler {
	protected IList<ICommandHandler> commandHandlers;

	public CommandHandlerWithCommandList(string commandIdentifier, IList<ICommandHandler> commandHandlers) : base(commandIdentifier) {
		this.commandHandlers = commandHandlers;
	}

	protected override void ExecuteCommand(string arguments) {
		if (arguments == string.Empty) {
			DefaultAction();
			return;
		}
		if (arguments == "?") {
			HelpMessage();
			return;
		}

		foreach (ICommandHandler handler in commandHandlers) {
			try {
				handler.HandleCommand(socketMessage, arguments);
				return;
			}
			catch (IsNotCommandException) { }
		}
		throw new UnknownCommandException();
	}

	protected override void ExecuteButtonClick(SocketMessageComponent socketMessageComponent, string customId) {
		foreach (ICommandHandler handler in commandHandlers) {
			try {
				handler.HandleButtonClick(socketMessageComponent);
				return;
			}
			catch (UnknownButtonException) { }
		}
		throw new UnknownButtonException();
	}
}