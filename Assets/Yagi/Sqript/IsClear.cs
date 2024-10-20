using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IsClear : MonoBehaviour
{
    public static int _stagesCleared = 0;
    public static bool _concealed = false;
    /// <summary>
    /// ゲームを一周クリアしたかのフラグ
    /// </summary>
    public static bool _gameAllClear = false;
    bool _isbool;
    Timer time;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    /// <summary>
    /// ステージを開放するかどうかの判定
    /// </summary>
    /// <param name="stage"></param>
    public void StageClear(int stage)
    {
        if (_stagesCleared < stage)
        {
            _stagesCleared = stage;
            if (_stagesCleared == 3)
            {
                _gameAllClear = true;
            }
            PlayerPrefs.SetInt("nowStage", _stagesCleared);
        }
    }
}
