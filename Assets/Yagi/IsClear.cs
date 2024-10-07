using UnityEngine;

public class IsClear : MonoBehaviour
{
    public static int _stagesCleared = 0;
    public static bool _concealed = false;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 隠しルートでゴールしたかの判定
    /// </summary>
    public void AnotherClear()
    {
        _concealed = true;
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
            PlayerPrefs.SetInt("nowStage", _stagesCleared);
        }
    }
}
