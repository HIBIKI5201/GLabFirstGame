using UnityEngine;

/// <summary>
/// ゲームオーバーになったときに赤いオーバーレイを表示させるクラス
/// </summary>
public class GameOverEffect : MonoBehaviour
{
    [SerializeField] private DeathOverlayController _deathOverlayController; // 死亡時に表示される赤いオーバーレイを管理するクラス
    private GameManager _gameManager;
    private bool _isActive = false; // 一度だけ処理を行うためのフラグ

    private void Start()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
    }

    private void Update()
    {
        // ゲームオーバー状態かチェックする
        if (!_isActive && _gameManager.StateType == GameStateType.GameOver)
        {
            ApplyGameOverEffects();
            _isActive = true;
        }
    }

    /// <summary>
    /// ゲームオーバーに演出を行う
    /// </summary>
    private void ApplyGameOverEffects()
    {
        _deathOverlayController.OnActive(); // オーバーレイを表示
        AudioManager.Instance.PlaySE("dead");
    }
}
