using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Goal : MonoBehaviour
{
    [SerializeField] GameManager _gameManager;
    [Header("�t�F�[�h�A�E�g����C���[�W"), SerializeField] GameObject _fadeImage;
    [SerializeField] SceneLoader _sceneLoader;
    PlayerController _playerController;
    [SerializeField] GameObject _clearText;
    [SerializeField] Text _timerTxt;
    [SerializeField] Text _clearTime;
    public Animator _animator;
    [SerializeField, Header("���݂̃X�e�[�W")] public int _nowStage;
    Rigidbody2D _rb;
    [SerializeField, Header("�S�[�����������")] float _warkTime;
    [SerializeField, Header("�����A�j���[�V�����̖��O")] string _anime;
    [FormerlySerializedAs("_goalPerformance")] [SerializeField] Stage1GoalSequence goalSequence;
    [FormerlySerializedAs("_goal3Performance")] [SerializeField] Stage3GoalSequence goal3Sequence;
    GameProgressManager _gameProgressManager;
    Timer _timer;
    bool _walk;

    private void Awake()
    {
        _fadeImage.SetActive(false);
    }

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _gameProgressManager = FindAnyObjectByType<GameProgressManager>();
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
            if (_gameManager.StateType == GameStateType.StageClear)
            {
                _rb.Sleep();
                if(goalSequence != null) goalSequence.StartSequence(_warkTime);
                if(goal3Sequence != null) goal3Sequence.StartCoroutine(goal3Sequence.DoPerformance(_warkTime));
                _playerController.StopAction(_warkTime + 120f);
                StartCoroutine(Walk(_warkTime));
                if (goal3Sequence == null) Invoke(nameof(Clear), _warkTime);
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
        _clearTime.text = $"クリアタイム {min.ToString("00")}:{sec.ToString("00")}";
        _gameProgressManager.StageClear(_nowStage);
    }

    IEnumerator Walk(float time)
    {
        _walk = true;
        if (_anime != null)
        {
            _animator.SetBool("isClear",true);
            _animator.Play(_anime);
            Debug.Log("�S�[���A�j���[�V����");
            Debug.Log(_animator.gameObject.name);
        }
        yield return new WaitForSeconds(time);
        _walk = false;
        _animator.SetBool("isClear",false);
        if (goal3Sequence == null) StartCoroutine(Image(2f));
    }

    IEnumerator Image(float time)
    {
        yield return new WaitForSeconds(time);
        _fadeImage.SetActive(true);
        FadeOut fadeOut = _fadeImage.GetComponent<FadeOut>();
        if(goal3Sequence == null) StartCoroutine(LoadScene(fadeOut._fadeTime));
    }

    public IEnumerator LoadScene(float time)
    {
        yield return new WaitForSeconds(time);
        _sceneLoader.FadeAndLoadScene();
    }
}
