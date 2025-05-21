using UnityEngine;

/// <summary>
/// ゲームオーバーの判定を行うクラス
/// </summary>
public class GameOverSystem : MonoBehaviour
{
    private PlayerController _playerController;
    private Timer _timer;
    private bool _isCalled;
    
    private void Start()
    {
        _isCalled = false;
        _playerController = FindAnyObjectByType<PlayerController>();
        _timer = FindAnyObjectByType<Timer>();
    }

    private void Update()
    {
        // ゲームオーバー状態、現在のHPが0以下、制限時間が0秒以下
        // かつ、まだ条件を満たしていない時
        if ((GameManager.Instance.CurrentState == GameStateType.GameOver
             || _playerController.CurrentHp <= 0
             || _timer.CurrentTime <= 0) && !_isCalled)
        {
            GameManager.Instance.ChangeState(GameStateType.GameOver); 
            _isCalled = true;
        }
    }
}
