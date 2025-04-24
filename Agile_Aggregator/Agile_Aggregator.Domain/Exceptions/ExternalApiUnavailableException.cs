public class ExternalApiUnavailableException : Exception
{
    public ExternalApiUnavailableException(string message, Exception inner = null)
        : base(message, inner) { }
}
