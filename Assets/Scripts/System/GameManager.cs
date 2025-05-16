using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameState _state;
    public static GameManager instance;
    public GameState State 
    { 
        get => _state; 
        set 
        {
            if (_state == value)
                return;
            //Debug.Log($"{_state}����{value}�Ɉڍs"); 
            _state = value;
        } 
    }
    
    [Space,Tooltip("FPS�l���w�肷��")]
    [SerializeField] bool _settingFPS;
    [Tooltip("FPS�l")]
    [SerializeField,Range(10,120)] int _targetFPS;
    public enum GameState
    {
        BeforeStart,
        Playing,
        Pause,
        StageClear,
        GameOver
    }
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
