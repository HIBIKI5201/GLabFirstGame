using DG.Tweening;
using System.Collections;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    [SerializeField] CanvasGroup _stageText;
    [SerializeField] int _nowStage;
    public static bool _isFirst = true; //1��ڂȂ�true(���X�|�[������false�ɂ��Ă���)
    PlayerController _player;
    GameManager _gameManager;
    Timer _timer;
    GameOverSystem _gameOver;
    Checkpoint _checkpoint;
    GoalSequenceManager _goal;
    void Start()
    {
        _goal = GetComponent<GoalSequenceManager>();
        _checkpoint = FindAnyObjectByType<Checkpoint>();
        _gameManager = FindAnyObjectByType<GameManager>();
        _gameOver = FindAnyObjectByType<GameOverSystem>();
        _gameManager.StateType = GameStateType.Playing;
        _timer = FindAnyObjectByType<Timer>();
        _player = GetComponent<PlayerController>();
        _player.StopAction(2f);
        _stageText.gameObject.SetActive(false);
        StartCoroutine(TimerStart(2.5f));
        if (CheckPointManager._checkPoint[_nowStage - 1] != Vector2.zero)
        {
            transform.position = CheckPointManager._checkPoint[_nowStage - 1];
        }
    }

    IEnumerator TimerStart(float time)
    {
        if(_isFirst) _stageText.gameObject.SetActive(true);
        _timer.enabled = false;
        _gameOver.enabled = false;
        yield return new WaitForSeconds(time);
        _timer.enabled = true;
        _gameOver.enabled = true;
        _stageText.DOFade(0, 0.3f).OnComplete(TextHidden);
    }

    /// <summary>
    /// �t�F�[�h������������e�L�X�g���\���ɂ���
    /// </summary>
    void TextHidden()
    {
        _stageText.gameObject.SetActive(false);
        _isFirst = false;
    }
}
