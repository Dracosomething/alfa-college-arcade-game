using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
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
public static class DynamicExceptionAttributesHandler
{
    private const string AwakeMethodName = "Awake";
    private const BindingFlags NonStaticMembers = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
    
    private static Dictionary<Type, DynamicExceptionAttributeFieldsWrapper> s_customAttributeFieldsPerTypeMap = new();
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    [InitializeOnLoadMethod]
    public static void PreProcessFilesOnStartUpAndCreateEventHandler()
    {
        List<Exception> exceptions = new();

        s_customAttributeFieldsPerTypeMap = GetExceptionsForAndTypesWithDynamicExceptionAttributes(exceptions);
        
        if (exceptions.Any())
        {
            AggregateException aggregateException = new(exceptions);
            aggregateException.Handle(exception =>
            {
                Debug.LogException(exception);

                return true;
            });
        }
       
        // We check if the application is playing since this method is run twice when starting the game in edit mode,
        // this check makes it so we only hook the listener to sceneLoaded at RuntimeInitializeOnLoadMethod
        if (Application.isPlaying)
            // We hook our HandleDynamicExceptionAttributes to the sceneLoaded event and discard the parameters given by the event
            SceneManager.sceneLoaded += (_, _) => HandleDynamicExceptionAttributes();
    }
    
    /// <summary>
    /// Fils the input exceptions list with the exceptions for the issues found in the classes and also return the types with the fields.
    /// </summary>
    private static Dictionary<Type, DynamicExceptionAttributeFieldsWrapper> GetExceptionsForAndTypesWithDynamicExceptionAttributes(List<Exception> exceptions) =>
        AppDomain.CurrentDomain.GetAssemblies() // We get all assemblies currently loaded
            .SelectMany( // We project the list of assemblies to a list with all types(classes) in those assemblies
                assembly => assembly.GetTypes(),
                (_, type) => type)
            .Where(type => typeof(MonoBehaviour).IsAssignableFrom(type)) // We filter out the types that don't inherit from MonoBehaviour
            .SelectMany( // We project the list of types to a list of all types with the corresponding fields, where one entry is {type, field}
                GetFieldsRecursively, // We get the fields recursively of the type
                (type, field) => new { type, field })
            .Where(typeAndField => typeAndField.field.HasAttribute<ComponentAttribute>() || // We filter out all fields without one of our custom attributes
                                   typeAndField.field.HasAttribute<RequiredSerializeFieldAttribute>())
            .Select(typeAndField => // Does nothing for the result but adds some exceptions to the exceptions list in case fields with RequiredSerializeField don't have SerializeField
            {
                if (typeAndField.field.HasAttribute<RequiredSerializeFieldAttribute>() && !typeAndField.field.HasAttribute<SerializeField>())
                    exceptions.Add(new MissingSerializeFieldAttributeException($"{typeAndField.field.Name} in {typeAndField.type.Name} is missing a SerializeField attribute."));

                return typeAndField;
            })
            .GroupBy( // We parse all fields into a DynamicExceptionAttributeFieldsWrapper and group them by their type
                typeAndField => typeAndField.type,
                typeAndField => typeAndField.field,
                (type, fields) => new { type, fields = new DynamicExceptionAttributeFieldsWrapper(fields) })
            .Select(typeAndFields => // Does nothing but add some exceptions to the exceptions list if any of the types have an Awake method
            {
                if (typeAndFields.type.GetMethod(AwakeMethodName, NonStaticMembers) != null)
                    exceptions.Add(new ClassCannotHaveAwakeException($"{typeAndFields.type.Name} has an Awake method but this doesn't work with the custom Component and RequiredSerializeField attributes."));

                return typeAndFields;
            })
            .ToDictionary( // We convert the result to a dictionary
                typeAndFields => typeAndFields.type,
                typeAndFields => typeAndFields.fields);
    
    private static void HandleDynamicExceptionAttributes()
    {
        Dictionary<Exception, GameObject> exceptionsToThrowWithContextObject = new();

        MonoBehaviour[] loadedScripts = Resources.FindObjectsOfTypeAll<GameObject>()
            .Where(gameObject => gameObject.activeInHierarchy)
            .SelectMany(
                gameObject => gameObject.GetComponents<MonoBehaviour>(),
                (gameObject, scriptComponent) => scriptComponent)
            .Where(scriptComponent => !scriptComponent.IsUnityNull())
            // We want to filter only get the types in s_customAttributeFieldsPerTypeMap contains the type to make the scripts we go through fewer.
            .Where(scriptComponent => s_customAttributeFieldsPerTypeMap.ContainsKey(scriptComponent.GetType()))
            .ToArray();

        foreach (MonoBehaviour scriptComponent in loadedScripts)
        {
            Type scriptClass = scriptComponent.GetType();
            GameObject scriptGameObject = scriptComponent.gameObject;

            ValidateSerializeFieldsInitialized(scriptClass, scriptComponent, scriptGameObject,
                exceptionsToThrowWithContextObject);
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
    }

    private static void ValidateSerializeFieldsInitialized(Type scriptClass, MonoBehaviour scriptComponent,
        GameObject gameObject, Dictionary<Exception, GameObject> exceptions)
    {
        string[] nonSerializedFields = GetNamesOfFieldsMatchingCondition(
            fields: s_customAttributeFieldsPerTypeMap[scriptClass].RequiredSerializeFields, 
            condition: field => field.GetValue(scriptComponent).IsUnityNull(),
            nameSelector: field => field.Name);

        if (!nonSerializedFields.Any())
            return;

        exceptions.Add(BuildException<SerializeFieldNotSetException>(
            gameObject,
            messageBody: $"({scriptClass.Name}) has the following serialize fields not initialized: ",
            nonSerializedFields), gameObject);
    }

    private static void InitializeComponentsAndThrowIfMissingComponents(Type scriptClass, MonoBehaviour scriptComponent,
        GameObject gameObject, Dictionary<Exception, GameObject> exceptions)
    {
        string[] missingComponents = GetNamesOfFieldsMatchingCondition(
            fields: s_customAttributeFieldsPerTypeMap[scriptClass].Components,
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

    private static IEnumerable<FieldInfo> GetFieldsRecursively(Type classType)
    {
        List<FieldInfo> fields = classType.GetFields(NonStaticMembers).ToList();

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
        return (TException)Activator.CreateInstance(typeof(TException), exceptionMessageBuilder.ToString());
    }
}