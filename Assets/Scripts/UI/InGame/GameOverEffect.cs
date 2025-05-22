using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

/// <summary>
/// ゲームオーバーになったときに赤いオーバーレイを表示させるクラス
/// </summary>
public class GameOverEffect : MonoBehaviour
{
    [SerializeField] private DeathOverlayController _deathOverlayController; // 死亡時に表示される赤いオーバーレイを管理するクラス

    private void Start()
    {
        GameManager.Instance.CurrentStateProp
            .Where(state => state == GameStateType.GameOver) // ゲームオーバーになった時に
            .Subscribe(_ => ApplyGameOverEffects()) // ゲームオーバー演出を行う
            .AddTo(this);
    }

    /// <summary>
    /// ゲームオーバーに演出を行う
    /// </summary>
    private void ApplyGameOverEffects()
    {
        AudioManager.Instance.PlaySE("dead");
        _deathOverlayController.OnActive().Forget(); // 赤いオーバーパネルを表示
    }
}
