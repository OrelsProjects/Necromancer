using System;

public class TragetNotAvailableException : Exception
{
    public TragetNotAvailableException()
    {
    }

    public TragetNotAvailableException(string message)
        : base(message)
    {
    }

    public TragetNotAvailableException(string message, Exception inner)
        : base(message, inner)
    {
    }
}