using UnityEngine;

/// <summary>
/// アイテム：石
/// </summary>
public class Rock : ItemBase
{
    private string _seNameGet = "getRock"; // アイテム取得時に再生するSEの名称
    protected override void Activate()
    {
        // 投げていない時であれば、以降の処理は行わない
        if (!IsThrowing) return;
        
        var hit = Physics2D.OverlapCircleAll(transform.position, EffectRange);
        foreach (var obj in hit)
        {
            if (obj.CompareTag("Enemy"))
            {
                if (obj.TryGetComponent<Enemy>(out var enemy))
                {
                    enemy.ReactionStone(ActiveTime);
                    if (enemy.State != EnemyStateType.Faint)
                    {
                        // まだ敵が気絶状態ではなければ、SEを再生する
                        AudioManager.Instance.PlaySE("damage_enemy");
                    }
                }
                
                Destroy(gameObject, 0.3f);
            }
        }
    }
    protected override void PlaySE()
    {
        AudioManager.Instance.PlaySE(_seNameGet);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        Gizmos.DrawWireSphere(transform.position, EffectRange);
    }
}
