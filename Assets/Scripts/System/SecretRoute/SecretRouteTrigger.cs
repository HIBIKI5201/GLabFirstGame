using UnityEngine;

/// <summary>
/// 隠しルートを解放するトリガー
/// </summary>
public class SecretRouteTrigger : MonoBehaviour
{
    [SerializeField] private SecretRouteManager _secretClearCheck;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Colliderにプレイヤーが触れて、ゲームを既に一周クリアしていた場合
        if (collision.CompareTag("Player") && GameProgressManager.IsGameCompleted)
        {
            _secretClearCheck.PassedTriggerPoint();
        }
    }
}
