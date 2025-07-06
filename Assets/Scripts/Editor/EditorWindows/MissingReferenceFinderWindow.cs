using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

public class MissingReferenceFinderWindow : EditorWindow
{
    private static readonly List<GameObject> s_objectsWithMissingScripts = new List<GameObject>();
    private static readonly List<(Component, string, string)> s_componentsWithMissingReferences = new List<(Component, string, string)>();

    private static Vector2 _mainScroll;
    private static Vector2 _scroll1;
    private static Vector2 _scroll2;
    private static GUIStyle _divider = null;

    private static GUIStyle Divider
    {
        get
        {
            if (_divider == null)
            {
                var whiteTexture = new Texture2D(1, 1);
                whiteTexture.SetPixel(0, 0, Color.white);
                whiteTexture.Apply();
                _divider = new GUIStyle();
                _divider.normal.background = whiteTexture;
                _divider.margin = new RectOffset(2, 2, 2, 2);
            }
            return _divider;
        }
    }

    [MenuItem("Akazukin/Window/Missing Reference Finder...")]
    private static void Init()
    {
        var window = GetWindow<MissingReferenceFinderWindow>();
        window.titleContent = new GUIContent("Missing Reference Finder");
        window.Show();
    }

    private void OnGUI()
    {
        _mainScroll = GUILayout.BeginScrollView(_mainScroll);
        GUILayout.Label($"Missing Script がある GameObjects ({s_objectsWithMissingScripts.Count})：", EditorStyles.boldLabel);
        if (s_objectsWithMissingScripts.Count > 0)
        {
            _scroll1 = GUILayout.BeginScrollView(_scroll1);
            foreach (var gameObject in s_objectsWithMissingScripts)
            {
                GUI.enabled = false;
                GUILayout.BeginHorizontal();
                GUILayout.Label($"    {gameObject.name}");
                GUILayout.FlexibleSpace();
                EditorGUILayout.ObjectField(gameObject, typeof(GameObject), allowSceneObjects: false);
                GUILayout.EndHorizontal();
                GUI.enabled = true;
            }
            GUILayout.EndScrollView();
        }
        else
        {
            ColoredLabel($"    \u2713 開いている{(IsCurrentlyInPrefabMode(out _) ? " Prefab " : "シーン")}は Missing Script がありません。", Color.green);
        }

        HorizontalLine(1, Color.white, 3);

        GUILayout.Label($"Missing References がある Components ({s_componentsWithMissingReferences.Count})：", EditorStyles.boldLabel);
        if (s_componentsWithMissingReferences.Count > 0)
        {
            _scroll2 = GUILayout.BeginScrollView(_scroll2);
            foreach (var component in s_componentsWithMissingReferences)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label($"    {component.Item2} ({component.Item3})");
                GUILayout.FlexibleSpace();
                GUI.enabled = false;
                EditorGUILayout.ObjectField(component.Item1, typeof(Component), allowSceneObjects: false);
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }
        else
        {
            ColoredLabel($"    \u2713 開いている{(IsCurrentlyInPrefabMode(out _) ? " Prefab " : "シーン")}は Missing Reference がありません。", Color.green);
        }

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("結果をリフレッシュ", GUILayout.Height(30)))
        {
            RefreshResults();
            SelectMissingObjectsInHierarchy();
        }

        GUILayout.EndScrollView();
    }

    private bool IsCurrentlyInPrefabMode(out PrefabStage prefabStage)
    {
        prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        return prefabStage != null;
    }

    private void RefreshResults()
    {
        s_objectsWithMissingScripts.Clear();
        s_componentsWithMissingReferences.Clear();

        // If prefab mode is on: check prefab
        if (IsCurrentlyInPrefabMode(out var prefabState))
        {
            FindMissingReferencesInGameObject(prefabState.prefabContentsRoot, includeChildren: true);
        }
        // Else: check all opened scenes
        else
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                FindMissingReferencesInScene(SceneManager.GetSceneAt(i));
            }
        }
    }

    private void FindMissingReferencesInScene(Scene scene)
    {
        if (scene.isLoaded)
        {
            foreach (var rootObject in scene.GetRootGameObjects())
            {
                FindMissingReferencesInGameObject(rootObject, includeChildren: true);
            }
        }
    }

    private void FindMissingReferencesInGameObject(GameObject gameObject, bool includeChildren)
    {
        if (gameObject == null)
        {
            return;
        }

        var components = gameObject.GetComponents<Component>();
        foreach (var component in components)
        {
            if (component == null)
            {
                // Component is missing
                if (!s_objectsWithMissingScripts.Contains(gameObject))
                {
                    s_objectsWithMissingScripts.Add(gameObject);
                }
            }
            else
            {
                var componentSerializedObject = new SerializedObject(component);
                var propertyIterator = componentSerializedObject.GetIterator();

                while (propertyIterator.NextVisible(true))
                {
                    if (propertyIterator.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        if (propertyIterator.objectReferenceInstanceIDValue != 0 && propertyIterator.objectReferenceValue == null)
                        {
                            // Property ID exists but the reference is null
                            // Therefore a reference is missing
                            s_componentsWithMissingReferences.Add((component, propertyIterator.displayName, propertyIterator.propertyPath));
                        }
                    }
                }
            }
        }

        if (includeChildren)
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                FindMissingReferencesInGameObject(gameObject.transform.GetChild(i).gameObject, true);
            }
        }
    }

    private void SelectMissingObjectsInHierarchy()
    {
        var gameObjectsToSelect = new List<GameObject>();
        if (s_componentsWithMissingReferences.Count > 0)
        {
            foreach (var item in s_componentsWithMissingReferences)
            {
                if (!gameObjectsToSelect.Contains(item.Item1.gameObject))
                {
                    gameObjectsToSelect.Add(item.Item1.gameObject);
                }
            }
        }
        gameObjectsToSelect.AddRange(s_objectsWithMissingScripts);
        if (gameObjectsToSelect.Count > 0)
        {
            Selection.objects = gameObjectsToSelect.ToArray();
            Debug.LogWarning($"参照が切れているオブジェクト ({gameObjectsToSelect.Count}) を選択しています:\n- {string.Join("\n- ", gameObjectsToSelect)}");
        }
    }

    private static void ColoredLabel(string content, Color color)
    {
        var cachedContentColor = GUI.color;
        GUI.contentColor = color;
        GUILayout.Label(content);
        GUI.contentColor = cachedContentColor;
    }

    private static void HorizontalLine(int height, Color color, float margin)
    {
        Divider.fixedHeight = height;
        var cachedGUIColor = GUI.color;
        GUI.color = color;
        GUILayout.Space(margin);
        GUILayout.Box(GUIContent.none, Divider);
        GUILayout.Space(margin);
        GUI.color = cachedGUIColor;
    }
}
