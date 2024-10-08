using UnityEngine;
using UnityEngine.UI;

public class IsSelect : MonoBehaviour
{
    [SerializeField, Header("ステージのボタンを１から順番に入れてください")] Button[] _stage = { };
    [SerializeField, Header("隠しステージのボタン")] Button _another;

    void Start()
    {
        foreach (var stage in _stage)
        {
            stage.enabled = false;
        }

        if (_another)
        {
            _another.enabled = false;
        }
        ClearCheck();
    }

    private void ClearCheck()
    {
        for (var i = 0; i <= IsClear._stagesCleared; i++)
        {
            _stage[i].enabled = true;
        }

        if (_another)
        {
            if (IsClear._concealed == true)
            {
                _another.enabled = true;
            }
        }
    }

    /// <summary>
    /// リセット機能
    /// </summary>
    public void ResetGame()
    {
        IsClear._stagesCleared = 0;
        PlayerPrefs.SetInt("nowStage", IsClear._stagesCleared);
        IsClear._concealed = false;
        Debug.Log(IsClear._stagesCleared);
    }
}
