using System.IO;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SceneAttribute), true)]
public class SceneDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var sceneNames = GetSceneNames();
        if (property.propertyType == SerializedPropertyType.Integer)
        {
            property.intValue = EditorGUI.Popup(position, label.text, property.intValue, sceneNames);
        }
        else if (property.propertyType == SerializedPropertyType.String)
        {
            var index = GetSceneIndexByName(property.stringValue);
            index = EditorGUI.Popup(position, label.text, index, sceneNames);
            property.stringValue = GetSceneNameByIndex(index);
        }
        else
        {
            EditorGUI.LabelField(position, "SceneAttribute only works on integers and strings.");
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    private string[] GetSceneNames()
    {
        var names = new string[EditorBuildSettings.scenes.Length + 1];
        names[0] = "(none)";
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            names[i + 1] = Path.GetFileNameWithoutExtension(EditorBuildSettings.scenes[i].path);
        }
        return names;
    }

    private string GetSceneNameByIndex(int index)
    {
        var sceneNames = GetSceneNames();
        return index == 0 ? string.Empty : (0 <= index && index < sceneNames.Length) ? sceneNames[index] : string.Empty;
    }

    private int GetSceneIndexByName(string sceneName)
    {
        var sceneNames = GetSceneNames();
        if (sceneNames == null || sceneName.Length <= 0)
        {
            return 0;
        }

        for (int i = 0; i < sceneNames.Length; i++)
        {
            if (sceneName.Equals(sceneNames[i]))
            {
                return i;
            }
        }
        return 0;
    }
}
