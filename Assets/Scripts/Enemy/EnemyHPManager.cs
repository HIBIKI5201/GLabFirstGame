using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 敵のHPを管理するクラス
/// </summary>
public class EnemyHPManager
{
    private int _currentHp; // 現在のHP
    private bool _canDamage; // ダメージを受けるか
    private SpriteRenderer[] _spriteRenderers;
    private Enemy _enemy; // エネミー
    private Coroutine _damageCoro = null; // ダメージ演出のためのコルーチン
    
    public EnemyHPManager(int hp, bool canDamage, SpriteRenderer[] spriteRenderers, Enemy enemy)
    {
        _currentHp = hp;
        _canDamage = canDamage;
        _spriteRenderers = spriteRenderers;
        _enemy = enemy;
    }
    
    /// <summary>
    /// HPを減らすメソッド
    /// </summary>
    /// <param name="value">ダメージを受ける場合は負の数を渡す</param>
    public void LifeFluctuation(int value)
    {
        if (!_canDamage && value < 0)
        {
            return;
        }

        _currentHp += value;
        
        if (value < 0)
        {
            DamageEffect(); // ダメージエフェクトを再生する
            Debug.Log($"{value}ダメージを受けた  現在のHP:{_currentHp}");
        }
        
        if (_currentHp <= 0)
        {
            _enemy.DestroyThis(); // HPがゼロになったら自分を破棄する
        }
    }
    
    /// <summary>
    /// ダメージを受けた時のエフェクト
    /// </summary>
    private void DamageEffect()
    {
        if (_damageCoro != null)
        {
            //StopCoroutine(_damageCoro);
        }
           
        // _damageCoro = StartCoroutine(Damage());
    }
    
    /// <summary>
    /// ダメージを受けたときの演出を行うコルーチン
    /// </summary>
    private IEnumerator Damage()
    {
        _canDamage = false;
        float time = Time.time;
        while (Time.time <= time + 0.5f)
        {
            _spriteRenderers.ToList().ForEach(x => x.color = new Color(0.5f, 0.5f, 0.5f));
            yield return new WaitForSeconds(0.1f);
            _spriteRenderers.ToList().ForEach(x => x.color = Color.white);
            yield return new WaitForSeconds(0.1f);
        }

        _canDamage = true;
    }
}
