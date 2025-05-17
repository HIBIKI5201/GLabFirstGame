using UnityEngine;

/// <summary>
/// アイテム：空き瓶
/// </summary>
public class Bottle : ItemBase
{
    [SerializeField] private Vector2 _size;
    [SerializeField] private float _angle;
    private bool _effected;
    private Rigidbody2D _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    protected override void Activate()
    {
        // 投げていない時であれば、以降の処理は行わない
        if (!IsThrowing) return;

        if (!Landing)
        {
            CheckForLanding(); // 地面の判定をチェックする
        }
        else
        {
            HandleLandedBottle(); // 地面に落ちたあとの処理
        }
    }

    /// <summary>
    /// 地面に衝突したかの判定
    /// </summary>
    private void CheckForLanding()
    {
        var hit = Physics2D.OverlapBoxAll(transform.position, _size, _angle);
        foreach (var obj in hit)
        {
            if (obj.gameObject.CompareTag("Ground"))
            {
                Landing = true;
                GetComponent<Collider2D>().enabled = true;
                AudioManager.Instance.PlaySE("crack"); // 地面に衝突した時のSEを再生する
            }
        }
    }
    
    /// <summary>
    /// 地面に落ちたあとの処理
    /// </summary>
    private void HandleLandedBottle()
    {
        if (!_effected)
        {
            var hit = Physics2D.OverlapCircleAll(transform.position, EffectRange);

            foreach (var obj in hit)
            {
                if (obj.CompareTag("Enemy"))
                {
                    // 敵のリアクションを呼び出す
                    obj.gameObject.GetComponent<Enemy>().ReactionBottle(transform.position, ActiveTime);
                }
            }

            _effected = true;
            Destroy(gameObject, ActiveTime);
        }

        if (_effected && _rb.velocity.y == 0)
        {
            _rb.velocity = Vector2.zero;
            _rb.bodyType = RigidbodyType2D.Kinematic;
            gameObject.GetComponent<Collider2D>().enabled = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        Gizmos.DrawWireSphere(transform.position, EffectRange);
        Gizmos.DrawWireCube(transform.position, _size);
    }
}
