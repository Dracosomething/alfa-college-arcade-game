using System;

public class NoMainCameraException : Exception
{
    public NoMainCameraException()
        : base("No main camera has been defined!")
    {}
}