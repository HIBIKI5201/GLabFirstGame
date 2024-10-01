using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameState _state;
    public GameState State { get => _state; set { Debug.Log($"{_state}����{value}�Ɉڍs"); _state = value; } }
    
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
    }

    void Update()
    {
        if (_settingFPS)
            Application.targetFrameRate = _targetFPS;
        else
            Application.targetFrameRate = -1;
    }
}
