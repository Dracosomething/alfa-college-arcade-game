using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

/// <summary>
/// This class does 2 things right after all GameObjects are loaded but before Awake is called.
///
/// First it checks if every SerializeField with the RequiredSerializeField attribute has a value, if not we throw an exception.
/// Second it initializes all fields marked with the Component attribute and throws an exception if the required Component can't be found.
///
/// This class also throws all exceptions that have been found across the project.
/// </summary>
public static class CustomAttributeHandler
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitializeEventHandler()
    {
        /*
         * Get all types with an attribute
         * Validate that they lack an awake method
         * Validate that RequiredSerializeFields also have SerializeField
         */
        
        SceneManager.sceneLoaded += (scene, mode) => HandleCustomAttributes();
    }
    
    private static void HandleCustomAttributes()
    {
        Dictionary<Exception, GameObject> exceptionsToThrowWithContextObject = new();

        MonoBehaviour[] loadedScripts = Resources.FindObjectsOfTypeAll<GameObject>()
            .SelectMany(
                gameObject => gameObject.GetComponents<MonoBehaviour>(),
                (gameObject, scriptComponent) => scriptComponent)
            .Where(scriptComponent => !scriptComponent.IsUnityNull())
            .ToArray();

        foreach (MonoBehaviour scriptComponent in loadedScripts)
        {
            Type scriptClass = scriptComponent.GetType();
            GameObject scriptGameObject = scriptComponent.gameObject;
            
            ValidateSerializeFieldsInitialized(scriptClass, scriptComponent, scriptGameObject, exceptionsToThrowWithContextObject);
            InitializeComponentsAndThrowIfMissingComponents(scriptClass, scriptComponent, scriptGameObject,
                exceptionsToThrowWithContextObject);
        }

        if (!exceptionsToThrowWithContextObject.Any())
            return;
        
        // Makes sure all exceptions from exceptionsToThrowWithContextObject are thrown with the corresponding context.
        AggregateException aggregateException = new(exceptionsToThrowWithContextObject.Keys);
        aggregateException.Handle(exception =>
        {
            Debug.LogException(exception, exceptionsToThrowWithContextObject[exception]);

            return true;
        });

        throw aggregateException;
    }

    private static void ValidateSerializeFieldsInitialized(Type scriptClass, MonoBehaviour scriptComponent, GameObject gameObject,
        Dictionary<Exception, GameObject> exceptions)
    {
        string[] nonSerializedFields = GetNamesOfFieldsMatchingCondition(
            fields: GetFieldsWithAttribute<RequiredSerializeFieldAttribute>(scriptClass), 
            condition: field =>
            {
                // We check if the field has the SerializeField attribute so that we can give a different exception for this field,
                // since everything with the RequiredSerializeField attribute must also have the SerializeField attribute.
                if (!field.HasAttribute<SerializeField>())
                {
                    exceptions.Add(new MissingSerializeFieldAttributeException($"{field.Name} in {scriptClass.Name} is missing a SerializeField attribute"), gameObject);
                    return false;
                }

                return field.GetValue(scriptComponent).IsUnityNull();
            },
            nameSelector: field => field.Name);
                
        if (!nonSerializedFields.Any())
            return;

        exceptions.Add(BuildException<SerializeFieldNotSetException>(
            gameObject,
            messageBody: $"({scriptClass.Name}) has the following serialize fields not initialized: ", 
            nonSerializedFields), gameObject);
    }

    private static void InitializeComponentsAndThrowIfMissingComponents(Type scriptClass, MonoBehaviour scriptComponent, GameObject gameObject,
        Dictionary<Exception, GameObject> exceptions)
    {
        string[] missingComponents = GetNamesOfFieldsMatchingCondition(
            fields: GetFieldsWithAttribute<ComponentAttribute>(scriptClass), 
            condition: field =>
            {
                ComponentAttribute componentAttribute = field.GetCustomAttribute<ComponentAttribute>();
                Type fieldType = field.FieldType; // We get the datatype of the field

                bool doesComponentExist = scriptComponent.TryGetComponent(fieldType, out Component component);

                if (!doesComponentExist && componentAttribute.InitializeIfMissing)
                {
                    doesComponentExist = true;
                    component = scriptComponent.gameObject.AddComponent(fieldType);
                }

                field.SetValue(scriptComponent, component); // We initialize the field (will be null if not found)

                return !doesComponentExist;
            },
            nameSelector: field => field.FieldType.Name);
        
        if (!missingComponents.Any())
            return;
        
        exceptions.Add(BuildException<MissingComponentException>(
            gameObject,
            messageBody: " is missing the following components: ",
            missingComponents), gameObject);
    }

    private static IEnumerable<FieldInfo> GetFieldsWithAttribute<TAttribute>(Type classType)
        where TAttribute : Attribute
    {
        IEnumerable<FieldInfo> fields = GetFieldsRecursively(classType);
        
        return fields
            .Where(field => field.HasAttribute<TAttribute>()); // We filter out the fields that don't have the requested attribute
    }

    private static IEnumerable<FieldInfo> GetFieldsRecursively(Type classType)
    {
        List<FieldInfo> fields = classType.GetFields(BindingFlags.Public |
                                                     BindingFlags.NonPublic |
                                                     BindingFlags.Instance).ToList();
        
        if (!classType.BaseType.IsUnityNull() && classType.BaseType != typeof(MonoBehaviour))
            fields.AddRange(GetFieldsRecursively(classType.BaseType));

        return fields;
    }

    private static string[] GetNamesOfFieldsMatchingCondition(IEnumerable<FieldInfo> fields,
        Func<FieldInfo, bool> condition, Func<FieldInfo, string> nameSelector) =>
        fields
            .Where(condition)
            .Select(nameSelector)
            .ToArray();
    
    private static TException BuildException<TException>(GameObject gameObject, string messageBody,
        string[] missingFields) where TException : notnull, Exception
    {
        var exceptionMessageBuilder = new StringBuilder();

        exceptionMessageBuilder.Append(gameObject.name);
        exceptionMessageBuilder.Append(messageBody);
    
        foreach (string missingComponentName in missingFields)
            exceptionMessageBuilder.Append(missingComponentName).Append(", ");

        // Complicated way to create an instance of TException.
        return (TException) Activator.CreateInstance(typeof(TException), exceptionMessageBuilder.ToString());
    }

    private static IEnumerable<Type> GetTypesWithCustomAttributes() =>
        AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(
                assembly => assembly.GetTypes(),
                (_, type) => type)
            .Where(type => type.IsAssignableFrom(typeof(MonoBehaviour)))
            .SelectMany(
                type => type.GetFields(),
                (type, field) => new { type, field })
            .Where(typeAndField => typeAndField.field.HasAttribute<ComponentAttribute>() || 
                                   typeAndField.field.HasAttribute<RequiredSerializeFieldAttribute>())
            .Select(typeAndField => typeAndField.type);
}