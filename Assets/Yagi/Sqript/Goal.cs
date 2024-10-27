using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Goal : MonoBehaviour
{
    [SerializeField] GameManager _gameManager;
    [Header("フェードアウトするイメージ"), SerializeField] GameObject _fadeImage;
    [SerializeField] SceneLoader _sceneLoader;
    PlayerController _playerController;
    [SerializeField] GameObject _clearText;
    [SerializeField] Text _timerTxt;
    [SerializeField] Text _clearTime;
    Animator _animator;
    [SerializeField, Header("現在のステージ")] public int _nowStage;
    Rigidbody2D _rb;
    [SerializeField, Header("ゴール後歩く時間")] float _warkTime;
    [SerializeField, Header("歩くアニメーションの名前")] string _anime;
    [SerializeField] Stage1GoalPerformance _goalPerformance;
    [SerializeField] Stage3GoalPerformance _goal3Performance;
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
        _clearText.SetActive(false);
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
                if(_goalPerformance != null) _goalPerformance.Perfomance(_warkTime);
                if(_goal3Performance != null) _goal3Performance.StartCoroutine(_goal3Performance.DoPerformance(_warkTime));
                _playerController.StopAction(_warkTime + 10f);
                StartCoroutine(Walk(_warkTime));
                if (_goal3Performance == null) Invoke(nameof(Clear), _warkTime);
                _timer.enabled = false;
            }
        }
    }

    public void Clear()
    {
        AudioManager.Instance.PlaySE("stageclear");
        _clearText.SetActive(true);
        _timerTxt.enabled = false;
        _clearTime.enabled = true;
        int min = Mathf.FloorToInt(_timer._currentTime / 60);
        int sec = Mathf.FloorToInt(_timer._currentTime % 60);
        _clearTime.text = $"クリア時間 {min.ToString("00")}:{sec.ToString("00")}";
        _isClear.StageClear(_nowStage);
    }

    IEnumerator Walk(float time)
    {
        _walk = true;
        if (_anime != null)
        {
            _animator.SetBool("isClear",true);
            _animator.Play(_anime);
            Debug.Log("ゴールアニメーション");
            Debug.Log(_animator.gameObject.name);
        }
        yield return new WaitForSeconds(time);
        _walk = false;
        if (_goal3Performance == null) StartCoroutine(Image(2f));
    }

    IEnumerator Image(float time)
    {
        yield return new WaitForSeconds(time);
        _fadeImage.SetActive(true);
        FadeOut fadeOut = _fadeImage.GetComponent<FadeOut>();
        if(_goal3Performance == null) StartCoroutine(LoadScene(fadeOut._fadeTime));
    }

    public IEnumerator LoadScene(float time)
    {
        yield return new WaitForSeconds(time);
        _sceneLoader.FadeAndLoadScene();
    }
}
