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
            _clearObj[0].SetActive(false);
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

        switch (IsClear._stagesCleared)
        {
            case 0:
                break;
            case 1:
                _clearObj[0].SetActive(true);
                break;
            case 2:
                _clearObj[1].SetActive(true);
                break;
            case 3:
                _clearObj[2].SetActive(true);
                break;
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
