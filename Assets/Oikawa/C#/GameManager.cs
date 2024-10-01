using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameState _state;
    public GameState State { get => _state; set { Debug.Log($"{_state}から{value}に移行"); _state = value; } }
    
    [Space,Tooltip("FPS値を指定する")]
    [SerializeField] bool _settingFPS;
    [Tooltip("FPS値")]
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
