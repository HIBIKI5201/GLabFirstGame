using UnityEngine;

/// <summary>
/// アイテムを制御する基底クラス
/// アイテムの共通機能を実装する
/// </summary>
[RequireComponent(typeof(Collider2D))]
public abstract class ItemBase : MonoBehaviour
{
    /// <summary>アイテムをどう投げるか/summary>
    [Tooltip("Straight まっすぐ、Parabola 放物的")]
    [SerializeField] ThrowType _throwType = ThrowType.Straight;
    /// <summary>アイテム発動中の効果範囲/summary>
    [Tooltip("アイテム発動中の効果範囲")]
    [SerializeField] float _effectRange = 1f;
    [Tooltip("アイテムの効果時間")]
    [SerializeField] static float _activateTime = 5;
    public float ActivatetTime => _activateTime;
    public float EffectRange => _effectRange;
    public ThrowType Throw => _throwType;
    bool _isThrowing;
    public bool IsThrowing => _isThrowing;
    public bool Landing { get; set; }
    public GameObject Player { get; private set; }
    PauseManager _pauseManager;
    /// <summary>
    /// アイテムが発動する効果を実装する
    /// </summary>
    public abstract void Activate();
    private void Awake()
    {
        _pauseManager = FindAnyObjectByType<PauseManager>();
        _pauseManager.OnPauseResume += PauseAction;
    }
    private void OnDisable()
    {
        _pauseManager.OnPauseResume -= PauseAction;
    }
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
    Vector2 _keepVelocity;
    private void Pause()
    {
        if (gameObject.TryGetComponent(out Rigidbody2D rb))
        {
            _keepVelocity = rb.velocity;
            rb.Sleep();
        }
    }
    private void Resume()
    {
        if (gameObject.TryGetComponent(out Rigidbody2D rb))
        {
            rb.WakeUp();
            rb.velocity = _keepVelocity;
        }
    }
    private void Update()
    {
        Activate();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isThrowing)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                Player = collision.gameObject;
                AudioManager.Instance.PlaySE("itemGet");
                // 見えない所に移動する
                this.transform.position = Camera.main.transform.position;
                // コライダーを無効にする
                GetComponent<Collider2D>().enabled = false;
                // プレイヤーにアイテムを渡す
                collision.gameObject.GetComponent<PlayerController>().GetItem(this);
            }
        }
    }
    /// <summary>
    /// アイテムを投げた時に呼ぶ
    /// </summary>
    public void Throwing() => _isThrowing = true;
    public enum ThrowType
    {
        /// <summary>真っ直ぐ投げる</summary>
        Straight,
        /// <summary>放物線を描くように投げる</summary>
        Parabola
    }
}
