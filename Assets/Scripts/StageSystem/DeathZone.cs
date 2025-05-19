using UnityEngine;

/// <summary>
/// デスゾーン（落下判定）を処理するコンポーネント
/// プレイヤーが触れるとゲームオーバーになり、その他のオブジェクトは破棄される
/// </summary>
public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // プレイヤーが触れたらゲームオーバー状態にする
            GameManager.instance.StateType = GameStateType.GameOver;
        }
        else
        {
            // プレイヤー以外のオブジェクトが触れたら、そのオブジェクトを破棄する
            Destroy(collision.gameObject);
        }

    }
}
