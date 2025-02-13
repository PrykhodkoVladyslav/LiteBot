namespace LiteBot.Exceptions;

public class UnknownButtonException : Exception {
	public UnknownButtonException() : this("Is unknown button exception") { }
	public UnknownButtonException(string message) : base(message) { }
}
