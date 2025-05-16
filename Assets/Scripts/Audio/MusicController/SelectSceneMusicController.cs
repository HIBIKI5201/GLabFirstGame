using UnityEngine;

/// <summary>
/// ステージ選択シーンの音楽を再生するクラス
/// </summary>
public class SelectSceneMusicController : MonoBehaviour
{
    private void Start()
    {
        AudioManager.Instance.PlayBGM("selectedStage");
    }
}
