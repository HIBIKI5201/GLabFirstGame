using DG.Tweening;
using System.Collections;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    [SerializeField] CanvasGroup _stageText;
    [SerializeField] int _nowStage;
    public static bool _isFirst = true; //1回目ならtrue(リスポーン時はfalseにしておく)
    PlayerController _player;
    GameManager _gameManager;
    CheckPointManager _checkPointManager;
    Timer _timer;
    GameOverSystem _gameOver;
    Checkpoint _checkpoint;
    Goal _goal;
    void Start()
    {
        _checkPointManager = FindAnyObjectByType<CheckPointManager>();
        _goal = GetComponent<Goal>();
        _checkpoint = FindAnyObjectByType<Checkpoint>();
        _gameManager = FindAnyObjectByType<GameManager>();
        _gameOver = FindAnyObjectByType<GameOverSystem>();
        _gameManager.State = GameManager.GameState.Playing;
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
    /// フェードが完了したらテキストを非表示にする
    /// </summary>
    void TextHidden()
    {
        _stageText.gameObject.SetActive(false);
        _isFirst = false;
    }
}
