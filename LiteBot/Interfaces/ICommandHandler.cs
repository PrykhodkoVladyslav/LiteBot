using LiteBot.DTOs;

namespace LiteBot.Interfaces;

public interface ICommandHandler {
	Task HandleCommandAsync(CommandInfo commandInfo);
}
