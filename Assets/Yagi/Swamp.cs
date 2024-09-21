using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swamp : MonoBehaviour
{
    Enemy _enemy;
    PlayerController _player;
    [Tooltip("沼の中にいるのかの判定")] bool _isSwamp = false;
    [SerializeField, Tooltip("ダメージの間隔")] float _damageInterval = 1f;
    [SerializeField, Tooltip("移動速度減少の倍率")] float _speedDown;
    float _timer;
    float _enemyTimer;
    float _defaultSpeed;
    float _defaultMove;
    float _defaultEnemySpeed;
    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        _enemyTimer += Time.deltaTime;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _defaultSpeed = _player._speed;
            _defaultMove = _player._movePower;
            _player._speed *= _speedDown;
            _player._movePower *= _speedDown;
        }

        if (collision.gameObject.tag == "Enemy")
        {
            _enemy = collision.gameObject.GetComponent<Enemy>();
            if (_enemy == null) Debug.Log("エネミーは空");
            
                _defaultEnemySpeed = _enemy._currentSpeed;
                _enemy._currentSpeed *= _speedDown;
            
        }

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (_timer > 1)
            {
                PlayerDamage();
                _timer = 0;
            }
        }

        if (collision.gameObject.tag == "Enemy")
        {
            if (_enemyTimer > 1)
            {
                _enemy.LifeFluctuation(-1);
                _enemyTimer = 0;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _player._speed = _defaultSpeed;
            _player._movePower = _defaultMove;
            _player.Slow(_speedDown, 2f);
        }

        if (collision.gameObject.tag == "Enemy")
        {
            _enemy._currentSpeed = _defaultEnemySpeed;
            _enemy.SlowDownScale(_speedDown, 2);
        }
    }
    private void PlayerDamage()
    {
        _player.FluctuationLife(-1);
    }
}
