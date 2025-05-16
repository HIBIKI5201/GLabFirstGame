using UnityEngine;

/// <summary>
/// タイトルシーンの音楽を再生するクラス
/// </summary>
public class TitleSceneMusicController : MonoBehaviour
{
    private void Start()
    {
        AudioManager.Instance.PlayBGM("title");
    }
}
