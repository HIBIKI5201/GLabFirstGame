using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour
{
    PlayerController _player;
    GameManager gameManager;
    Timer _timer;
    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        gameManager.State = GameManager.GameState.Playing;
        if (GameOver.position != Vector2.zero) transform.position = GameOver.position;
        _timer = FindAnyObjectByType<Timer>();
        _player = GetComponent<PlayerController>();
        _player.StopAction(2f);
        StartCoroutine(TimerStart(2.5f));
    }

    IEnumerator TimerStart(float time)
    {
        _timer.enabled = false;
        yield return new WaitForSeconds(time);
        _timer.enabled = true;
    }
}
