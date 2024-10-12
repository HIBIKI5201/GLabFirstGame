using UnityEngine;


public class Checkpoint : MonoBehaviour
{
    [SerializeField] GameManager _gameManager;

    [SerializeField] GameObject _player;

    [Header("•ÏX‚³‚ê‚é‰æ‘œ"), SerializeField] Sprite _changeSprite;

    SpriteRenderer _spriteRenderer;

    CapsuleCollider2D _capsuleCollider;

    public Vector2 _checkpoint { get; set; }

    private void Start()
    {
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _checkpoint = _player.transform.position;
            _capsuleCollider.enabled = false;
            if (_changeSprite) _spriteRenderer.sprite = _changeSprite;
        }
    }
}
