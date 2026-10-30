using System;

public class ClassCannotHaveAwakeException : Exception
{
    public ClassCannotHaveAwakeException(string message) 
        : base(message)
    {}
}