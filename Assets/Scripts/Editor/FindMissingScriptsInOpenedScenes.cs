using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class FindMissingScriptsInOpenedScenes
{
    private static readonly List<GameObject> m_gameObjectsWithMissingScripts = new List<GameObject>();

    [MenuItem("Akazukin/Tools/Check Missing Scripts in Opened Scenes")]
    public static void FindAndSelectMissingScriptsInOpenedScenes()
    {
        m_gameObjectsWithMissingScripts.Clear();

        // Prefab Mode exception:
        var prefabState = PrefabStageUtility.GetCurrentPrefabStage();
        if (prefabState != null)
        {
            FindMissingScriptsFromRootObject(prefabState.prefabContentsRoot);
            HighlightGameObjectsWithMissingScripts(isPrefabMode: true);
            return;
        }

        // Default: check opened scenes:
        var currentLoadedScenes = SceneManager.sceneCount;
        var loadedScenes = new Scene[currentLoadedScenes];

        for (int i = 0; i < currentLoadedScenes; i++)
        {
            loadedScenes[i] = SceneManager.GetSceneAt(i);
        }

        foreach (var scene in loadedScenes)
        {
            FindMissingScriptsInScene(scene);
        }

        HighlightGameObjectsWithMissingScripts(isPrefabMode: false);
    }

    private static void FindMissingScriptsInScene(Scene scene)
    {
        if (scene.isLoaded)
        {
            foreach (var rootObject in scene.GetRootGameObjects())
            {
                FindMissingScriptsFromRootObject(rootObject);
            }
        }
    }

    private static void FindMissingScriptsFromRootObject(GameObject rootObject)
    {
        var children = rootObject.GetComponentsInChildren<Transform>(includeInactive: true);
        foreach (var child in children)
        {
            var components = child.GetComponents<Component>();
            foreach (var component in components)
            {
                if (component == null && !m_gameObjectsWithMissingScripts.Contains(child.gameObject))
                {
                    m_gameObjectsWithMissingScripts.Add(child.gameObject);
                }
            }
        }
    }

    private static void HighlightGameObjectsWithMissingScripts(bool isPrefabMode)
    {
        if (m_gameObjectsWithMissingScripts.Count > 0)
        {
            PrintMissingScriptList();
            Selection.objects = m_gameObjectsWithMissingScripts.ToArray();
        }
        else
        {
            Debug.Log($"開いた{(isPrefabMode ? "プレハブ" : "シーン")}に参照が切れているスクリプトはありません。");
        }
    }

    private static void PrintMissingScriptList()
    {
        if (m_gameObjectsWithMissingScripts.Count <= 0)
        {
            return;
        }

        Debug.LogWarning($"参照が切れているオブジェクト ({m_gameObjectsWithMissingScripts.Count}) を選択しています:\n- {string.Join("\n- ", m_gameObjectsWithMissingScripts)}");
    }
}
