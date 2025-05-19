using DG.Tweening;
using System.Collections;
using UnityEngine;

/// <summary>
/// ステージ開始時の演出
/// </summary>
public class InGameStartSequence : MonoBehaviour
{
    [SerializeField] private CanvasGroup _stageText; // ステージ名UIのCanvasGroup
    [SerializeField] private int _nowStage;
    
    public static bool _isFirstEntry = true; // ステージに初めて入った時のみtrue（リスポーン時はfalse）
    
    private PlayerController _player;
    private GameManager _gameManager;
    private Timer _timer;
    private GameOverSystem _gameOverSystem;
    
    private void Start()
    {
        InitializeReferences();
        SetupStageEntry();
        PositionPlayerAtCheckpoint();
    }

    /// <summary>
    /// 必要なコンポーネントへの参照を初期化
    /// </summary>
    private void InitializeReferences()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
        _gameOverSystem = FindAnyObjectByType<GameOverSystem>();
        _timer = FindAnyObjectByType<Timer>();
        _player = GetComponent<PlayerController>();
    }
    
    /// <summary>
    /// ステージ開始時の初期設定を行う
    /// </summary>
    private void SetupStageEntry()
    {
        _gameManager.StateType = GameStateType.Playing; // ゲームの進行状況を変更
        _player.StopAction(2f); // プレイヤーを少しだけ動けないようにする
        _stageText.gameObject.SetActive(false);
        StartCoroutine(InitializeGameSystems(2.5f)); // 開始演出とタイマー起動のコルーチンを開始
    }

    /// <summary>
    /// チェックポイント情報に基づいてプレイヤーの位置を設定
    /// </summary>
    private void PositionPlayerAtCheckpoint()
    {
        if (CheckPointManager._checkPoint[_nowStage - 1] != Vector2.zero)
        {
            // リスポーン地点が原点以外になっている場合は、その場所にプレイヤーの位置を移動させる
            transform.position = CheckPointManager._checkPoint[_nowStage - 1];
        }
    }

    /// <summary>
    /// ゲームシステムの初期化と開始演出を実行
    /// </summary>
    private IEnumerator InitializeGameSystems(float delayTime)
    {
        if (_isFirstEntry)
        {
            // 初回進入時のみステージ名を表示（リスポーン時は表示しない）
            _stageText.gameObject.SetActive(true);
        }
        
        // タイマーとゲームオーバーシステムを一時的に無効化
        _timer.enabled = false;
        _gameOverSystem.enabled = false;
        
        yield return new WaitForSeconds(delayTime);
        
        _timer.enabled = true;
        _gameOverSystem.enabled = true;
        _stageText.DOFade(0, 0.3f).OnComplete(HideStageNameDisplay);
    }

    /// <summary>
    /// テキストのゲームオブジェクトをを非アクティブにする処理
    /// </summary>
    private void HideStageNameDisplay()
    {
        _stageText.gameObject.SetActive(false);
        _isFirstEntry = false;
    }
}
