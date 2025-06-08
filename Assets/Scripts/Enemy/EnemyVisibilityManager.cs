using UnityEngine;

/// <summary>
/// カメラの視界に入った敵のみをアクティブにして処理を最適化するクラス
/// </summary>
[RequireComponent(typeof (BoxCollider2D))]
public class EnemyVisibilityManager : MonoBehaviour
{
    // ステージに配置されているすべての敵オブジェクト
    [SerializeField] private Enemy[] _enemies;

    private void Start()
    {
        _enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        // 最初はすべての敵を無効にする
        for (int i = 0; i < _enemies.Length; i++)
        {
            _enemies[i].SetEnemyEnabledState(false);
        }
    }

    /// <summary>
    /// 敵がトリガー領域に入ったときに呼ばれ、敵を有効化
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision)
        {
            SetEnemyActive(collision.GetComponent<Enemy>(), true);
        }
    }

    /// <summary>
    /// 敵がトリガー領域に入ったときに呼ばれ、敵を無効化
    /// </summary>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision)
        {
            SetEnemyActive(collision.GetComponent<Enemy>(), false);
        }
    }
    
    /// <summary>
    /// 指定された敵の有効/無効状態を切り替える
    /// </summary>
    private void SetEnemyActive(Enemy enemy, bool shouldBeEnabled)
    {
        if (enemy)
        {
            // 有効/無効状態を切り替えるべき敵を検索
            for (int i = 0; i < _enemies.Length; i++)
            {
                if (_enemies[i] == null)
                {
                    continue;
                }

                if (enemy == _enemies[i])
                {
                    // 一致したインデックスのEnemyコンポーネントをアクティブにする
                    enemy.SetEnemyEnabledState(shouldBeEnabled);
                }
            }
        }
    }
}
