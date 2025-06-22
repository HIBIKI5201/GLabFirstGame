using UnityEngine;
using UnityEngine.Audio;

public partial class TutaSimizu : MonoBehaviour
{
    //ヒット時に鳴らす音
    public AudioClip hitSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = hitSound;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // 敵に当たったら音を鳴らす
            AudioManager.Instance.PlaySE("");
        }
    }
}
