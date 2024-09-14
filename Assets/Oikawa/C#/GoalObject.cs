using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalObject : MonoBehaviour
{
    [SerializeField] GameManager _gameManager;
    void Start()
    {
        if(_gameManager == null)
            _gameManager = GameObject.FindAnyObjectByType<GameManager>();
    }
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            _gameManager.State = GameManager.GameState.StageClear;
        }
    }
}
