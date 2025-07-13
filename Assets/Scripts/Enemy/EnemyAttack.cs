using UnityEngine;

/// <summary>
/// 敵の攻撃を管理するクラス
/// </summary>
public class EnemyAttackHandler
{
    private int _attackPoint;
    private float _lastAttackingTime;

    public EnemyAttackHandler(int attackPoint, ref float attackedTimer)
    {
        _attackPoint = attackPoint;
        _lastAttackingTime = attackedTimer;
    }
    
    /// <summary>
    /// 攻撃
    /// </summary>
    public void Attack(PlayerController player)
    {
        // 攻撃のクールタイム中であれば以降の処理は行わない
        if (Time.time <= _lastAttackingTime + 0.1f || player == null)
        {
            return;
        }

        _lastAttackingTime = Time.time;
        player.FluctuationLife(-_attackPoint);
    }
}
