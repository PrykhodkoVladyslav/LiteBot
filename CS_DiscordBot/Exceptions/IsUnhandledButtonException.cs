namespace LiteBot.Exceptions;

public class IsUnhandledButtonException : Exception {
	public IsUnhandledButtonException() : this("Is unhandled button exception") { }
	public IsUnhandledButtonException(string message) : base(message) { }
}