using UnityEngine;

/// <summary>
/// アイテム：肉
/// </summary>
public class Meat : ItemBase
{
    [SerializeField] private Vector2 _size;
    [SerializeField] private float _angle;
    private bool _effected;
    private Rigidbody2D _rb;

    protected override void Activate()
    {
        // 投げていない時であれば、以降の処理は行わない
        if (!IsThrowing) return;

        if (!Landing)
        {
            CheckForLanding();
        }
        else
        {
            HandleLanded();
        }
    }

    /// <summary>
    /// 地面に衝突したかの判定
    /// </summary>
    private void CheckForLanding()
    {
        var hit = Physics2D.OverlapBoxAll(transform.position, _size, _angle);
        _rb = GetComponent<Rigidbody2D>();
        
        foreach (var obj in hit)
        {
            if (obj.gameObject.CompareTag("Ground"))
            {
                Landing = true;
                GetComponent<Collider2D>().enabled = true;
                AudioManager.Instance.PlaySE("meat");
            }
        }
    }

    /// <summary>
    /// 地面に落ちたあとの処理
    /// </summary>
    private void HandleLanded()
    {
        if (!_effected)
        {
            var hit = Physics2D.OverlapCircleAll(transform.position, EffectRange);
            
            
            foreach (var obj in hit)
            {
                if (obj.gameObject.CompareTag("Enemy"))
                {
                    // 敵のリアクションを呼び出す
                    obj.gameObject.GetComponent<Enemy>().ReactionMeat(transform.position, ActiveTime);
                }
            }

            _effected = true;
            Destroy(gameObject, ActiveTime);
        }

        if (_effected && _rb.linearVelocity.y == 0)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0;
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
