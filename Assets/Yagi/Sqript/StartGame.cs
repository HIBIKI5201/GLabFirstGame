using DG.Tweening;
using System.Collections;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    [SerializeField] CanvasGroup _stageText;
    public static bool _isFirst = true; //1回目ならtrue(リスポーン時はfalseにしておく)
    PlayerController _player;
    GameManager _gameManager;
    Timer _timer;
    GameOverSystem _gameOver;
    Checkpoint _checkpoint;
    Goal _goal;
    void Start()
    {
        _goal = GetComponent<Goal>();
        _checkpoint = FindAnyObjectByType<Checkpoint>();
        _gameManager = FindAnyObjectByType<GameManager>();
        _gameOver = FindAnyObjectByType<GameOverSystem>();
        _gameManager.State = GameManager.GameState.Playing;
        if (Checkpoint._checkpoint != Vector2.zero)
        {
            if (_goal._nowStage == 3) Checkpoint._checkpoint = new Vector2(Checkpoint._checkpoint.x, Checkpoint._checkpoint.y + 1.034f) ;
            transform.position = Checkpoint._checkpoint;
        }
        _timer = FindAnyObjectByType<Timer>();
        _player = GetComponent<PlayerController>();
        _player.StopAction(2f);
        _stageText.gameObject.SetActive(false);
        StartCoroutine(TimerStart(2.5f));
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
