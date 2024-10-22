using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour
{
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
        _timer.enabled = false;
        _gameOver.enabled = false;
        yield return new WaitForSeconds(time);
        _timer.enabled = true;
        _gameOver.enabled = true;
    }
}
