using Discord;
using Discord.WebSocket;
using LiteBot.CommandHandlers;
using LiteBot.CommandHandlers.Commands;
using LiteBot.Exceptions;
using LiteBot.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LiteBot.HostedServices;

public class DiscordBot(
	IConfiguration configuration,
	IOptions<WhiteListOptions> whiteListOptions,
	ILogger<DiscordBot> logger
) : IHostedService {
	private readonly WhiteListOptions _whiteListOptions = whiteListOptions.Value;

	private DiscordSocketClient? _client;
	private const string commandIdentifier = "=";
	private ICommandHandler _commandHandler = new BotCommandHandler(commandIdentifier);

	public async Task StartAsync(CancellationToken cancellationToken) {
		_client = new DiscordSocketClient(
			new DiscordSocketConfig {
				GatewayIntents = GatewayIntents.All
			}
		);

		_client.MessageReceived += HandleCommandAsync;
		_client.ButtonExecuted += HandleButtonAsync;
		_client.Log += LogAsync;
		_client.Ready += ReadyAsync;

		await _client.LoginAsync(TokenType.Bot, GetTokenOrThrow());
		await _client.StartAsync();
	}

	public async Task StopAsync(CancellationToken cancellationToken) {
		if (_client is null)
			return;

		if (_client.ConnectionState == ConnectionState.Connected || _client.ConnectionState == ConnectionState.Connecting)
			await _client.StopAsync();

		await _client.DisposeAsync();
	}



	private string GetTokenOrThrow() {
		var token = configuration.GetValue<string>("Token");

		if (string.IsNullOrEmpty(token))
			throw new Exception("Enter your bot token to configuration file.");

		return token;
	}

	private async Task HandleCommandAsync(SocketMessage message) {
		if (configuration.GetValue<bool>("IgnoreMessagesFromBots") && message.Author.IsBot)
			return;

		if (_whiteListOptions.Enabled &&
			!_whiteListOptions.AllowedChannelIds.Contains(message.Channel.Id)) {
			return;
		}

		if (configuration.GetValue<bool>("LogReceivedMessages"))
			LogMessageInfo(message);

		try {
			_commandHandler.HandleCommand(message);
		}
		catch (IsNotCommandException) { }
		catch (UnknownCommandException) {
			await message.Channel.SendMessageAsync($"Невідома команда, для детальнішої інформації про команди спробуйте \"{commandIdentifier}?\"");
		}
		catch (Exception e) {
			await message.Channel.SendMessageAsync($"Невідома помилка, код помилки {e}");
		}
	}

	private async Task HandleButtonAsync(SocketMessageComponent arg) {
		if (arg.Data.CustomId == "sd 1") {
			await arg.Channel.SendMessageAsync("b1");
		}
		else if (arg.Data.CustomId == "sd 2") {
			await arg.Channel.SendMessageAsync("b2", messageReference: new MessageReference(arg.Message.Id, arg.Message.Channel.Id));
		}

		await arg.RespondAsync("Click handled");
	}

	private Task LogAsync(LogMessage msg) {
		logger.LogInformation(msg.ToString());

		return Task.CompletedTask;
	}

	private Task ReadyAsync() {
		logger.LogInformation("Bot is ready to use!");

		return Task.CompletedTask;
	}

	private async Task MessageUpdatedAsync(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel) {
		// If the message was not in the cache, downloading it will result in getting a copy of `after`.
		var message = await before.GetOrDownloadAsync();
		Console.WriteLine($"{message} -> {after}");
	}



	private void LogMessageInfo(SocketMessage message) {
		logger.LogInformation($"""
			Message info:
			Channel: {message.Channel}
			Author: {message.Author}
			Id: {message.Id}
			EditedTimestamp: {message.EditedTimestamp}
			CreatedAt: {message.CreatedAt}
			CleanContent: {message.CleanContent}
			Content: {message.Content}
			""");
	}
}
