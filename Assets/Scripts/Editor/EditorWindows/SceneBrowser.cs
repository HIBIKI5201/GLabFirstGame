using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneBrowser : EditorWindow
{
    private const int k_contextButtonWidth = 110;
    private static Vector2 s_scrollView = default;
    private static GUIStyle s_divider = null;

    [SerializeField] private bool _showScenesNotInBuildSettings = false;
    [SerializeField] private bool _notInBuildSettingsFoldout = false;

    private static GUIStyle Divider
    {
        get
        {
            if (s_divider == null)
            {
                var whiteTexture = new Texture2D(1, 1);
                whiteTexture.SetPixel(0, 0, Color.white);
                whiteTexture.Apply();
                s_divider = new GUIStyle();
                s_divider.normal.background = whiteTexture;
                s_divider.margin = new RectOffset(2, 2, 2, 2);
            }
            return s_divider;
        }
    }

    [MenuItem("Akazukin/Window/Scene Browser...")]
    private static void Init()
    {
        var window = GetWindow<SceneBrowser>();
        window.titleContent = new GUIContent("Scene Browser");
        window.minSize = new Vector2(590, window.minSize.y);
        window.Show();
    }

    private bool ColoredButton(string content, Color color, params GUILayoutOption[] options)
    {
        var cachedBackgroundColor = GUI.backgroundColor;
        GUI.backgroundColor = color;
        if (GUILayout.Button(content, options))
        {
            GUI.backgroundColor = cachedBackgroundColor;
            return true;
        }
        GUI.backgroundColor = cachedBackgroundColor;
        return false;
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

    private void OnGUI()
    {
        GUILayout.Label("Scene Browser", EditorStyles.boldLabel);
        _showScenesNotInBuildSettings = EditorGUILayout.Toggle("Show Scenes not in Build", _showScenesNotInBuildSettings);

        if (GUILayout.Button("Build Settings...", GUILayout.Height(25)))
        {
            GetWindow(Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
        }

        HorizontalLine(1, Color.white, 3);

        s_scrollView = GUILayout.BeginScrollView(s_scrollView);
        GUILayout.Label("Scenes in Build Settings", EditorStyles.boldLabel);
        var scenes = EditorBuildSettings.scenes;
        if (scenes == null || scenes.Length <= 0)
        {
            EditorGUILayout.HelpBox("There are no Scenes in the Build Settings.", MessageType.Info);
        }
        else
        {
            HorizontalLine(1, Color.white, 3);
            for (int i = 0; i < scenes.Length; i++)
            {
                DrawSceneSlot(scenes[i].path, i);
            }
        }

        if (_showScenesNotInBuildSettings)
        {
            EditorGUILayout.Space();
            _notInBuildSettingsFoldout = EditorGUILayout.Foldout(_notInBuildSettingsFoldout, "Scenes not in Build Settings", true);
            if (_notInBuildSettingsFoldout)
            {
                var allScenesGUID = AssetDatabase.FindAssets("t:scene");
                var scenesNotInBuildSettings = new List<string>();
                if (allScenesGUID != null && allScenesGUID.Length > 0)
                {
                    foreach (var sceneGUID in allScenesGUID)
                    {
                        var sceneAssetPath = AssetDatabase.GUIDToAssetPath(sceneGUID);
                        if (!IsSceneAssetInBuildSettings(sceneAssetPath))
                        {
                            scenesNotInBuildSettings.Add(sceneAssetPath);
                        }
                    }
                }
                if (scenesNotInBuildSettings == null || scenesNotInBuildSettings.Count <= 0)
                {
                    EditorGUILayout.HelpBox($"No scenes can be found.", MessageType.Info);
                }
                else
                {
                    EditorGUI.indentLevel++;
                    foreach (var scene in scenesNotInBuildSettings)
                    {
                        DrawSceneSlot(scene);
                    }
                    EditorGUI.indentLevel--;
                }
            }
        }
        GUILayout.EndScrollView();
    }

    private void DrawSceneSlot(string sceneAssetPath, int? buildIndex = null)
    {
        var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneAssetPath);
        if (sceneAsset == null)
        {
            return;
        }

        var sceneName = sceneAsset.name;

        var label = buildIndex != null ? $"[{buildIndex}] {sceneName}" : sceneName;
        if (IsSceneActive(sceneName))
        {
            GUILayout.Label(label, EditorStyles.boldLabel);
        }
        else
        {
            GUILayout.Label(label);
        }

        GUILayout.BeginHorizontal();
        var cacheGUIState = GUI.enabled;
        GUI.enabled = false;
        EditorGUILayout.ObjectField(sceneAsset, typeof(SceneAsset), true, GUILayout.Width(200));
        GUILayout.FlexibleSpace();

        var isSceneLoaded = IsSceneLoaded(sceneName);

        GUI.enabled = !isSceneLoaded;
        if (isSceneLoaded)
        {
            if (IsSceneActive(sceneName))
            {
                GUILayout.Label("Is Active Scene", GUILayout.Width(k_contextButtonWidth));
            }
            else
            {
                GUI.enabled = true;
                if (ColoredButton("Set Active", Color.yellow, GUILayout.Width(k_contextButtonWidth)))
                {
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
                }
            }
        }
        else
        {
            if (ColoredButton("Open (Additive)", Color.green, GUILayout.Width(k_contextButtonWidth)))
            {
                EditorSceneManager.OpenScene(sceneAssetPath, OpenSceneMode.Additive);
            }
        }

        GUI.enabled = !isSceneLoaded;
        if (ColoredButton("Open (Single)", Color.green, GUILayout.Width(k_contextButtonWidth)))
        {
            EditorSceneManager.OpenScene(sceneAssetPath, OpenSceneMode.Single);
        }

        GUI.enabled = GetNumberOfEnabledScenes() > 1;
        if (isSceneLoaded)
        {
            if (ColoredButton("Close", Color.red, GUILayout.Width(k_contextButtonWidth)))
            {
                EditorSceneManager.CloseScene(SceneManager.GetSceneByName(sceneAsset.name), false);
            }
        }
        else
        {
            if (ColoredButton("Remove", Color.red, GUILayout.Width(k_contextButtonWidth)))
            {
                EditorSceneManager.CloseScene(SceneManager.GetSceneByName(sceneAsset.name), true);
            }
        }

        GUI.enabled = cacheGUIState;
        GUILayout.EndHorizontal();
        HorizontalLine(1, Color.white, 3);
    }

    private bool IsSceneAssetInBuildSettings(string sceneAssetPath)
    {
        if (EditorBuildSettings.scenes.Length > 0)
        {
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.path == sceneAssetPath)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsSceneActive(string sceneName)
    {
        return SceneManager.GetActiveScene() == SceneManager.GetSceneByName(sceneName);
    }

    private bool IsSceneLoaded(string sceneName)
    {
        var scene = SceneManager.GetSceneByName(sceneName);
        return scene != null && scene.isLoaded;
    }

    private int GetNumberOfEnabledScenes()
    {
        int result = 0;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (scene != null && scene.isLoaded)
            {
                result++;
            }
        }
        return result;
    }
}
