using System.Collections.Generic;
using System.Text;

public class CollectionHelper
{
    public static string ConvertCollectionToProperlyFormattedString<T>(IEnumerable<T> collection)
    {
        var stringBuilder = new StringBuilder();
        
        foreach (var entry in collection)
            stringBuilder.Append(entry).Append(", ");

        return stringBuilder.ToString();
    }
}