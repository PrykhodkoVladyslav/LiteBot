using Discord;
using Discord.WebSocket;
using LiteBot.CommandHandlers;
using LiteBot.CommandHandlers.Commands;
using LiteBot.Exceptions;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace LiteBot.HostedServices;

public class DiscordBot : IHostedService {
	private DiscordSocketClient? _client;
	private const string commandIdentifier = "=";
	private ICommandHandler _commandHandler = new BotCommandHandler(commandIdentifier);

	private HashSet<ulong> channelsWhiteList = null!;

	public async Task StartAsync(CancellationToken cancellationToken) {
		var token = GetTokenOrThrow();

		_client = new DiscordSocketClient(
			new DiscordSocketConfig {
				GatewayIntents = GatewayIntents.All
			}
		);

		_client.MessageReceived += CommandsHandlerAsync;
		_client.ButtonExecuted += ButtonHandlerAsync;
		_client.Log += LogAsync;
		_client.Ready += async () => {
			await Console.Out.WriteLineAsync("Bot is ready to use!");
		};

		try {
			channelsWhiteList = JsonConvert.DeserializeObject<HashSet<ulong>>(File.ReadAllText("Configurations\\WhiteList.json"))
				?? throw new NullReferenceException();
		}
		catch (Exception e) {
			var message = $"""
				Error reading Configurations/WhiteList.json file.
				Maybe the file does not exist or it is not correct.
				{e}
				""";

			throw new Exception(message);
		}

		await _client.LoginAsync(TokenType.Bot, token);
		await _client.StartAsync();
	}

	private static string GetTokenOrThrow() {
		try {
			return File.ReadAllText("Token.txt");
		}
		catch (Exception) {
			throw new Exception("Enter your bot token to file Token.txt");
		}
	}

	public async Task StopAsync(CancellationToken cancellationToken) {
		if (_client is null)
			return;

		if (_client.ConnectionState == ConnectionState.Connected || _client.ConnectionState == ConnectionState.Connecting)
			await _client.StopAsync();

		await _client.DisposeAsync();
	}

	private async Task CommandsHandlerAsync(SocketMessage message) {
		if (!channelsWhiteList.Contains(message.Channel.Id))
			return;

		await Task.Run(() => HandleMessage(message));
	}

	private async Task ButtonHandlerAsync(SocketMessageComponent arg) {
		if (arg.Data.CustomId == "sd 1") {
			await arg.Channel.SendMessageAsync("b1");
		}
		else if (arg.Data.CustomId == "sd 2") {
			await arg.Channel.SendMessageAsync("b2", messageReference: new MessageReference(arg.Message.Id, arg.Message.Channel.Id));
		}

		await arg.RespondAsync("Click handled");
	}

	private async Task LogAsync(LogMessage msg) {
		await Task.Run(() => Console.WriteLine(msg.ToString()));
	}

	private async Task MessageUpdatedAsync(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel) {
		// If the message was not in the cache, downloading it will result in getting a copy of `after`.
		var message = await before.GetOrDownloadAsync();
		Console.WriteLine($"{message} -> {after}");
	}



	protected void PrintMessageInfo(SocketMessage message) {
		Console.WriteLine($"""
			Channel: {message.Channel}
			Author: {message.Author}
			Id: {message.Id}
			EditedTimestamp: {message.EditedTimestamp}
			CreatedAt: {message.CreatedAt}
			CleanContent: {message.CleanContent}
			Content: {message.Content}

			""");
	}

	protected void HandleMessage(SocketMessage message) {
		if (message.Author.IsBot)
			return;

		PrintMessageInfo(message);

		//IReadOnlyCollection<Attachment> collection = message.Attachments;
		//foreach (Attachment item in collection) {
		//	Console.WriteLine("" + item.ToString());
		//	Console.WriteLine("ContentType: " + item.ContentType);
		//	Console.WriteLine("Description: " + item.Description);
		//	Console.WriteLine("URL: " + item.Url);
		//}

		try {
			_commandHandler.HandleCommand(message);
		}
		catch (IsNotCommandException) { }
		catch (UnknownCommandException) {
			message.Channel.SendMessageAsync($"Невідома команда, для детальнішої інформації про команди спробуйте \"{commandIdentifier}?\"");
		}
		catch (Exception e) {
			message.Channel.SendMessageAsync($"Невідома помилка, код помилки {e}");
		}
	}
}
