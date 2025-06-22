using System.Collections;
using UnityEngine;

/// <summary>
/// アイテム：ツタ
/// </summary>
public class Ivy : ItemBase
{
    [SerializeField] private float bottom = -10f;
    [SerializeField] private float effectTime = 1f; // 効果時間

    private BoxCollider2D _boxCollider2D;
    private Rigidbody2D _rigidbody2D;
    private float _releasedTime;

    private void Awake()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private bool IsBottom()
    {
        return transform.position.y < bottom;
    }

    private void Start()
    {
        StartCoroutine(BottomCheck());
    }

    private IEnumerator BottomCheck()
    {
        Debug.Log("奈落にはまだ落ちていない");
        yield return new WaitUntil(IsBottom); // 条件がtrueになるまで待つ
        Debug.Log("奈落に落ちた");
        Destroy(gameObject);
    }

    protected override void Activate()
    {

        // 投げていない時であれば、以降の処理は行わない
        if (!IsThrowing) return;
        _releasedTime += Time.deltaTime;

        var hit = Physics2D.OverlapBoxAll(transform.position, _boxCollider2D.size, 0);
        foreach (var obj in hit)
        {
            if (obj.gameObject.CompareTag("Ground"))
            {
                Landing = true;
                _boxCollider2D.enabled = true;
                AudioManager.Instance.PlaySE("crack"); // 地面に衝突した時のSEを再生する
            }
        }

        if (_releasedTime > 0.5f && TryGetComponent(out _rigidbody2D) && _rigidbody2D.linearVelocity.y <= 0)
        {
            _rigidbody2D.linearVelocity = Vector2.zero;
            _rigidbody2D.angularVelocity = 0;
            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            _boxCollider2D.enabled = false;
        }


        hit = Physics2D.OverlapCircleAll(transform.position, EffectRange);
        foreach (var obj in hit)
        {
            if (obj.CompareTag("Enemy"))
            {
                if (obj.TryGetComponent<Enemy>(out var enemy))
                {
                    enemy.ReactionStone(effectTime);

                    if (enemy.State != EnemyStateType.Faint)
                    {
                        // まだ敵が気絶状態ではなければ、SEを再生する
                        AudioManager.Instance.PlaySE("damage_enemy");
                    }
                }

                Destroy(gameObject, 0f);
            }
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        Gizmos.DrawWireSphere(transform.position, EffectRange);
    }

    protected override void PlaySE()
    {
        Debug.Log($"Play Ivy Pickup Sound.");
    }
}
