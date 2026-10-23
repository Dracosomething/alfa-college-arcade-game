using System;

[AttributeUsage(AttributeTargets.Field)]
public class ComponentAttribute : Attribute
{
    public bool InitializeIfMissing { get; set; } = false;
}