using UnityEngine;

public class BossBGMChange : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            AudioManager.Instance.PlayBGM("bossFight");
    }
}
