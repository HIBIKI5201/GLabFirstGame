using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public GameStateType stateType;
    public GameStateType StateType 
    { 
        get => stateType; 
        set 
        {
            if (stateType == value)
            {
                return; // 既にその状態だったら代入処理は行わない
            }
            
            stateType = value;
        } 
    }
    
    [Header("フレームレートの設定")]
    [SerializeField] private bool _settingFPS;
    [SerializeField,Range(10,120)] private int _targetFPS;

    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        // FPS設定を行うか。行わない場合、フレームレートの制限を行わず、デバイスが出せる最大のフレームレートで動作する
        Application.targetFrameRate = _settingFPS ? _targetFPS : -1; 
    }
}
