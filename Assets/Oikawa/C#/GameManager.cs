using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameState _state;
    public GameState State { get => _state; set { Debug.Log($"{_state}����{value}�Ɉڍs"); _state = value; } }
    public enum GameState
    {
        BeforeStart,
        Playing,
        StageClear,
        GameOver
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
