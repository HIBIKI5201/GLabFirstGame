using UnityEngine;

/// <summary>
/// ゲームオーバーになったときに赤いオーバーレイを表示させるクラス
/// </summary>
public class GameOverEffect : MonoBehaviour
{
    [SerializeField, Tooltip("死亡時に表示される赤いオーバーレイ")]
    private GameObject _redObject;

    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
    }

    private void Update()
    {
        // ゲームオーバー状態かチェックする
        if (_gameManager.StateType == GameStateType.GameOver)
        {
            ApplyGameOverEffects();
        }
    }

    /// <summary>
    /// ゲームオーバーに演出を行う
    /// </summary>
    private void ApplyGameOverEffects()
    {
        _redObject.SetActive(true);
        AudioManager.Instance.PlaySE("dead");
    }
}
