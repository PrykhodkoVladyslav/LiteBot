using Discord.WebSocket;

namespace LiteBot.Interfaces;

public interface ICurrentUserService {
	void Initialize(SocketUser socketUser);

	SocketUser? GetSocketUser();
	SocketUser GetRequiredSocketUser();
	ulong GetId();
}
