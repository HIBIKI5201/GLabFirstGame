using UnityEngine;
using UnityEngine.UI;

public class IsSelect : MonoBehaviour
{
    [SerializeField, Header("�X�e�[�W�̃{�^�����P���珇�Ԃɓ���Ă�������")] Button[] _stage = { };
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
        for (var i = 0; i <= GameProgressManager.HighestClearedStage; i++)
        {
            if(GameProgressManager.HighestClearedStage == 3)break;
            _stage[i].enabled = true;
            _stage[i].image.color = Color.white;
        }

        for (var i = 1; i <= GameProgressManager.HighestClearedStage; i++)
        {
            _clearObj[i - 1].SetActive(true);
        }
    }

    public void ResetGame()
    {
        GameProgressManager.HighestClearedStage = 0;
        PlayerPrefs.SetInt("nowStage", GameProgressManager.HighestClearedStage);
        GameProgressManager.IsSecretModeUnlocked = false;
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
        Debug.Log(GameProgressManager.HighestClearedStage);
    }
}
