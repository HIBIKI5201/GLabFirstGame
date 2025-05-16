using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [FormerlySerializedAs("_state")] public GameStateType stateType;
    public static GameManager instance;
    public GameStateType StateType 
    { 
        get => stateType; 
        set 
        {
            if (stateType == value)
                return;
            //Debug.Log($"{stateType}����{value}�Ɉڍs"); 
            stateType = value;
        } 
    }
    
    [Space,Tooltip("FPS�l���w�肷��")]
    [SerializeField] bool _settingFPS;
    [Tooltip("FPS�l")]
    [SerializeField,Range(10,120)] int _targetFPS;
    
    void Start()
    {
        instance = this;
    }

    void Update()
    {
        if (_settingFPS)
            Application.targetFrameRate = _targetFPS;
        else
            Application.targetFrameRate = -1;
    }
}
