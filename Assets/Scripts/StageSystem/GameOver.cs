using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField, Header("RedImage������")] GameObject _redObject;
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
        if (_gameManager.StateType == GameStateType.GameOver)
        {
            _redObject.SetActive(true);
            AudioManager.Instance.PlaySE("dead");
        }
    }
}
