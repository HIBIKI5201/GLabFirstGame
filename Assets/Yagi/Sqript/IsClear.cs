using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IsClear : MonoBehaviour
{
    [SerializeField,Header("SelectStageが設定されているSceaneLoderオブジェクト")] SceneLoader _load;
    public static int _stagesCleared = 0;
    public static bool _concealed = false;
    bool _isbool;
    Timer time;

    private void Start()
    {
        time = GetComponent<Timer>();
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        AnotherClear();
    }

    /// <summary>
    /// 隠しルートでゴールしたかの判定
    /// </summary>
    public void AnotherClear()
    {
        if (_isbool)
        {
            if (time._currentTime <= 0)
            _concealed = true;
            _load.FadeAndLoadScene();
        }
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
