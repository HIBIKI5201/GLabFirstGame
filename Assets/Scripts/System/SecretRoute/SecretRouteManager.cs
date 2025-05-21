using UniRx;
using UnityEngine;

/// <summary>
/// 隠しクリアを管理するクラス
/// </summary>
public class SecretRouteManager : MonoBehaviour
{
    [SerializeField] private SceneLoader _load;
    private Timer _time;
    private bool _hasPassedTriggerPoint; // 特定の地点を通過したか

    private void Start()
    {
        _time = FindAnyObjectByType<Timer>();

        _time.CurrentTimeProp
            .Where(time => time <= 0)
            .Subscribe(_ => IsAnother())
            .AddTo(this);
    }
    
    /// <summary>
    /// 制限時間が残りゼロになったとき、隠しルートのフラグを立てる
    /// </summary>
    private void IsAnother()
    {
        if (_hasPassedTriggerPoint && GameProgressManager.IsGameCompleted) // 一定地点を通過 & ゲームを1周クリアした記録があった場合
        {
            GameProgressManager.IsSecretModeUnlocked = true; // 隠しルートをアンロック
            GameProgressManager.HighestClearedStage = 1; // ステージ1をクリアとする
            PlayerPrefs.SetInt("nowStage", GameProgressManager.HighestClearedStage); // セーブ
            
            // チェックポイントをリセット
            Checkpoint checkpoint = FindAnyObjectByType<Checkpoint>();
            checkpoint.ResetPoint();
            
            _load.FadeAndLoadScene();
        }
    }

    /// <summary>
    /// 隠しルート解放となる特定の地点を通過したか
    /// </summary>
    public void PassedTriggerPoint() => _hasPassedTriggerPoint = true;
}
