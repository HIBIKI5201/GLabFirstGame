using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalObject : MonoBehaviour
{
    [SerializeField] GameManager _gameManager;
    void Start()
    {
        if(_gameManager == null)
            _gameManager = GameManager.instance;
    }
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            _gameManager.State = GameManager.GameState.StageClear;
        }
    }
}
