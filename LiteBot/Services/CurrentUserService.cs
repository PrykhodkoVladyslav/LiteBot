using Discord.WebSocket;
using LiteBot.Exceptions;
using LiteBot.Interfaces;

namespace LiteBot.Services;

public class CurrentUserService : ICurrentUserService {
	private SocketUser? _socketUser;

	public void Initialize(SocketUser socketUser) {
		ArgumentNullException.ThrowIfNull(socketUser, nameof(socketUser));

		_socketUser = socketUser;
	}

	public SocketUser? GetSocketUser() => _socketUser;

	public SocketUser GetRequiredSocketUser() {
		return _socketUser ?? throw new SocketUserIsNotInicializedException();
	}

	public ulong GetId() {
		return GetRequiredSocketUser().Id;
	}
}
