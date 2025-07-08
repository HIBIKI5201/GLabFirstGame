using UnityEngine;
[RequireComponent (typeof(AudioSource),typeof(SpriteRenderer))]
public class HideGrass : MonoBehaviour
{
    private SpriteRenderer _renderer;
    private AudioSource _audioSource;
    [SerializeField,Range(0,1)] private float _alpha = 0.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _renderer.color = new Color(1,1,1,_alpha);
            _audioSource.Play();

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _renderer.color = Color.white;
            _audioSource.Play();
        }
    }
}
