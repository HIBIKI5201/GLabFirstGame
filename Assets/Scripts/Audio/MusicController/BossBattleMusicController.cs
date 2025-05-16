using UnityEngine;

/// <summary>
/// ボス戦のBGMを再生する
/// </summary>
public class BossBattleMusicController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            AudioManager.Instance.PlayBGM("bossFight");
        }
    }
}
