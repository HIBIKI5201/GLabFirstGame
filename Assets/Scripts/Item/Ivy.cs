using System.Collections;
using UnityEngine;

/// <summary>
/// アイテム：ツタ
/// </summary>
public class Ivy : ItemBase
{
    [Header("地面に接触したら、このレイヤーになる")]
    [SerializeField, Layer]
    private int _onTouchGroundLayer;

    [Header("奈落の Y ポジション")]
    [SerializeField]
    private float _ivyDisappearHeight = -10f;

    [Header("敵に効果時間")]
    [SerializeField]
    private float _enemyEffectTime = 1f;

    [Header("咲いた状態")]
    [SerializeField, Required]
    private BoxCollider2D _expandCollider;

    [SerializeField, Required]
    private SpriteRenderer _expandSprite;

    private BoxCollider2D _boxCollider2D;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody2D;
    private float _releasedTime;
    private Vector3 _cacheScale;
    private bool _putIvySoundPlayed;

    private void Awake()
    {
        _cacheScale = transform.localScale;
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _expandCollider.enabled = false;
        _expandSprite.enabled = false;
    }

    private bool IsBottom()
    {
        return transform.position.y < _ivyDisappearHeight;
    }

    private void Start()
    {
        StartCoroutine(BottomCheck());
    }

    private IEnumerator BottomCheck()
    {
        yield return new WaitUntil(IsBottom); // 条件がtrueになるまで待つ
        Debug.Log("奈落に落ちた");
        Destroy(gameObject);
    }

    protected override void Activate()
    {
        if (!IsThrowing)
        {
            return;
        }

        _releasedTime += Time.deltaTime;

        var hit = Physics2D.OverlapBoxAll(transform.position, _boxCollider2D.size, 0);
        foreach (var obj in hit)
        {
            if (obj.gameObject.CompareTag("Ground"))
            {
                gameObject.layer = _onTouchGroundLayer;
                Landing = true;
                _boxCollider2D.enabled = true;
                if (!_putIvySoundPlayed)
                {
                    AudioManager.Instance.PlaySE("putIvy");
                    _putIvySoundPlayed = true;
                }

                _boxCollider2D.offset = _expandCollider.offset;
                _boxCollider2D.size = _expandCollider.size;
                _spriteRenderer.sprite = _expandSprite.sprite;
                transform.localScale = _expandSprite.transform.localScale.x * _cacheScale;
            }
        }

        if (_releasedTime > 0.5f && TryGetComponent(out _rigidbody2D) && Mathf.Abs(_rigidbody2D.linearVelocity.y) <= 0)
        {
            _rigidbody2D.linearVelocity = Vector2.zero;
            _rigidbody2D.angularVelocity = 0;
            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            _boxCollider2D.isTrigger = true;
        }

        hit = Physics2D.OverlapCircleAll(transform.position, EffectRange);
        foreach (var obj in hit)
        {
            if (obj.TryGetComponent<Enemy>(out var enemy) && !enemy.CanAvoidIvy)
            {
                enemy.ReactionStone(_enemyEffectTime);

                if (enemy.State != EnemyStateType.Faint)
                {
                    // まだ敵が気絶状態ではなければ、SEを再生する
                    AudioManager.Instance.PlaySE("damage_enemy");
                }
                Destroy(gameObject, 0f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // FIXME: This is not invoked since the Trigger is deactivated from PlayerController.cs
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            AudioManager.Instance.PlaySE("enterIvy");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        Gizmos.DrawWireSphere(transform.position, EffectRange);
    }

    protected override void PlaySE()
    {
        AudioManager.Instance.PlaySE("getIvy");
    }
}
