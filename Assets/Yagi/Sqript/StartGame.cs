using DG.Tweening;
using System.Collections;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    [SerializeField] CanvasGroup _stageText;
    PlayerController _player;
    GameManager _gameManager;
    Timer _timer;
    GameOverSystem _gameOver;
    void Start()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
        _gameOver = FindAnyObjectByType<GameOverSystem>();
        _gameManager.State = GameManager.GameState.Playing;
        if (GameOver.position != Vector2.zero) transform.position = GameOver.position;
        _timer = FindAnyObjectByType<Timer>();
        _player = GetComponent<PlayerController>();
        _player.StopAction(2f);
        StartCoroutine(TimerStart(2.5f));
    }

    IEnumerator TimerStart(float time)
    {
        _stageText.gameObject.SetActive(true);
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
    }
}
