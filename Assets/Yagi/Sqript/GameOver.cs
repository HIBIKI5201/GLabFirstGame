using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField, Header("RedImage‚ð“ü‚ê‚é")] GameObject _redObject;
    GameManager _gameManager;

    void Start()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
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
        }
    }
}
