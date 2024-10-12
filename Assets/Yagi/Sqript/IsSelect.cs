using UnityEngine;
using UnityEngine.UI;

public class IsSelect : MonoBehaviour
{
    [SerializeField, Header("�X�e�[�W�̃{�^�����P���珇�Ԃɓ���Ă�������")] Button[] _stage = { };
    [SerializeField, Header("�B���X�e�[�W�̃{�^��")] Button _another;

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
