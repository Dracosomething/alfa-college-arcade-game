using System;

public class MissingSerializeFieldAttributeException : Exception
{
    public MissingSerializeFieldAttributeException(string message)
        : base(message)
    {}
}
    