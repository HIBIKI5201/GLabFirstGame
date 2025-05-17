using UnityEngine;

/// <summary>
/// 敵の攻撃を管理するクラス
/// </summary>
public class EnemyAttackHandler
{
    private int _attack;
    private float _attackedTimer;
    private PlayerController _player;

    public EnemyAttackHandler(int attack, ref float attackedTimer)
    {
        _attack = attack;
        _attackedTimer = attackedTimer;
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
    
    /// <summary>
    /// 攻撃
    /// </summary>
    public void Attack()
    {
        // 攻撃のクールタイム中であれば以降の処理は行わない
        if (Time.time <= _attackedTimer + 0.1f) return;
        
        _attackedTimer = Time.time;
        _player.FluctuationLife(-_attack);
    }
}
