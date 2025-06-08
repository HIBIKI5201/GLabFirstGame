using System.Collections;
using UnityEngine;

/// <summary>
/// アイテム：ツタ
/// </summary>
public class Ivy : ItemBase
{
    [SerializeField] private float bottom = -10f;
    [SerializeField] private float effectTime = 1f; // 効果時間

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
        //if (!IsThrowing) return;

        var hit = Physics2D.OverlapCircleAll(transform.position, EffectRange);

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
        throw new System.NotImplementedException();
    }
}
