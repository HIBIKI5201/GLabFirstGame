using UnityEngine;

public class GoalObject : MonoBehaviour
{
    [SerializeField] GameManager _gameManager;
    [SerializeField] GameObject _rightEnd;
    bool _isFirst;
    void Start()
    {
        if(_gameManager == null)
            _gameManager = GameManager.instance;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            _gameManager.StateType = GameStateType.StageClear;
            Destroy(_rightEnd);
            if(!_isFirst) AudioManager.Instance.PlaySE("goal");
            _isFirst = true;
        }
    }
}
