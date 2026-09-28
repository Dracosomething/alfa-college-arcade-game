using System;

public class AnimationNotDefinedException : Exception
{
    public AnimationNotDefinedException(string message)
        : base(message)
    {}
}