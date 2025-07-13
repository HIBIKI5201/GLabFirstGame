using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(RequiredAttribute), true)]
public class RequiredDrawer : PropertyDrawer
{
    private static readonly List<string> _propertiesNotFound = new List<string>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.isArray)
        {
            for (int i = 0; i < property.arraySize; i++)
            {
                var item = property.GetArrayElementAtIndex(i);
                DrawPropertyField(position, item, label);
            }
        }
        else
        {
            DrawPropertyField(position, property, label);
        }
    }

    private void DrawPropertyField(Rect position, SerializedProperty property, GUIContent label)
    {
        var isPropertyExist = property.objectReferenceValue != null;

        var cacheBackgroundColor = GUI.backgroundColor;
        GUI.backgroundColor = isPropertyExist ? cacheBackgroundColor : Color.red;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.backgroundColor = cacheBackgroundColor;

        var context = property.serializedObject.targetObject;
        var propertyInternalFullPath = $"{context.GetHashCode()}.{property.propertyPath}";
        if (isPropertyExist)
        {
            if (_propertiesNotFound.Contains(propertyInternalFullPath))
            {
                _propertiesNotFound.Remove(propertyInternalFullPath);
            }
        }
        else
        {
            if (!_propertiesNotFound.Contains(propertyInternalFullPath))
            {
                _propertiesNotFound.Add(propertyInternalFullPath);
                Debug.LogError($"A required property \"{property.displayName}\" on GameObject \"{context.name}\" is not assigned, click here to ping the missing context.\nSerialized path: {property.propertyPath}.", context);
            }
        }
    }
}