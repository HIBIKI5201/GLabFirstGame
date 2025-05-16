using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverSystem : MonoBehaviour
{
    GameManager _gameManager;
    SceneLoader _sceneLoader;
    PlayerController _playerController;
    Timer _timer;
    bool _called;
    void Start()
    {
        _called = false;
        _gameManager = FindAnyObjectByType<GameManager>();
        _sceneLoader = FindAnyObjectByType<SceneLoader>();
        _playerController = FindAnyObjectByType<PlayerController>();
        _timer = FindAnyObjectByType<Timer>();
    }

    // Update is called once per frame
    void Update()
    {
        int currentHP = _playerController.CurrentHp;
        float currentTime = _timer._currentTime;

        if (
            (_gameManager.State == GameManager.GameState.GameOver
            || currentHP <= 0
            || currentTime <= 0) && !_called)
        {
            _gameManager.State = GameManager.GameState.GameOver; 
            _called = true;
            //Debug.Log("ゲームオーバーによるやり直し");
        }
    }
}
