using UnityEngine;

/// <summary>
/// カメラの視界に入った敵のみをアクティブにして処理を最適化するクラス
/// </summary>
[RequireComponent(typeof (BoxCollider2D))]
public class EnemyVisibilityManager : MonoBehaviour
{
    [SerializeField] private Enemy[] _enemies; // ステージに配置されているすべての敵オブジェクト
    private Transform[] _enemiesTra; // 敵のtransformへのcache
    
    private void Start()
    {
        _enemies = FindObjectsOfType<Enemy>();
        _enemiesTra = new Transform[_enemies.Length]; // 敵の数に応じて配列を生成
        
        for (int i = 0; i < _enemies.Length; i++)
        {
            _enemies[i].enabled = false;
            _enemiesTra[i] = _enemies[i].transform;
        }
    }
    
    /// <summary>
    /// 敵がトリガー領域に入ったときに呼ばれ、敵を有効化
    /// </summary>
    private void OnTriggerEnter2D(Collider2D col) => SetEnemyActive(col, true);
    
    /// <summary>
    /// 敵がトリガー領域に入ったときに呼ばれ、敵を無効化
    /// </summary>
    private void OnTriggerExit2D(Collider2D col) => SetEnemyActive(col, false);
    
    /// <summary>
    /// 指定された敵の有効/無効状態を切り替える
    /// </summary>
    private void SetEnemyActive(Collider2D col, bool enabled)
    {
        Transform target = col.transform;
        if (target.CompareTag("Enemy"))
        {
            // 有効/無効状態を切り替えるべき敵を検索
            for (int i = 0; i < _enemies.Length; i++)
            {
                if (_enemies[i] == null || _enemiesTra[i] == null) continue;

                if (target == _enemiesTra[i])
                {
                    _enemies[i].enabled = enabled; // 一致したインデックスのEnemyコンポーネントをアクティブにする
                }
            }
        }
    }
}
