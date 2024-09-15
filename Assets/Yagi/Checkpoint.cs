using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] GameManager _gameManager;

    [SerializeField] GameObject _player;

    SpriteRenderer _spriteRenderer;

    Vector2 _checkpoint;

    private void Update()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_gameManager.State == GameManager.GameState.GameOver) _player.transform.position = _checkpoint;      
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _checkpoint = _player.transform.position;
            _spriteRenderer.enabled = false;
        }
    }
}
