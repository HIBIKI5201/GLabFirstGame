using UnityEngine;
using UnityEngine.UI;

public class IsSelect : MonoBehaviour
{
    [SerializeField, Header("ステージのボタンを１から順番に入れてください")] Button[] _stage = { };

    void Start()
    {
        foreach (var stage in _stage)
        {
            stage.enabled = false;
            stage.image.color = Color.gray;
        }
        ClearCheck();
    }

    private void ClearCheck()
    {
        for (var i = 0; i <= IsClear._stagesCleared; i++)
        {
            _stage[i].enabled = true;
            _stage[i].image.color = Color.white;
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
        Debug.Log($"現在のクリア状況{IsClear._stagesCleared}");
        foreach (var stage in _stage)
        {
            stage.enabled = false;
            stage.image.color = Color.gray;
        }
        ClearCheck();
    }
}
