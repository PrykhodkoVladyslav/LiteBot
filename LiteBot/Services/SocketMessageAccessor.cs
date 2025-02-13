using Discord.WebSocket;
using LiteBot.Exceptions;
using LiteBot.Interfaces;

namespace LiteBot.Services;

class SocketMessageAccessor : ISocketMessageAccessor {
	private SocketMessage? _socketMessage;

	public void Initialize(SocketMessage socketMessage) {
		ArgumentNullException.ThrowIfNull(socketMessage, nameof(socketMessage));

		_socketMessage = socketMessage;
	}

	public SocketMessage? GetSocketMessage() {
		return _socketMessage;
	}

	public SocketMessage GetRequiredSocketMessage() {
		return _socketMessage ?? throw new SocketMessageIsNotInicializedException();
	}
}
