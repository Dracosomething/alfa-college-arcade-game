using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;

public class DynamicExceptionAttributeFieldsWrapper
{
    public List<FieldInfo> Components { get; }
    public List<FieldInfo> RequiredSerializeFields { get; }

    public DynamicExceptionAttributeFieldsWrapper(IEnumerable<FieldInfo> fieldInfos)
    {
        // We project fieldInfos to a list to avoid multiple enumerations
        List<FieldInfo> fieldInfoList = fieldInfos.ToList();
        
        Components = fieldInfoList
            .Where(field => field.HasAttribute<ComponentAttribute>())
            .ToList();
        
        RequiredSerializeFields = fieldInfoList
            .Where(field => field.HasAttribute<RequiredSerializeFieldAttribute>())
            .ToList();
    }
}