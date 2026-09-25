using System;

public class SerializeFieldNotSetException : Exception
{
    public SerializeFieldNotSetException(string message)
        : base(message)
    {}
}