using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour
{
    [SerializeField] GameManager _gameManager;
    PlayerController _playerController;
    [SerializeField] Text _clearText;
    [SerializeField] Text _timerTxt;
    Rigidbody2D _rb;
    Timer _timer;
    bool _wark;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _clearText.enabled = false;
        _playerController = GetComponent<PlayerController>();
        _timer = GameObject.FindAnyObjectByType<Timer>();
    }

    void Update()
    {
        if (_wark) this.transform.position = new Vector2(transform.position.x + Time.deltaTime, transform.position.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "goal")
        {
            if (_gameManager.State == GameManager.GameState.StageClear)
            {
                _rb.Sleep();
                _playerController.StopAction(10f);
                StartCoroutine(Work(2f));
                Invoke("Clear", 2f);
                _timer.enabled = false;
            }
        }
    }

    private void Clear()
    {
        _clearText.enabled = true;
        _timerTxt.rectTransform.position = _clearText.rectTransform.position - new Vector3(0, 50, 0);
    }

    IEnumerator Work(float time)
    {
        _wark = true;
        yield return new WaitForSeconds(time);
        _wark = false;
    }
}
