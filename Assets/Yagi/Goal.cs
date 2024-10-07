using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Goal : MonoBehaviour
{
    [SerializeField] GameManager _gameManager;
    [Header("フェードアウトするイメージ"),SerializeField] GameObject _fadeImage;
    PlayerController _playerController;
    [SerializeField] Text _clearText;
    [SerializeField] Text _timerTxt;
    [SerializeField,Header("現在のステージ")] int _nowStage;
    Rigidbody2D _rb;
    IsClear _isClear;
    Timer _timer;
    bool _wark;

    private void Awake()
    {
        _fadeImage.SetActive(false);
    }

    private void Start()
    {
        _isClear = FindAnyObjectByType<IsClear>();
        _rb = GetComponent<Rigidbody2D>();
        _clearText.enabled = false;
        _playerController = GameObject.FindAnyObjectByType < PlayerController >();
        _timer = GameObject.FindAnyObjectByType<Timer>();
    }

    void Update()
    {
        if (_wark) this.transform.position = new Vector2(transform.position.x + Time.deltaTime *2, transform.position.y);
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
        _isClear.StageClear(_nowStage);
    }

    IEnumerator Work(float time)
    {
        _wark = true;
        yield return new WaitForSeconds(time);
        _wark = false;
        StartCoroutine(Image(2f));
    }

    IEnumerator Image(float time)
    {
        yield return new WaitForSeconds(time);
        _fadeImage.SetActive(true);
        FadeOut fadeOut = _fadeImage.GetComponent<FadeOut>();
        StartCoroutine(LoadScene(fadeOut._fadeTime));
    }

    IEnumerator LoadScene(float time)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene("SelectStage");
    }
}
