using Discord.WebSocket;

namespace LiteBot.Interfaces;

public interface ISocketMessageAccessor {
	void Initialize(SocketMessage socketMessage);

	SocketMessage? GetSocketMessage();
	SocketMessage GetRequiredSocketMessage();
}
