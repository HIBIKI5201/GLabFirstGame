using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Goal : MonoBehaviour
{
    [SerializeField] GameManager _gameManager;
    [Header("�t�F�[�h�A�E�g����C���[�W"), SerializeField] GameObject _fadeImage;
    PlayerController _playerController;
    [SerializeField] Text _clearText;
    [SerializeField] Text _timerTxt;
    [SerializeField] Text _clearTime;
    Animator _animator;
    [SerializeField, Header("���݂̃X�e�[�W")] int _nowStage;
    Rigidbody2D _rb;
    [SerializeField, Header("�S�[�����������")] float _warkTime;
    [SerializeField, Header("�����A�j���[�V�����̖��O")] string _anime;
    IsClear _isClear;
    Timer _timer;
    bool _walk;

    private void Awake()
    {
        _fadeImage.SetActive(false);
    }

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _isClear = FindAnyObjectByType<IsClear>();
        _rb = GetComponent<Rigidbody2D>();
        _clearText.enabled = false;
        _clearTime.enabled = false;
        _playerController = GameObject.FindAnyObjectByType<PlayerController>();
        _timer = GameObject.FindAnyObjectByType<Timer>();
    }

    void Update()
    {
        if (_walk) this.transform.position = new Vector2(transform.position.x + Time.deltaTime * 2, transform.position.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "goal")
        {
            if (_gameManager.State == GameManager.GameState.StageClear)
            {
                _rb.Sleep();
                _playerController.StopAction(_warkTime + 10f);
                StartCoroutine(Walk(_warkTime));
                Invoke(nameof(Clear), _warkTime);
                _timer.enabled = false;
            }
        }
    }

    private void Clear()
    {
        _clearText.enabled = true;
        _timerTxt.enabled = false;
        _clearTime.enabled = true;
        int min = Mathf.FloorToInt(_timer._currentTime / 60);
        int sec = Mathf.FloorToInt(_timer._currentTime % 60);
        _clearText.text = string.Format("{0:00}:{1:00}", min, sec);
        _isClear.StageClear(_nowStage);
    }

    IEnumerator Walk(float time)
    {
        _walk = true;
        if (_anime != null) _animator.Play(_anime);
        yield return new WaitForSeconds(time);
        _walk = false;
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
