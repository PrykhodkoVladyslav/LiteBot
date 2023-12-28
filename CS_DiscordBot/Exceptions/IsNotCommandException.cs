namespace LiteBot.Exceptions;

public class IsNotCommandException : Exception {
	public IsNotCommandException() : this("Is not command exception") { }
	public IsNotCommandException(string message) : base(message) { }
}