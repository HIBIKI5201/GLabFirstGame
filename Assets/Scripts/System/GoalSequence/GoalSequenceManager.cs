using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ステージゴール到達時の演出と処理を制御するクラス
/// </summary>
public class GoalSequenceManager : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private SceneLoader _sceneLoader;
    [SerializeField] private Stage1GoalSequence goalSequence;
    [SerializeField] private Stage3GoalSequence goal3Sequence;
    public Animator _animator;
    
    [Header("UI")]
    [SerializeField] private GameObject _fadeImage; // フェード用の画像オブジェクト
    [SerializeField] private GameObject _clearText;
    [SerializeField] private Text _timerTxt;
    [SerializeField] private Text _clearTime;
    
    [Header("設定")]
    [SerializeField, Header("現在のステージ")] public int _nowStage;
    [SerializeField, Header("歩くアニメーションの時間")] private float _warkTime;
    [SerializeField, Header("歩くアニメーションの名前")] private string _anime;
    
    private PlayerController _playerController;
    private Rigidbody2D _rb;
    private GameProgressManager _gameProgressManager;
    private Timer _timer;
    private bool _isWalk;

    private void Awake()
    {
        _fadeImage.SetActive(false);
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
        _gameProgressManager = FindAnyObjectByType<GameProgressManager>();
        _playerController = FindAnyObjectByType<PlayerController>();
        _timer = FindAnyObjectByType<Timer>();
        _clearText.SetActive(false);
        _clearTime.enabled = false;
        
    }

    private void Update()
    {
        if (_isWalk) 
        {
            // 歩かせる処理
            transform.position = new Vector2(transform.position.x + Time.deltaTime * 2, transform.position.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "goal" && GameManager.Instance.CurrentState  == GameStateType.StageClear)
        {
            _rb.Sleep(); // 物理演算を止める
            
            // コンポーネントがアサインされている方のシーケンスを実行する（ステージ3だけ特殊なゴール演出があるため）
            if (goalSequence != null) goalSequence.StartSequence(_warkTime);
            if (goal3Sequence != null)
            {
                goal3Sequence.StartCoroutine(goal3Sequence.DoPerformance(_warkTime));
            }
            else
            {
                Invoke(nameof(ShowClearUI), _warkTime);
            }
            
            _playerController.StopAction(_warkTime + 120f); // プレイヤーが歩くのを止める
            StartCoroutine(PlayWalkAnimation(_warkTime));
            _timer.enabled = false;
        }
    }

    /// <summary>
    /// クリアUIを表示し、クリアタイムを計算して表示する
    /// </summary>
    public void ShowClearUI()
    {
        AudioManager.Instance.PlaySE("stageclear");
        _timerTxt.enabled = false; // 残り時間を表示しているテキストの更新を止める
        
        _clearText.SetActive(true);
        _clearTime.enabled = true;
        
        int min = Mathf.FloorToInt(_timer.CurrentTime / 60); // 小数点以下切り捨て
        int sec = Mathf.FloorToInt(_timer.CurrentTime % 60);
        _clearTime.text = $"クリアタイム {min:00}:{sec:00}";
        
        _gameProgressManager.StageClear(_nowStage);
    }

    /// <summary>
    /// 歩行アニメーションを実行し、指定時間後にフェードアウトを開始する
    /// </summary>
    private IEnumerator PlayWalkAnimation(float time)
    {
        _isWalk = true;
        
        if (_anime != null)
        {
            _animator.SetBool("isClear",true);
            _animator.Play(_anime); // 指定したアニメーションを再生する
        }
        
        yield return new WaitForSeconds(time);
        
        _isWalk = false;
        _animator.SetBool("isClear",false);

        if (goal3Sequence == null)
        {
            StartCoroutine(StartFadeOut(2f));
        }
    }

    /// <summary>
    /// 指定時間待機後、フェードアウト効果を開始する
    /// </summary>
    private IEnumerator StartFadeOut(float time)
    {
        yield return new WaitForSeconds(time);
        
        _fadeImage.SetActive(true);
        FadeOut fadeOut = _fadeImage.GetComponent<FadeOut>();
        if (goal3Sequence == null)
        {
            StartCoroutine(LoadNextScene(fadeOut._fadeTime));
        }
    }

    /// <summary>
    /// 指定時間待機後、次のシーンを読み込む
    /// </summary>
    public IEnumerator LoadNextScene(float time)
    {
        yield return new WaitForSeconds(time);
        
        _sceneLoader.FadeAndLoadScene();
    }
}
