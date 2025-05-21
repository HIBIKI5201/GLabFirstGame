using UnityEngine;

/// <summary>
/// プレイヤーの進行状況に基づいて、表示するエンディングを制御するクラス
/// </summary>
public class EndingController : MonoBehaviour
{
    [SerializeField] GameObject _badEnd;
    [SerializeField] GameObject _happyEnd;
    
    private void Start()
    {
        // 隠しエンディングが解放されているか確認
        bool showSecretEnding = GameProgressManager.IsSecretModeUnlocked;
        
        // 解放されていなかったらBadEndのオブジェクトを表示, 解放されていたらHappyEndのオブジェクトを表示
        _badEnd.SetActive(!showSecretEnding);
        _happyEnd.SetActive(showSecretEnding);
    }
}
