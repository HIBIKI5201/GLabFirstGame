using UnityEngine;
using UnityEngine.UI;

public class IsSelect : MonoBehaviour
{
    [SerializeField, Header("ステージのボタンを１から順番に入れてください")] Button[] _stage = { };
    [SerializeField] GameObject[] _clearObj = new GameObject[3];

    void Start()
    {
        foreach (var stage in _stage)
        {
            stage.enabled = false;
            stage.image.color = Color.gray;
        }
        foreach (var clearObj in _clearObj)
        {
            clearObj.SetActive(false);
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

        for (var i = 1; i <= IsClear._stagesCleared; i++)
        {
            _clearObj[i - 1].SetActive(true);
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
        foreach (var stage in _stage)
        {
            stage.enabled = false;
            stage.image.color = Color.gray;
        }
        foreach (var clearObj in _clearObj)
        {
            clearObj.SetActive(false);
        }
        ClearCheck();
        Debug.Log(IsClear._stagesCleared);
    }
}
