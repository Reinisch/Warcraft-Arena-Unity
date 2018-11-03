using System.Collections.Generic;
using UnityEditor;

public static class EditorSerializationHelper
{
    public static IEnumerable<SerializedProperty> GetVisibleChildren(SerializedProperty property)
    {
        property = property.Copy();
        var nextElement = property.Copy();
        bool hasNextElement = nextElement.NextVisible(false);
        if (!hasNextElement)
            nextElement = null;

        property.NextVisible(true);
        while (true)
        {
            if ((SerializedProperty.EqualContents(property, nextElement)))
                yield break;

            yield return property;

            bool hasNext = property.NextVisible(false);
            if (!hasNext)
                break;
        }
    }
}
