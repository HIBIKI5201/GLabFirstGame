using UnityEngine;
using System.Collections;

/// <summary>
/// 沼オブジェクトを管理するクラス
/// </summary>
public class Swamp : MonoBehaviour
{
    [SerializeField] private float _damageInterval = 1f;
    [SerializeField] private float _speedDown;
    
    private float _enemyTimer;
    private float _defaultSpeed;
    private float _defaultMove;
    private float _defaultEnemySpeed;
    
    private Enemy _enemy;
    private PlayerController _player;
    private Coroutine _coroutine;
    
    private void Start()
    {
        _player = FindAnyObjectByType<PlayerController>();
        _defaultSpeed = _player._maxSpeed;
        _defaultMove = _player._movePower;
    }

    private void Update()
    {
        _enemyTimer += Time.deltaTime;
    }
    
    /// <summary>
    /// 沼に入った時
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // 対プレイヤーの処理 
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

        if (collision.gameObject.CompareTag("Enemy")) // 対エネミーの処理
        {
            AudioManager.Instance.PlaySE("damage_enemy");
            _enemy = collision.gameObject.GetComponent<Enemy>();

            _defaultEnemySpeed = _enemy._currentSpeed;
            _enemy._currentSpeed *= _speedDown;
        }
    }

    /// <summary>
    /// 沼にいる間
    /// </summary>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && _enemyTimer > 1)
        {
            AudioManager.Instance.PlaySE("damage_enemy");
            _enemy.LifeFluctuation(-1);
            _enemyTimer = 0;
        }
    }

    /// <summary>
    /// 沼から出た時
    /// </summary>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _player._maxSpeed = _defaultSpeed;
            _player._movePower = _defaultMove;
            _coroutine = StartCoroutine(Slow(_speedDown, 2f));
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            _enemy._currentSpeed = _defaultEnemySpeed;
            _enemy.SlowDownScale(_speedDown, 2);
        }
    }

    private IEnumerator Slow(float down, float time)
    {
        _player._movePower *= down;
        _player._maxSpeed *= down;
        
        yield return new WaitForSeconds(time);
        
        _player._movePower = _defaultMove; 
        _player._maxSpeed = _defaultSpeed;
    }
}
