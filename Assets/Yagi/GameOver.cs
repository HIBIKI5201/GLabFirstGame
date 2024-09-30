using UnityEngine;

public class GameOver : MonoBehaviour
{
    [Header("RedImage‚ð“ü‚ê‚é"), SerializeField] GameObject _redObject;
    GameManager _gameManager;
    public static Vector2 position;
    Checkpoint _checkpoint;

    
    void Start()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
        _checkpoint = FindAnyObjectByType<Checkpoint>();
    }

    void Update()
    {
        if (_gameManager.State == GameManager.GameState.GameOver)
        {
            _redObject.SetActive(true);
            position = _checkpoint._checkpoint;
        }
    }
}
