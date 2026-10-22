using System;

[AttributeUsage(AttributeTargets.Field)]
public class ComponentAttribute : Attribute
{
    public bool InitializeIfMissing { get; } = false;
    
    public ComponentAttribute()
    { }

    public ComponentAttribute(bool initializeIfMissing)
    {
        this.InitializeIfMissing = initializeIfMissing;
    }
}