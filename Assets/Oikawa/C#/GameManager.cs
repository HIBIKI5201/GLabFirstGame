using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameState _state;
    public static GameManager instance;
    AudioManager _audio;
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
        instance = this;
        _audio = AudioManager.Instance;
        _audio.PlayBGM(SceneManager.GetActiveScene().name.ToLower());
    }

    void Update()
    {
        if (_settingFPS)
            Application.targetFrameRate = _targetFPS;
        else
            Application.targetFrameRate = -1;
    }
}
