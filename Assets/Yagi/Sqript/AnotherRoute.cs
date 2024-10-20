using UnityEngine;

public class AnotherRoute : MonoBehaviour
{
    [SerializeField, Header("SelectStageが設定されているSceaneLoderオブジェクト")] SceneLoader _load;
    /// <summary>
    /// ある場所を超えたかのフラグ
    /// </summary>
    public bool _isPast;
    Timer time;

    private void Start()
    {
        time = FindAnyObjectByType<Timer>();
    }

    private void Update()
    {
        if (time._currentTime <= 0) IsAnother();
    }

    /// <summary>
    /// ルート分岐をするかどうかの処理
    /// </summary>
    private void IsAnother()
    {
        if (_isPast && IsClear._gameAllClear)
        {
            IsClear._concealed = true;
            IsClear._stagesCleared = 1;
            PlayerPrefs.SetInt("nowStage", IsClear._stagesCleared);
            _load.FadeAndLoadScene();
        }
    }
}
