using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
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
    
    [SerializeField] bool _settingFPS;
    [SerializeField,Range(10,120)] int _targetFPS;
    
    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        if (_settingFPS)
        {
            Application.targetFrameRate = _targetFPS;
        }
        else
        {
            Application.targetFrameRate = -1;
        }
    }
}
