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
        FieldInfo field = typeof(PlayerController)
            .GetField("_currentHp", BindingFlags.NonPublic | BindingFlags.Instance);
        int currentHP = (int)field.GetValue(_playerController);

        field = typeof(Timer)
            .GetField("_currentTime", BindingFlags.NonPublic | BindingFlags.Instance);
        float currentTime = (float)field.GetValue(_timer);

        if (
            (_gameManager.State == GameManager.GameState.GameOver
            || currentHP <= 0
            || currentTime <= 0)&&!_called)
        {
            _called = true;
            Invoke("Load", 2);
        }
    }
    public void Load()
    {
        Debug.Log("�Q�[���I�[�o�[�ɂ���蒼��");
        _sceneLoader.FadeAndLoadScene();
    }
}
