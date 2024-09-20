using UnityEngine;

/// <summary>
/// アイテムを制御する基底クラス
/// アイテムの共通機能を実装する
/// </summary>
[RequireComponent(typeof(Collider2D))]
public abstract class ItemBase : MonoBehaviour
{
    /// <summary>アイテムを取った時に鳴る効果音</summary>
    [Tooltip("アイテムを取った時に鳴らす効果音")]
    [SerializeField] AudioClip _sound = default;
    /// <summary>アイテムの効果をいつ発揮するか</summary>
    [Tooltip("Get を選ぶと、取った時に効果が発動する。Use を選ぶと、アイテムを使った時に発動する")]
    [SerializeField] ActivateTiming _whenActivated = ActivateTiming.Use;
    /// <summary>アイテムをどう投げるか/summary>
    [Tooltip("Straight まっすぐ、Parabola 放物的")]
    [SerializeField] ThrowType _throwType = ThrowType.Straight;
    /// <summary>アイテム発動中の効果範囲/summary>
    [Tooltip("アイテム発動中の効果範囲")]
    [SerializeField] float _effectRange = 1f;
    [Tooltip("アイテムの効果時間")]
    [SerializeField] float _activateTime;
    public float EffectRange => _effectRange;
    public ThrowType Throw => _throwType;
    bool _isThrowing;
    public bool IsThrowing => _isThrowing;
    public GameObject Player { get; private set; }
    /// <summary>
    /// アイテムが発動する効果を実装する
    /// </summary>
    public abstract void Activate();
    private void Update()
    {
        if(_whenActivated == ActivateTiming.Use)
        Activate();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isThrowing)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                Player = collision.gameObject;
                if (_sound)
                {
                    AudioSource.PlayClipAtPoint(_sound, Camera.main.transform.position);
                }

                // アイテム発動タイミングによって処理を分ける
                if (_whenActivated == ActivateTiming.Get)
                {
                    Activate();
                    Destroy(this.gameObject);
                }
                else if (_whenActivated == ActivateTiming.Use)
                {
                    // 見えない所に移動する
                    this.transform.position = Camera.main.transform.position;
                    // コライダーを無効にする
                    GetComponent<Collider2D>().enabled = false;
                    // プレイヤーにアイテムを渡す
                    collision.gameObject.GetComponent<PlayerController>().GetItem(this);
                }
            }
        }
    }
    /// <summary>
    /// アイテムを投げた時に呼ぶ
    /// </summary>
    public void Throwing() => _isThrowing = true;
    /// <summary>
    /// アイテムをいつアクティベートするか
    /// </summary>
    enum ActivateTiming
    {
        /// <summary>取った時にすぐ使う</summary>
        Get,
        /// <summary>「使う」コマンドで使う</summary>
        Use,
    }
    public enum ThrowType
    {
        /// <summary>真っ直ぐ投げる</summary>
        Straight,
        /// <summary>放物線を描くように投げる</summary>
        Parabola
    }
}
