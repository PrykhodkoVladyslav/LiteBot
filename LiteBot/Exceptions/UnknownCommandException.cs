namespace LiteBot.Exceptions;

public class UnknownCommandException : Exception
{
    public UnknownCommandException() : this("Unknown command exception") { }
    public UnknownCommandException(string message) : base(message) { }
}