using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 敵のHPを管理するクラス
/// </summary>
public class EnemyDamageHandler : IDisposable
{
    private int _currentHp; // 現在のHP
    private bool _canDamage; // ダメージを受けるか
    private SpriteRenderer[] _spriteRenderers;
    private Enemy _enemy; // エネミー
    private CancellationTokenSource _damageCts = new CancellationTokenSource(); // キャンセル用
    
    public EnemyDamageHandler(int hp, bool canDamage, SpriteRenderer[] spriteRenderers, Enemy enemy)
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
        
        if (_currentHp <= 0)
        {
            _enemy.Die(); // HPがゼロになったら自分を破棄する
            return;
        }
        
        if (value < 0)
        {
            DamageEffect(); // ダメージエフェクトを再生する
            Debug.Log($"{value}ダメージを受けた  現在のHP:{_currentHp}");
        }
    }
    
    /// <summary>
    /// ダメージを受けた時のエフェクト
    /// </summary>
    private void DamageEffect()
    {
        // 実行中のダメージエフェクトがあればキャンセル
        if (_damageCts != null)
        {
            _damageCts.Cancel();
            _damageCts.Dispose();
            _damageCts = new CancellationTokenSource();
        }
           
        Damage(_damageCts.Token).Forget();
    }
    
    /// <summary>
    /// ダメージを受けたときの演出
    /// </summary>
    private async UniTask Damage(CancellationToken cancellationToken)
    {
        _canDamage = false;
        float time = Time.time;

        try
        {
            while (Time.time <= time + 0.5f)
            {
                // 一瞬グレーにする
                _spriteRenderers.ToList().ForEach(x => x.color = new Color(0.5f, 0.5f, 0.5f));
                await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: cancellationToken);
                
                // 白に戻す
                _spriteRenderers.ToList().ForEach(x => x.color = Color.white);
                await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // キャンセルされた場合、色を元に戻す
            foreach (var renderer in _spriteRenderers)
            {
                renderer.color = Color.white;
            }
        }
        finally
        {
            _canDamage = true;
        }
    }

    /// <summary>
    /// リソースの解放
    /// </summary>
    public void Dispose()
    {
        if (_damageCts != null)
        {
            _damageCts.Cancel();
            _damageCts.Dispose();
            _damageCts = null;
        }
    }
}
