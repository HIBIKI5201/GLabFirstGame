using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ReferenceFinderWindow : EditorWindow
{
    private static Vector2 _scroll = default;
    private static GUIStyle _divider = null;

    [SerializeField] private int _mode = 0;

    // Find what
    [SerializeField] private UnityEngine.Object _findWhatObject = null;
    [SerializeField] private string _findWhatObjectGUID = string.Empty;

    // Replace with
    [SerializeField] private UnityEngine.Object _replaceWithObject = null;
    [SerializeField] private string _replaceWithObjectGUID = string.Empty;

    [SerializeField] private string _searchFolder = "Assets/";
    [SerializeField] private bool _searchFilterFoldout = false;

    // Search types
    [SerializeField] private bool _searchScenes = true;
    [SerializeField] private bool _searchPrefabs = true;
    [SerializeField] private bool _searchScriptableObjects = true;

    [SerializeField] private List<UnityEngine.Object> _searchedResults = new List<UnityEngine.Object>();

    private readonly System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();
    private int _searchTimeInMilliseconds = 0;

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

    [MenuItem("Akazukin/Window/Reference Finder...")]
    private static void Init()
    {
        var window = GetWindow<ReferenceFinderWindow>();
        window.titleContent = new GUIContent("Reference Finder");
        window.Show();
    }

    private void OnGUI()
    {
        var cacheBackgroundColor = GUI.backgroundColor;
        GUILayout.BeginHorizontal();
        GUI.backgroundColor = _mode == 0 ? Color.yellow : Color.white;
        if (GUILayout.Button("検索", GUILayout.Height(40)))
        {
            _mode = 0;
        }
        GUI.backgroundColor = _mode == 1 ? Color.yellow : Color.white;
        if (GUILayout.Button("検索と置換", GUILayout.Height(40)))
        {
            _mode = 1;
        }
        GUI.backgroundColor = cacheBackgroundColor;
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        GUILayout.Label($"{(_mode == 0 ? "検索" : _mode == 1 ? "検索と置換" : "")}", EditorStyles.boldLabel);
        _findWhatObject = EditorGUILayout.ObjectField("なにを検索", _findWhatObject, typeof(UnityEngine.Object), allowSceneObjects: true);
        _findWhatObjectGUID = _findWhatObject ? AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(_findWhatObject)).ToString() : "<None>";

        GUI.enabled = false;
        EditorGUI.indentLevel++;
        EditorGUILayout.TextField("GUID （リードオンリー）", _findWhatObjectGUID);
        EditorGUI.indentLevel--;
        GUI.enabled = true;

        if (_mode == 1)
        {
            EditorGUILayout.Space();

            _replaceWithObject = EditorGUILayout.ObjectField("なにに置換", _replaceWithObject, typeof(UnityEngine.Object), allowSceneObjects: true);
            _replaceWithObjectGUID = _replaceWithObject ? AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(_replaceWithObject)).ToString() : "<None>";

            GUI.enabled = false;
            EditorGUI.indentLevel++;
            EditorGUILayout.TextField("GUID （リードオンリー）", _replaceWithObjectGUID);
            EditorGUI.indentLevel--;
            GUI.enabled = true;
        }

        EditorGUILayout.Space();
        HorizontalLine(1, Color.white, 3);

        GUILayout.Label("オプション", EditorStyles.boldLabel);

        using (var _ = new GUILayout.HorizontalScope())
        {
            _searchFolder = EditorGUILayout.TextField("フォルダに検索", _searchFolder);
            if (GUILayout.Button("ブラウス", GUILayout.Width(60)))
            {
                _searchFolder = EditorUtility.OpenFolderPanel("Browse Search Folder", _searchFolder, string.Empty);
                if (_searchFolder.ToLower().Contains("assets"))
                {
                    _searchFolder = _searchFolder.Substring(_searchFolder.ToLower().IndexOf("assets"));
                }
            }
        }

        _searchFilterFoldout = EditorGUILayout.Foldout(_searchFilterFoldout, "以下のタイプだけ検索:", true);
        if (_searchFilterFoldout)
        {
            EditorGUI.indentLevel++;
            _searchScenes = EditorGUILayout.Toggle(new GUIContent("Scenes", ".unity"), _searchScenes);
            _searchPrefabs = EditorGUILayout.Toggle(new GUIContent("Prefabs", ".prefab"), _searchPrefabs);
            _searchScriptableObjects = EditorGUILayout.Toggle(new GUIContent("ScriptableObjects", ".asset"), _searchScriptableObjects);
            EditorGUI.indentLevel--;
        }

        GUI.enabled = _mode == 0 ? _findWhatObject : _mode == 1 ? _findWhatObject && _replaceWithObject : false;
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button($"{(_mode == 0 ? "検索" : _mode == 1 ? "検索と置換" : "")}", GUILayout.Height(35)))
        {
            if (_mode == 1)
            {
                if (EditorUtility.DisplayDialog("検索と置換を確認", $"すべての \"{_findWhatObject.name}\" を \"{_replaceWithObject.name}\" に置換しますか？\nこの作業は復旧できません！", "Ok", "キャンセル"))
                {
                    _stopwatch.Reset();
                    _stopwatch.Start();
                    _searchedResults = Search(_findWhatObjectGUID, _searchFolder, GetFilters());
                    if (_mode == 1)
                    {
                        Replace(_searchedResults, _findWhatObjectGUID, _replaceWithObjectGUID);
                    }
                    _stopwatch.Stop();
                    _searchTimeInMilliseconds = (int)_stopwatch.ElapsedMilliseconds;
                }
            }
            else
            {
                _stopwatch.Reset();
                _stopwatch.Start();
                _searchedResults = Search(_findWhatObjectGUID, _searchFolder, GetFilters());
                _stopwatch.Stop();
                _searchTimeInMilliseconds = (int)_stopwatch.ElapsedMilliseconds;
            }
        }
        GUI.backgroundColor = cacheBackgroundColor;
        GUI.enabled = true;

        EditorGUILayout.Space();
        HorizontalLine(1, Color.white, 3);

        using (var _ = new GUILayout.HorizontalScope())
        {
            GUILayout.Label($"検索結果 ({_searchedResults.Count}):", EditorStyles.boldLabel);
            if (GUILayout.Button("リストをクリア"))
            {
                _searchedResults.Clear();
            }
        }

        _scroll = GUILayout.BeginScrollView(_scroll);
        if (_searchedResults.Count <= 0)
        {
            GUILayout.Label("検索結果なし。");
        }
        else
        {
            GUILayout.Label($"検索時間: {_searchTimeInMilliseconds} ms.");
            GUI.enabled = false;
            foreach (var result in _searchedResults)
            {
                EditorGUILayout.ObjectField(result.name, result, typeof(UnityEngine.Object), allowSceneObjects: true);
            }
            GUI.enabled = true;
        }
        GUILayout.EndScrollView();
        GUI.backgroundColor = cacheBackgroundColor;
    }

    private string[] GetFilters()
    {
        var filters = new List<string>();
        if (_searchScenes)
        {
            filters.Add("t:scene");
        }
        if (_searchPrefabs)
        {
            filters.Add("t:prefab");
        }
        if (_searchScriptableObjects)
        {
            filters.Add("t:scriptableobject");
        }
        return filters.ToArray();
    }

    private List<UnityEngine.Object> Search(string targetAssetGUID, string searchInFolder, params string[] filters)
    {
        var results = new List<UnityEngine.Object>();
        var pendingGUIDs = AssetDatabase.FindAssets(string.Join(" ", filters), new string[] { searchInFolder });
        if (pendingGUIDs != null && pendingGUIDs.Length > 0)
        {
            for (int i = 0; i < pendingGUIDs.Length; i++)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(pendingGUIDs[i]);
                if (EditorUtility.DisplayCancelableProgressBar($"検索中 {assetPath} ...", $"{pendingGUIDs.Length} アセットの {i} を検索しています... {_searchedResults.Count} を見つけました.", (float)i / pendingGUIDs.Length))
                {
                    break;
                }
                try
                {
                    // Do not parse paths that are directories. They throw errors when parsing.
                    if (!Directory.Exists(assetPath) && File.ReadAllText(assetPath).Contains(targetAssetGUID))
                    {
                        results.Add(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath));
                    }
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }
        EditorUtility.ClearProgressBar();
        return results;
    }

    private void Replace(List<UnityEngine.Object> listOfObjectsContainingTargetGUID, string targetGUIDToBeReplaced, string replaceWithThisGUID)
    {
        if (listOfObjectsContainingTargetGUID.Count <= 0 && string.IsNullOrEmpty(targetGUIDToBeReplaced) || string.IsNullOrEmpty(replaceWithThisGUID))
        {
            Debug.Log($"置換必要なファイルがないため置換作業を無視しました。");
            return;
        }

        for (int i = 0; i < listOfObjectsContainingTargetGUID.Count; i++)
        {
            var obj = listOfObjectsContainingTargetGUID[i];
            var assetPath = AssetDatabase.GetAssetPath(obj);
            if (EditorUtility.DisplayCancelableProgressBar($"置換中 {assetPath} ...", $"{listOfObjectsContainingTargetGUID.Count} アセットの {i} を検索しています", (float)i / listOfObjectsContainingTargetGUID.Count))
            {
                break;
            }
            try
            {
                var newTextContent = File.ReadAllText(assetPath).Replace(targetGUIDToBeReplaced, replaceWithThisGUID);
                File.WriteAllText(assetPath, newTextContent);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }
        Debug.Log($"{listOfObjectsContainingTargetGUID.Count} ファイルを置換しました。");
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }
    public static void HorizontalLine(int height, Color color, float margin)
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
