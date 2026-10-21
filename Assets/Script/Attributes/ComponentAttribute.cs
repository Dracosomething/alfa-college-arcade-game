using System;

[AttributeUsage(AttributeTargets.Field)]
public class ComponentAttribute : Attribute
{
    private string _componentName;
}