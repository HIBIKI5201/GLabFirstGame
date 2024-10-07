using UnityEngine;
using System.Collections;

public class Swamp : MonoBehaviour
{
    Coroutine _coroutine;
    Enemy _enemy;
    PlayerController _player;
    [SerializeField, Tooltip("É_ÉÅÅ[ÉWÇÃä‘äu")] float _damageInterval = 1f;
    [SerializeField, Tooltip("à⁄ìÆë¨ìxå∏è≠ÇÃî{ó¶")] float _speedDown;
    float _timer;
    float _enemyTimer;
    float _defaultSpeed;
    float _defaultMove;
    float _defaultEnemySpeed;
    private void Start()
    {
        _player = FindAnyObjectByType<PlayerController>();
        _defaultSpeed = _player._maxSpeed;
        _defaultMove = _player._movePower;
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        _enemyTimer += Time.deltaTime;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (_coroutine != null)
            {
                StopAllCoroutines();
            }
            _player._maxSpeed = _defaultSpeed;
            _player._movePower = _defaultMove;
            _player._maxSpeed *= _speedDown;
            _player._movePower *= _speedDown;
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            _enemy = collision.gameObject.GetComponent<Enemy>();

            _defaultEnemySpeed = _enemy._currentSpeed;
            _enemy._currentSpeed *= _speedDown;

        }

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        //if (collision.gameObject.CompareTag("Player"))
        //{
        //    if (_timer > 1)
        //    {
        //        PlayerDamage();
        //        _timer = 0;
        //    }
        //}

        if (collision.gameObject.CompareTag("Enemy"))
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
        if (collision.gameObject.CompareTag("Player"))
        {
            _player._maxSpeed = _defaultSpeed;
            _player._movePower = _defaultMove;
            //_player.Slow(_speedDown, 2f);
            _coroutine = StartCoroutine(Slow(_speedDown, 2f));
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            _enemy._currentSpeed = _defaultEnemySpeed;
            _enemy.SlowDownScale(_speedDown, 2);
        }
    }
    private void PlayerDamage()
    {
        _player.FluctuationLife(-1);
    }

    IEnumerator Slow(float down, float time)
    {
        float defaultSpeed = _defaultSpeed;
        float defaultMove = _defaultMove;
        _player._movePower *= down;
        _player._maxSpeed *= down;
        yield return new WaitForSeconds(time);
        _player._movePower = defaultSpeed;
        _player._maxSpeed = defaultMove;
    }
}
