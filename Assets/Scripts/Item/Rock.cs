using UnityEngine;

/// <summary>
/// アイテム：石
/// </summary>
public class Rock : ItemBase
{
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
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        Gizmos.DrawWireSphere(transform.position, EffectRange);
    }
}
