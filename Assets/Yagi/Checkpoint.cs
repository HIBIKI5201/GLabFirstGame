using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Checkpoint : MonoBehaviour
{
    [SerializeField] GameManager _gameManager;

    [SerializeField] GameObject _player;

    [Header("ïœçXÇ≥ÇÍÇÈâÊëú"),SerializeField] Sprite _changeSprite;

    SpriteRenderer _spriteRenderer;

    CapsuleCollider2D _capsuleCollider;

    public Vector2 _checkpoint { get; set; }

    private void Start()
    {
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
    }
    private void Update()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _checkpoint = _player.transform.position;
            _capsuleCollider.enabled = false;
            if (_changeSprite)_spriteRenderer.sprite = _changeSprite;
        }
    }
}
