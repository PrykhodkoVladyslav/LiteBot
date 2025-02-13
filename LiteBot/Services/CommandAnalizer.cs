using LiteBot.DTOs;
using LiteBot.Interfaces;
using LiteBot.Options;
using Microsoft.Extensions.Options;

namespace LiteBot.Services;

public class CommandAnalizer(
	IOptions<BotOptions> botOptions
) : ICommandAnalizer {

	private readonly BotOptions _botOptions = botOptions.Value;
	private readonly char[] _spaceChars = [' ', '\t', '\n'];

	public bool IsCommand(string text) {
		return text.StartsWith(_botOptions.Prefix);
	}

	public bool IsCommand(string text, out CommandInfo? commandInfo) {
		if (!IsCommand(text)) {
			commandInfo = null;
			return false;
		}

		commandInfo = new CommandInfo(_botOptions.Prefix, GetCommandName(text), GetCommandArgument(text));
		return true;
	}

	public bool HasSubcommand(CommandInfo commandInfo, string subcommandName) {
		var subcommand = GetTextUntilSpace(commandInfo.Argument);

		return subcommand == subcommandName;
	}

	public bool HasSubcommand(CommandInfo commandInfo, string subcommandName, out SubcommandInfo? subcommandInfo) {
		if (!HasSubcommand(commandInfo, subcommandName)) {
			subcommandInfo = null;
			return false;
		}

		var subcommand = GetTextUntilSpace(commandInfo.Argument);

		int argumentIndex = subcommand.Length;

		while (commandInfo.Argument.Length > argumentIndex && _spaceChars.Contains(commandInfo.Argument[argumentIndex]))
			argumentIndex++;

		subcommandInfo = new SubcommandInfo(subcommand, commandInfo.Argument[argumentIndex..]);
		return true;
	}


	private string GetCommandName(string command) {
		return GetTextUntilSpace(command, GetCommandStartIndex(command));
	}

	private int GetCommandStartIndex(string command) {
		var prefixLength = _botOptions.Prefix.Length;

		while (prefixLength < command.Length && _spaceChars.Contains(command[prefixLength]))
			prefixLength++;

		return prefixLength;
	}

	private int GetArgumentStartIndex(string command) {
		var commandIndex = GetCommandStartIndex(command);

		while (commandIndex < command.Length && !_spaceChars.Contains(command[commandIndex]))
			commandIndex++;
		while (commandIndex < command.Length && _spaceChars.Contains(command[commandIndex]))
			commandIndex++;

		return commandIndex;
	}

	private string GetCommandArgument(string command) {
		return command[GetArgumentStartIndex(command)..];
	}

	private string GetTextUntilSpace(string text, int startIndex = 0) {
		var index = text.IndexOfAny(_spaceChars, startIndex);
		return index == -1 ? text[startIndex..] : text[startIndex..index];
	}
}
