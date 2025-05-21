using UniRx;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("フレームレートの設定")]
    [SerializeField] private bool _settingFPS;
    [SerializeField,Range(10,120)] private int _targetFPS;

    /// <summary>
    /// 現在のゲームの状態を表すリアクティブプロパティ
    /// </summary>
    public ReactiveProperty<GameStateType> CurrentStateProp => _currentStateProp;
    private readonly ReactiveProperty<GameStateType> _currentStateProp = new ReactiveProperty<GameStateType>();
    public GameStateType CurrentState => CurrentStateProp.Value;
    
    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        // FPS設定を行うか。行わない場合、フレームレートの制限を行わず、デバイスが出せる最大のフレームレートで動作する
        Application.targetFrameRate = _settingFPS ? _targetFPS : -1; 
    }
    
    /// <summary>
    /// ステートを変更する
    /// </summary>
    public void ChangeState(GameStateType newState) => CurrentStateProp.Value = newState;
}
