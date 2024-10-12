using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField,Header("RedImage‚ð“ü‚ê‚é")] GameObject _redObject;
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
        IsGameOver();
    }

    private void IsGameOver()
    {
        if (_gameManager.State == GameManager.GameState.GameOver)
        {
            _redObject.SetActive(true);
            if (_checkpoint._checkpoint != null) position = _checkpoint._checkpoint;
        }
    }
}
