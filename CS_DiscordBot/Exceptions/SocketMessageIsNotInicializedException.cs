namespace LiteBot.Exceptions;

class SocketMessageIsNotInicializedException : Exception {
	public SocketMessageIsNotInicializedException() : base("Socket message is not inicialized") { }
}
