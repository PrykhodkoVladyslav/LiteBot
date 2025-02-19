namespace LiteBot.Exceptions;

public class SocketMessageIsNotInicializedException : Exception {
	public SocketMessageIsNotInicializedException() : base("Socket message is not inicialized") { }
}
