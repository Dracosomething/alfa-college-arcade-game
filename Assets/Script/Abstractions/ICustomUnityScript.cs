using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public interface ICustomUnityScript
{
    public void InitializeComponents(MonoBehaviour self)
    {
        Type classType = GetType();

        // We get all non-static fields no matter the access modifier.
        FieldInfo[] fields = classType.GetFields(BindingFlags.Public |
                                                 BindingFlags.NonPublic |
                                                 BindingFlags.Instance);
        
        string[] missingComponentNames = fields
            .Where(field => field.HasAttribute<ComponentAttribute>()) // We filter out all fields without the ComponentAttribute attribute
            .Where(field => // We filter out all fields where the component is missing.
            {
                var fieldType = field.FieldType; // We get the datatype of the field

                bool doesComponentExist = self.TryGetComponent(fieldType, out Component component);
                
                field.SetValue(self, component); // We initialize the field (will be null if not found)
                
                return !doesComponentExist;
            })
            .Select(field => field.FieldType.Name) // We select the name of the data type of the field
            .ToArray();

        if (!missingComponentNames.Any())
            return; // There are no uninitialized components
        
        // We construct the exception message.
        var exceptionMessageBuilder = new StringBuilder();

        exceptionMessageBuilder.Append(self.name);
        exceptionMessageBuilder.Append(" is missing the following components: ");
    
        foreach (string missingComponentName in missingComponentNames)
            exceptionMessageBuilder.Append(missingComponentName).Append(", ");

        throw new MissingComponentException(exceptionMessageBuilder.ToString());
    }
}