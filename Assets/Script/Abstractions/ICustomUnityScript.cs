using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public interface ICustomUnityScript
{
    public void ValidateComponents(object self)
    {
        Type classType = GetType();

        FieldInfo[] fields = classType.GetFields(BindingFlags.Public |
                                                 BindingFlags.NonPublic |
                                                 BindingFlags.Instance);

        FieldInfo[] missingComponents = fields
            .Where(field => field.HasAttribute<ComponentAttribute>())
            .Where(field =>
            {
                var fieldValue = field.GetValue(self); // We get the current value of the field
                var fieldType = field.FieldType; // We get the datatype of the field
                var defaultValue = fieldType.Default(); // We get the default value of the data type of the field
                
                return fieldValue.IsUnityNull() && !fieldValue.Equals(defaultValue);
            })
            .ToArray();

        if (missingComponents.Any())
            return; // There are no uninitialized components
        
        var exceptionMessageBuilder = new StringBuilder();

        exceptionMessageBuilder.Append(classType.Name);
        exceptionMessageBuilder.Append(" is missing the following components: ");
    
        foreach (FieldInfo missingComponent in missingComponents)
            exceptionMessageBuilder.Append(missingComponent.Name).Append(", ");

        throw new MissingComponentException(exceptionMessageBuilder.ToString());
    }
}