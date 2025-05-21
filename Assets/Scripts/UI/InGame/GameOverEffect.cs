using UniRx;
using UnityEngine;

/// <summary>
/// ゲームオーバーになったときに赤いオーバーレイを表示させるクラス
/// </summary>
public class GameOverEffect : MonoBehaviour
{
    [SerializeField] private DeathOverlayController _deathOverlayController; // 死亡時に表示される赤いオーバーレイを管理するクラス
    private CompositeDisposable _disposable = new CompositeDisposable();

    private void Start()
    {
        GameManager.Instance.CurrentStateProp
            .Where(state => state == GameStateType.GameOver) // ゲームオーバーになった時に
            .Subscribe(_ => ApplyGameOverEffects()) // ゲームオーバー演出を行う
            .AddTo(_disposable);
    }

    /// <summary>
    /// ゲームオーバーに演出を行う
    /// </summary>
    private void ApplyGameOverEffects()
    {
        _deathOverlayController.OnActive(); // オーバーレイを表示
        AudioManager.Instance.PlaySE("dead");
    }

    private void OnDestroy()
    {
        _disposable?.Dispose();
    }
}
