using UnityEngine;

/// <summary>
/// アイテムのベースクラス
/// </summary>
[RequireComponent(typeof(Collider2D))]
public abstract class ItemBase : MonoBehaviour
{
    /// <summary>投げた時の挙動</summary>
    public ThrowType Throw => _throwType;
    [SerializeField] ThrowType _throwType = ThrowType.Straight;
    
    /// <summary>アイテムの効果範囲</summary>
    protected float EffectRange => _effectRange;
    [SerializeField] float _effectRange = 1f;
    
    /// <summary>効果時間</summary>
    protected float ActiveTime => _activeTime;
    private static float _activeTime = 5;
    
    /// <summary>アイテムを投げているか</summary>
    protected bool IsThrowing => _isThrowing;
    private bool _isThrowing;
    
    /// <summary>地面に落ちているか</summary>
    public bool Landing { get; protected set; }
    public GameObject Player { get; private set; }
    
    private PauseManager _pauseManager;
    private Vector2 _keepVelocity;
    
    /// <summary>
    /// アイテムを使用した時の処理
    /// </summary>
    protected abstract void Activate();
    /// <summary>
    /// アイテム取得時のSE再生処理
    /// </summary>
    protected abstract void PlaySE();
    
    private void Awake()
    {
        _pauseManager = FindAnyObjectByType<PauseManager>();
        _pauseManager.OnPauseResume += PauseAction; // ポーズイベントを購読
    }
    
    private void Update()
    {
        Activate();
    }
    
    private void OnDisable()
    {
        _pauseManager.OnPauseResume -= PauseAction; // 購読解除
    }
    
    /// <summary>
    /// アイテム獲得処理
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isThrowing)
        {
            if (collision.CompareTag("Player"))
            {
                Player = collision.gameObject;
                PlaySE();
                transform.position = Camera.main.transform.position;
                GetComponent<Collider2D>().enabled = false;
                collision.gameObject.GetComponent<PlayerController>().GetItem(this);
            }
        }
    }
    
    /// <summary>
    /// アイテムを投げる
    /// </summary>
    public void Throwing() => _isThrowing = true;

    #region ポーズ関連の処理

    /// <summary>
    /// ポーズ/ポーズ解除の状態が変更されたときに呼ばれるメソッド
    /// </summary>
    private void PauseAction(bool isPause)
    {
        if (isPause)
        {
            Pause();
        }
        else
        {
            Resume();
        }
    }
    
    /// <summary>
    /// ポーズ状態になった時の処理
    /// </summary>
    private void Pause()
    {
        if (gameObject.TryGetComponent(out Rigidbody2D rb))
        {
            _keepVelocity = rb.linearVelocity;
            rb.Sleep();
        }
    }
    
    /// <summary>
    /// ポーズ状態が解除されたときの処理
    /// </summary>
    private void Resume()
    {
        if (gameObject.TryGetComponent(out Rigidbody2D rb))
        {
            rb.WakeUp();
            rb.linearVelocity = _keepVelocity;
        }
    }

    #endregion
}
