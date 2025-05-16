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
        if (_isPast && GameProgressManager.IsGameCompleted)
        {
            GameProgressManager.IsSecretModeUnlocked = true;
            GameProgressManager.HighestClearedStage = 1;
            Checkpoint checkpoint = FindAnyObjectByType<Checkpoint>();
            checkpoint.ResetPoint();
            PlayerPrefs.SetInt("nowStage", GameProgressManager.HighestClearedStage);
            _load.FadeAndLoadScene();
        }
    }
}
