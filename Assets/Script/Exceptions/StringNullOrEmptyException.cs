using System;

public class StringNullOrEmptyException : Exception
{
    public StringNullOrEmptyException(string message) 
        : base(message)
    {}
}