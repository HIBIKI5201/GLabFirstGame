using UnityEngine;

public class AnotherRoute : MonoBehaviour
{
    [SerializeField, Header("SelectStage���ݒ肳��Ă���SceaneLoder�I�u�W�F�N�g")] SceneLoader _load;
    /// <summary>
    /// ����ꏊ�𒴂������̃t���O
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
    /// ���[�g��������邩�ǂ����̏���
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
