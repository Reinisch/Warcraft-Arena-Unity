using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnumFlagAttribute))]
public class EnumFlagDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        string[] nonMaskOptions = property.enumNames.Where(enumName => !enumName.Contains("Mask")).ToArray();
        for (int i = 0; i < nonMaskOptions.Length; i++)
            nonMaskOptions[i] = ObjectNames.NicifyVariableName(nonMaskOptions[i]);

        property.intValue = EditorGUI.MaskField(position, label, property.intValue, nonMaskOptions);
    }
}