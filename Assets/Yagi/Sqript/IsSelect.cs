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
            if(i != 0) _clearObj[i-1].SetActive(true);
        }
    }

    /// <summary>
    /// ���Z�b�g�@�\
    /// </summary>
    public void ResetGame()
    {
        IsClear._stagesCleared = 0;
        PlayerPrefs.SetInt("nowStage", IsClear._stagesCleared);
        IsClear._concealed = false;
        Debug.Log(IsClear._stagesCleared);
    }
}
