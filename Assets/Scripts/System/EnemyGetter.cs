using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ステージ内すべての敵の参照を取得する
/// </summary>
public class EnemyGetter : MonoBehaviour
{
    /// <summary>
    /// 敵の参照
    /// </summary>
    public List<Enemy> Enemies => _enemies; // ラムダ式 プロパティ
    private List<Enemy> _enemies = new List<Enemy>();

    private void Start()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy"); // シーン内のエネミーの参照を取得
        foreach(GameObject enemy in enemies)
        {
            _enemies.Add(enemy.GetComponent<Enemy>());　// 取得したゲームオブジェクトからEnemyクラスを取得してリストに追加する
        }
    }

    /// <summary>
    /// リストからエネミーの参照を取り除く
    /// 敵が死亡したタイミングで呼び出す
    /// </summary>
    public void RemoveEnemy(Enemy enemy)
    {
        if (_enemies.Contains(enemy))
        {
            _enemies.Remove(enemy); // リスト内に参照があれば、リストから消去する
        }
    }
}
