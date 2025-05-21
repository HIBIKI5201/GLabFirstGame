using UnityEngine;

/// <summary>
/// ゴールオブジェクトを管理するクラス
/// </summary>
public class GoalObject : MonoBehaviour
{
    [SerializeField] private GameObject _rightEnd; // ゴール付近の配置しているステージの右端の判定
    private bool _isFirst; // 初めて通過したか

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            if (!_isFirst)
            {
                AudioManager.Instance.PlaySE("goal");
            }
            
            GameManager.Instance.ChangeState(GameStateType.StageClear); // ゲームの状態を変更
            _isFirst = true;
            Destroy(_rightEnd); // 右端のコライダーを削除して、演出中先に進めるようにする
        }
    }
}
