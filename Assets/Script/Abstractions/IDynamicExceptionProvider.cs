using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public interface IDynamicExceptionProvider
{
    public void InitializeAndValidateFields(MonoBehaviour self)
    {
        InitializeComponents(self);
        ValidateSerializeFieldsSet(self);
    }
    
    public void InitializeComponents(MonoBehaviour self)
    {
        string[] missingComponentNames = GetFieldsWithAttribute<ComponentAttribute>()
            .Where(field => // We filter out all fields where the component is missing.
            {
                ComponentAttribute componentAttribute = field.GetCustomAttribute<ComponentAttribute>();
                Type fieldType = field.FieldType; // We get the datatype of the field

                bool doesComponentExist = self.TryGetComponent(fieldType, out Component component);

                if (!doesComponentExist && componentAttribute.InitializeIfMissing)
                {
                    doesComponentExist = true;
                    component = self.gameObject.AddComponent(fieldType);
                }
                
                field.SetValue(self, component); // We initialize the field (will be null if not found)
                
                return !doesComponentExist;
            })
            .Select(field => field.FieldType.Name) // We select the name of the data type of the field
            .ToArray();

        if (!missingComponentNames.Any())
            return; // There are no uninitialized components

        throw new MissingComponentException(ConstructExceptionMessage(
            self.name, 
            messageBody: " is missing the following components: ", 
            missingComponentNames));
    }

    public void ValidateSerializeFieldsSet(MonoBehaviour self)
    {
        string[] nonSerializedFields = GetFieldsWithAttribute<SerializeField>()
            .Where(field => field.GetValue(self).IsUnityNull()) // We filter out all fields that are null
            .Select(field => field.Name) // We select the name of the field
            .ToArray();
        
        if (!nonSerializedFields.Any())
            return; // all serialize fields have a value

        throw new SerializeFieldNotSetException(ConstructExceptionMessage(
            self.name, 
            messageBody: " has the following serialize fields not initialized: ", 
            nonSerializedFields));
    }
    
    private IEnumerable<FieldInfo> GetFieldsWithAttribute<TAttribute>()
        where TAttribute : Attribute
    {
        Type classType = GetType();

        IEnumerable<FieldInfo> fields = GetFieldsRecursively(classType);
        
        return fields
            .Where(field => field.HasAttribute<TAttribute>()); // We filter out the fields that don't have the requested attribute
    }

    private IEnumerable<FieldInfo> GetFieldsRecursively(Type classType)
    {
        // We get all non-static fields no matter the access modifier.
        List<FieldInfo> fields = classType.GetFields(BindingFlags.Public |
                                                 BindingFlags.NonPublic |
                                                 BindingFlags.Instance).ToList();
        
        // If the class has a base type that is not MonoBehaviour we recurse a level.
        if (classType.BaseType != null && classType.BaseType != typeof(MonoBehaviour))
            fields.AddRange(GetFieldsRecursively(classType.BaseType));

        return fields;
    }
    
    private string ConstructExceptionMessage(string selfName, string messageBody, string[] missingFieldNames)
    {
        var exceptionMessageBuilder = new StringBuilder();

        exceptionMessageBuilder.Append(selfName);
        exceptionMessageBuilder.Append(messageBody);
    
        foreach (string missingComponentName in missingFieldNames)
            exceptionMessageBuilder.Append(missingComponentName).Append(", ");

        return exceptionMessageBuilder.ToString();
    }
}