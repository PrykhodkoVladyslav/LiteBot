using LiteBot.DTOs;

namespace LiteBot.Interfaces;

public interface ICommandAnalizer {
	bool IsCommand(string text);
	bool IsCommand(string text, out CommandInfo? commandInfo);

	bool HasSubcommand(CommandInfo commandInfo, string subcommandName);
	bool HasSubcommand(CommandInfo commandInfo, string subcommandName, out SubcommandInfo? subcommandInfo);
}
