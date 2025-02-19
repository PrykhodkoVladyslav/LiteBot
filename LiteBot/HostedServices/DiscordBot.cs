using Discord;
using Discord.WebSocket;
using LiteBot.DTOs;
using LiteBot.Interfaces;
using LiteBot.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LiteBot.HostedServices;

public class DiscordBot(
	IOptions<BotOptions> botOptions,
	ILogger<DiscordBot> logger,
	IServiceScopeFactory serviceScopeFactory,
	ICommandAnalizer commandAnalizer
) : IHostedService {

	private readonly BotOptions _botOptions = botOptions.Value;
	private readonly WhiteListOptions _whiteListOptions = botOptions.Value.WhiteList;
	private DiscordSocketClient? _client;

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
		var token = _botOptions.Token;

		if (string.IsNullOrEmpty(token))
			throw new Exception("Enter your bot token to configuration file.");

		return token;
	}

	private async Task HandleCommandAsync(SocketMessage message) {
		if (_botOptions.IgnoreMessagesFromBots && message.Author.IsBot)
			return;

		if (_whiteListOptions.Enabled &&
			!_whiteListOptions.AllowedChannelIds.Contains(message.Channel.Id)) {
			return;
		}

		if (!commandAnalizer.IsCommand(message.Content, out CommandInfo? commandInfo))
			return;

		if (_botOptions.LogReceivedMessages)
			LogMessageInfo(message);

		await ExecuteCommandAsync(message, commandInfo!);
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
		logger.LogInformation("{LogMessage}", msg.ToString());

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
		logger.LogInformation("""
			Message info:
			Channel: {Channel}
			Author: {Author}
			Id: {Id}
			CreatedAt: {CreatedAt}
			CleanContent: {CleanContent}
			Content: {Content}
			""",
			message.Channel,
			message.Author,
			message.Id,
			message.CreatedAt,
			message.CleanContent,
			message.Content
		);
	}

	private async Task ExecuteCommandAsync(SocketMessage socketMessage, CommandInfo commandInfo) {
		await using var scope = serviceScopeFactory.CreateAsyncScope();
		var serviceProvider = scope.ServiceProvider;

		serviceProvider.GetRequiredService<ISocketMessageAccessor>().Initialize(socketMessage);
		serviceProvider.GetRequiredService<ICurrentUserService>().Initialize(socketMessage.Author);

		var commandController = serviceProvider.GetRequiredService<CommandController>();

		try {
			await commandController.HandleCommandAsync(commandInfo);
		}
		catch (Exception e) {
			logger.LogError(e, "Command handling unknown error.");
		}
	}
}
