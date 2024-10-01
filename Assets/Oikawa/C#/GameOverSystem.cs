using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverSystem : MonoBehaviour
{
    GameManager _gameManager;
    PlayerController _playerController;
    Timer _timer;
    void Start()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
        _playerController = FindAnyObjectByType<PlayerController>();
        _timer = FindAnyObjectByType<Timer>();
    }

    // Update is called once per frame
    void Update()
    {
        FieldInfo field = typeof(PlayerController)
            .GetField("_currentHp", BindingFlags.NonPublic | BindingFlags.Instance);
        int currentHP = (int)field.GetValue(_playerController);

        field = typeof(Timer)
            .GetField("_currentTime", BindingFlags.NonPublic | BindingFlags.Instance);
        float currentTime = (float)field.GetValue(_timer);

        if (_gameManager.State == GameManager.GameState.GameOver
            ||currentHP <= 0
            ||currentTime <= 0)
        {
            Debug.Log("ゲームオーバーによるやり直し");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
