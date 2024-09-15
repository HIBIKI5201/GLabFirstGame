using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    /// <summary>プレイヤーキャラのHP。これが0になるとゲームオーバー</summary>
    [Header("体力の最大値")]
    [SerializeField][Tooltip("プレイヤーの体力の最大値")] int _maxHp;
    int _currentHp;
    /// <summary>プレイヤーキャラが攻撃を行った時に、エネミーに与えるダメージ。</summary>
    [Header("攻撃力")]
    [SerializeField][Tooltip("プレイヤーの攻撃力")] int _attack;
    /// <summary>プレイヤーキャラクターの移動速度を決める値。数値が高いほど最大速度が高くなる</summary>
    [Header("移動速度の最大値")]
    [SerializeField][Tooltip("プレイヤーの速度の最大値")] float _speed;
    /// <summary>プレイヤーキャラクターの移動速度の加速度を決める値。数値が高いほど最大速度までの加速時間が短い</summary>
    [Header("移動速度の加速度")]
    [SerializeField][Tooltip("プレイヤーの移動速度の加速度")] float _movePower;
    /// <summary>プレイヤーキャラクターのJump時に上方向に掛ける力。</summary>
    [Header("ジャンプ力")]
    [SerializeField][Tooltip("プレイヤーのジャンプ力")] float _jumpPower;
    /// <summary>エネミーからダメージを受けた際、一定時間はダメージを受けないようにする時間。整数値で入力する。1=1秒</summary>
    [Header("無敵時間")]
    [SerializeField][Tooltip("プレイヤーの無敵時間")] int _damageCool;
    /// <summary>接地判定</summary>
   　bool _isJump;
    /// <summary>敵を踏んだ判定</summary>
    bool _isStompEnemy;
    /// <summary>持っているアイテムのリスト</summary>
    List<ItemBase> _itemList = new List<ItemBase>();
    Rigidbody2D _rb;
    float _jumpTimer = 0;
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _currentHp = _maxHp;
    }

    void Update()
    {
        Move();
        Jump();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isJump = false;
            _jumpTimer = 0;
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            _isStompEnemy = true;
            _rb.velocity = new Vector2(_rb.velocity.x,0);
            Debug.Log("踏んだ");
        }
    }
    private void Move()
    {
        //移動処理
        var x = Input.GetAxisRaw("Horizontal");
        _rb.AddForce(new Vector2(x, 0) * _movePower, ForceMode2D.Force);
        if (_rb.velocity.x > _speed)
        {
            _rb.velocity = new Vector2(_speed, _rb.velocity.y);
        }
        else if (_rb.velocity.x < -_speed)
        {
            _rb.velocity = new Vector2(-_speed, _rb.velocity.y);
        }
    }
    private void Jump()
    {
        if (!_isJump && Input.GetKeyDown(KeyCode.Space))
        {
            _rb.AddForce(new Vector2(0, _jumpPower), ForceMode2D.Impulse);
            _isJump = true;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            if (_isStompEnemy)
            {
                _isStompEnemy = false;
                _rb.AddForce(new Vector2(0, _jumpPower), ForceMode2D.Impulse);
            }
        }
        else if (_isStompEnemy)
        {
            _rb.AddForce(new Vector2(0, _jumpPower / 1.5f), ForceMode2D.Impulse);
            _isStompEnemy = false;
        }
        else if (_rb.velocity.y > 0)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * 0.99f);
        }
        //Debug.Log(_isJump);
    }
    public void GetItem(ItemBase item)
    {
        _itemList.Add(item);
    }
    /// <summary>
    /// プレイヤーの体力を引数に渡した数値分増減させます。
    /// </summary>
    /// <param name="value"></param>
    public void FluctuationLife(int value)
    {
        _currentHp += value;
        if (_currentHp > _maxHp)
        {
            _currentHp = _maxHp;
        }
        Debug.Log($"Playerの体力:{_currentHp}");
    }
}
