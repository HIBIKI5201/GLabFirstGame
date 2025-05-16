using UnityEngine;

public class Hole : MonoBehaviour
{
    GameManager _gameManager;
    void Start()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _gameManager.State = GameManager.GameState.GameOver;
        }
        else
        {
            Destroy(collision.gameObject);
        }

    }
}
